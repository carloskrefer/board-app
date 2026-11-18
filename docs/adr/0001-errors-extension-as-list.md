# ADR-0001: Represent the `errors` extension as a list of error items

- **Status:** Accepted
- **Date:** 2026-10-30
- **Applies to:** `ApiProblemDetails.Errors`, `DetailedApiError`

## Context

Error responses use the `application/problem+json` format (`ProblemDetails`) and
are extended with an `errors` member. This member must carry:

- One or more errors in a single response (e.g. several invalid fields in the same request).
- Errors tied to a specific input (field + location + rejected value), such as a
  validation failure on `password`.
- Errors **not** tied to any specific input, such as invalid login credentials,
  where we deliberately do not reveal whether the email or the password was wrong.
- A stable machine-readable `code` for every error, so clients can react to errors
  without parsing messages.

## Decision

`errors` is an array of `DetailedApiError` and is always present (empty when the
response has no detailed errors).

```json
{
  "title": "...",
  "detail": "...",
  "status": 400,
  "instance": "/auth/login",
  "traceId": "...",
  "errors": [
    {
      "code": "EMPTY",
      "message": "Password is empty.",
      "field": "password",
      "location": "Body",
      "rejectedValue": null
    }
  ]
}
```

`field`, `location` and `rejectedValue` are optional and only set when the error
relates to a specific input.

## Alternatives considered

### Dictionary keyed by error code
Rejected. Multiple errors can share the same code (e.g. `TEXT.EMPTY` for both `email`
and `password`), so the code is not a unique key.

### Dictionary keyed by field name
Rejected. Not every error is related to a field. Credential failures are reported
as a whole to avoid revealing which credential was wrong, so there is no field to
use as a key.

### Only use `title` and `detail` for singular error not related to a specific field, without an errors collection
Rejected. Every error has a code, so the code must live somewhere. Putting it
outside the collection would require a separate `code` extension, and carrying it
in `title` would break the convention used by other error responses. Keeping the
code inside the collection gives one consistent shape for all errors.

## Consequences

- Clients iterate over `errors`; they cannot look errors up by key.
- Ordering of items is not part of the contract. Tests normalize order before comparing.
- The same shape is used for single and multiple errors, which keeps client handling uniform.
- Adding new optional properties to `DetailedApiError` is backward compatible.
- `ApiProblemDetails` exists to document this response structure in Swagger, provide a consistent 
  model for tests to validate the response body, and support future module separation into microservices, 
  where validating the contents of Bad Request responses exchanged between services may be necessary.
