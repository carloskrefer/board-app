# ADR-0002: Layered validation strategy (controller validators, domain invariants, shared Predicates and Specifications)

- **Status:** Accepted
- **Date:** 2026-11-18
- **Applies to:** Controllers, `Core.Api.Validation` (`IValidator`, `CompositeValidator`), domain entities, services, repositories, `Core.Predicates`, `Core.Patterns.Specification`

## Context

The same business rule (e.g. "what is a valid password") tends to be needed in more than one place: at the API boundary, to reject bad requests early, and inside the domain, to guarantee that an invalid state can never exist. We need a strategy that answers:

- Where does each kind of validation live?
- How do we avoid duplicating rule logic between layers?
- How do we report **all** invalid fields in a single response, with a stable `code`, `field`, `location` and `rejectedValue` (see ADR-0001)?
- How do we avoid leaking internal or sensitive domain rules to API consumers?

Concerns that drive the decision:

- **Usability:** the client should receive every invalid field at once (`"Password and Name are invalid"`), not just the first one the domain happens to throw.
- **Performance:** a request with an invalid email format should be rejected at the controller, without going through service, repository and database.
- **Cohesion:** the rules that define a valid password are domain rules, so they must live in the domain (or in a Core component the domain uses), not only in the controller.
- **Test confidence:** if the domain never accepts an invalid state (it throws), testing the domain is enough to be sure those rules are never violated, and the database state stays valid as long as the other DDD rules are followed. Rules scattered across controllers and services mean duplicated tests and the risk that a new controller or service forgets to validate.
- **Error format control:** 400 and 500 responses must follow our own `ApiProblemDetails` format consistently, regardless of the kind of failure (deserialization, model binding, validation).
- **Information hiding:** some domain rules are internal, legally sensitive or a competitive advantage, and must not be exposed as-is.

## Decision

Validation is split across layers, each with a clear responsibility. Shared rule logic lives in Core as **Predicates** and **Specifications**.

### Dependency overview

| Layer / component | Depends on | Purpose |
| --- | --- | --- |
| Controller | Validators, Service | Runs validators; returns `400` with `ProblemDetails` if invalid |
| Validators | Predicates | Request validation producing `DetailedApiError` items |
| Service | Repository, Domain | Orchestration; simple validations based on repositories or external services |
| Repository | Specifications | Data access; reuses specifications as query filters |
| Domain | Validations inside methods (`Create` and others) | Protects invariants; returns error on invalid state |
| Validations inside domain methods | Predicates, Specifications | Reuse of the same rule logic as the controller |
| Predicates (Core) | none | Reusable `input -> bool` functions |
| Specifications (Core) | none | Reusable `entity -> bool` expressions |

### 1. Disable ASP.NET automatic validation

```csharp
builder.Services
    .AddControllers()
    .ConfigureApiBehaviorOptions(options =>
        options.SuppressModelStateInvalidFilter = true);
```

This disables the automatic `400` for deserialization errors, model binding errors and Data Annotation errors. Whatever happens, the request always reaches the endpoint method, and the worst case is that the parameter is `null`. Therefore, `[FromBody]` parameters are declared nullable (`SignInRequest?`) and validators must check for `null`. This way a `400` never mixes the automatic ASP.NET format with our own.

### 2. Controllers run custom validators

Validators are composable (`IValidator`, `CompositeValidator`, `ForField`, `WithLocation`, `WithHiddenValue`, `OnSuccess`) and produce `DetailedApiError` items directly, with the field path, the location (body, query, etc.) and the rejected value.

```csharp
[HttpPost]
public async Task<IActionResult> SignIn([FromBody] SignInRequest? request, CancellationToken ct = default)
{
    if (!request.Validate(out var errors))
        return problemResultFactory.CreateBadRequest(errors);
    // ...
}
```

```csharp
public class SignInValidator(SignInRequest? request) : CompositeValidator
{
    // ... one NotNull validator + one rule validator per field
    protected override IEnumerable<IValidator> GetValidators() =>
    [
        emailNotNull.OnSuccess(email),
        nameNotNull.OnSuccess(name),
        passwordNotNull.OnSuccess(password),
    ];
}
```

Controller validation covers **basic, input-shape rules**: not null, not empty, maximum length, format (via Predicates), and cross-field rules such as "if field X is filled, field Y is required". Controller validators collect all errors instead of stopping at the first one.

### 3. The domain protects its invariants and returns error

The domain never accepts an invalid state. Domain methods (`Create` and others) validate their inputs and the entity state and return error on violation. This validation is **intentionally repeated** relative to the controller, but the rule logic is not: both call the same Predicates. It also does not repeat relative to repositories, both call the same Specifications.

```csharp
void SignIn(string password)
{
    if (!password.IsValidPassword()) return PasswordError.InvalidFormat;
}
```

### 4. Predicates (Core)

Predicates are reusable functions with an input and a boolean output (e.g. `IsValidPassword`, implemented with a regex). They may use any logic; they are not required to be translatable to SQL. They are used by controller validators and by domain methods, so the rule is defined once.

### 5. Specifications (Core)

Specifications store reusable expressions whose input is a domain entity and whose output is a boolean (e.g. `IsUnderAgeSpecification`). They expose both `IsSatisfiedBy(entity)` for in-memory use in the domain and `ToExpression()` for use in repositories, so the same logic validates in the domain and filters in queries.

```csharp
// Domain
void StartDrivingLicenseProcess()
{
    if (IsUnderAgeSpecification.IsSatisfiedBy(this)) ...
}

// Repository
Task<List<User>> GetAllUnderageUsers() =>
    dbContext.Persons.Where(IsUnderAgeSpecification.ToExpression()).ToListAsync();
```

### 6. Services perform simple, lookup-based validation

Services only do simple validations that depend on repositories or external services, such as `repository.UserExists(id)` followed by an exception when the user does not exist.

### 7. Controller validators are NOT reused in the domain

Controller validators and domain validation are deliberately separate:

- **Validators produce `ApiProblemDetails` errors.** They emit objects with information that only matters at the API boundary (body vs. query param, DTO field name, error code for clients). The domain knows nothing about those concepts.
- **Domain errors are not the same as API errors.** A domain error such as "User lost priority: he spends too little money" must not be exposed. The controller exposes something like "User does not meet requirements, priority lost". Domain errors always need to be mapped to API errors anyway, so reusing the validator in the domain gains nothing.
- **Domain validators would be disproportionate.** Separate domain validators would end up as one validator per method, moving logic out of the method for no benefit. They would also have to protect invariants that depend on state beyond the method parameters, and on intermediate or asynchronous operations, which makes them complex.

## Alternatives considered

### Validate only in the domain

Rejected. The domain throws on the first violation, so clients would see one error at a time. Even if `User.Create` collected all its errors before throwing, the service may call other domain methods afterwards (`user.DoSomethingElse(address)`), and it would have to call every method just to collect validation errors, which is hard to read and error-prone. It also pushes requests that are obviously invalid (e.g. malformed email) all the way to the database before failing.

### Validate only in controllers or services

Rejected. Rules are scattered and duplicated, tests are duplicated, and a new controller or service can create an invalid domain object simply by forgetting to validate. It also contradicts cohesion: the rules that define a valid password are domain rules.

### Data Annotations with `[ApiController]`, like `[Required]`

Rejected. It automatically returns `400` and can fill the Swagger schema (e.g. `maxLength`), but:

- Annotations only carry a message; there is no error code, rejected value, field name or error location, so we cannot build a complete `ApiProblemDetails`.
- Cross-field rules need ever more specific attributes (`RequiredIfAnotherFieldIsX`, then `RequiredIfAnotherFieldIsXAndUserIsAdmin`).

Important: Data Annotations might be used for the sake of documentation (OpenApi) though. But they will have no validation effect
since `DataModel.IsValid` is not read and as previously stated, the applied configuration `SuppressModelStateInvalidFilter` 
disables the automatic 404 response.

### `IValidatableObject` on the DTO

Rejected. The DTO becomes too big and does not generate Swagger documentation.

### FluentValidation (manual validation)

Rejected. It is well-maintained and widely used, and it makes multi-field rules easy. However:

- **Format mismatch.** Its output still has to be converted by hand into our `DetailedApiError` format (code, location, rejected value). A custom validator produces that format natively.
- **Licensing risk.** Several well-known .NET libraries have moved to paid or commercial licenses in the past two years: FluentAssertions 8 (paid for commercial use), AutoMapper and MediatR (Lucky Penny commercial licensing), and MassTransit v9 (commercial release). FluentValidation is still open source (Apache 2.0), but it fits the same profile: a popular, community-maintained library whose maintainers could change their funding model. We would rather not build a core part of our API on that bet.
- **Version management overhead.** Every third-party package has to be tracked and upgraded. Updates are not always backwards compatible, so a routine version bump can break working code.
- **Larger attack surface.** Each dependency adds potential vulnerabilities and supply-chain exposure, plus the risk of an accidental update that pulls in a changed license.

Given this, owning a small validator that does exactly what we need costs less than depending on an external package.

### FluentValidation.AspNetCore (automatic validation)

Rejected. The package is deprecated.

### Action filter that returns `400` from `ModelState`

Rejected. It still depends on `ModelState`, whose format we are not in control of, and it works against the decision to suppress automatic validation and keep a single error shape.

### Reuse controller validators inside the domain

Rejected. See *Decision, section 7*.

## Consequences

- Rule logic is defined once (Predicates and Specifications) and reused by validators, domain and repositories. What is repeated is only the *call* to the rule at each layer, which is intended: the controller gives fast feedback, the domain guarantees integrity.
- Because the domain never accepts invalid state, domain tests are enough to guarantee those rules; controller tests only need to cover request-shape validation and the error format.
- Clients get every invalid field in one `400` response, and invalid requests are rejected before reaching service, repository or database.
- Domain errors must be explicitly mapped to API errors, so internal rules are not exposed.
- Every endpoint must handle a `null` request body, and validators must check for `null`. Forgetting `?` on the parameter type is a risk; code review should watch for it.
- Swagger schema is not filled automatically from our custom validators. If we want constraints such as `maxLength` in the documentation, use the Data Annotation `[MaxLength]` or create your own attribute.
- New validation rules go to Predicates or Specifications first when they are domain rules, and to a controller validator only when they are about the shape of the request.
