# Board
Board is the backend application of a kanban board app. Made for studying purposes.

# Commands
Running the application in development mode:
1. Be sure .NET SDK 8 is installed
2. Go to the solution's root folder
3. Spin up a new Postgres Database
4. Verify the `ConnectionStrings` in `src/App/appsettings.Development.json`
5. `dotnet run --project ./src/App`

Testing the application without containers:
1. Be sure .NET SDK 8 is installed
2. Go to the solution's root folder
3. Spin up a new Postgres Database
4. Change the connection string in `src/App/appsettings.Development.json`
5. `dotnet test`

Testing the application with containers:
1. Be sure .NET SDK 8 is installed
2. Be sure Docker Engine is installed and running
3. Go to the solution's root folder
4. Create `./.env` file containing the required variables mentioned in `compose.test.yml`
5. `docker compose -f compose.test.yml up --build --abort-on-container-exit --exit-code-from tests`

# Technologies
- ASP.NET Core Controller-Based Web Apis
- xUnit tests
- EF Core
- RabbitMQ
- PostgreSQL

# Architecture and patterns
- Clean Architecture
- EDA
- Outbox Pattern
- Modular Monolith
- Clean Code
- DDD
- Repository Pattern
- Unit of Work
- Optimistic Concurrency
- REST
- Result Pattern
- Create Pattern

# Folder structure
Startup webapi project _App_:
- /src/App

Core classlib project for shared code:
- /src/Core

Public DTOs and errors:
- /src/Core/Api/DTOs

Modules (e.g. _Module1_ and _Module2_):
- /src/Modules/Module1
- /src/Modules/Module2

Each module folder must have the following classlib projects:
- `<module_name>.Api`
- `<module_name>.Application`
- `<module_name>.Domain`
- `<module_name>.Infrastructure`

Module installation should happen by calling the method:
- File: `<module_name>ModuleInstaller.cs`
- Class: `<module_name>ModuleInstaller`
- Method: `Add<module_name>Module(...)`

Tests:
- /tests/

Core test project for shared code:
- /src/TestsCommon

Each module's layer must have it's own test project (e.g. _Module1_):
- /tests/Modules/Module1.Api.Tests
- /tests/Modules/Module1.Application.Tests
- /tests/Modules/Module1.Domain.Tests
- /tests/Modules/Module1.Infrastructure.Tests

# Bounded Contexts
A module can define single Bounded Context or it can be a part of a multiple module wide Bounded Context.

The existing bounded contexts and the related modules are defined bellow:
- Auth (Module Auth) 
- Board (Module Board)

Each bounded contexts uses its own language.

# Patterns
# Overall Patterns
- Exceptions should not be used for flow control. Only for unexpected errors (500 errors).
  - Domain and Application should use Result Pattern.

## Domain Layer Patterns
Follows DDD's domain modelling rules:
- Includes Value Objects (VOs), Entities, Aggregate Roots (ARs), Domain Services and Specifications.
- VOs, Entities and ARs instances should always be valid. Therefore during a creation call it should validate.
- VOs, Entities and ARs should have no explicit constructors or it should have `private` constructors.
  - Instantiation must happen using Create Pattern and Result Pattern.
  - Validation should happen manually in the create method. Other strategies such as the use of Data Annotations or 
  `IValidateObject` are not allowed.
- An Entity can only be a part of a single AR.
- Only ARs should be able to instantiate and manipulate it's own Entities.
  - Entities method must have `internal` access.
- One Aggregate should not contain or directly manipulate other Aggregates.

## Application Layer Patterns
- Each application layer service is called an _Use Case Service_ and must implement the `IUseCase` interface.
- Use Cases Services retrieve and change data through Repository interfaces. 
  - Each Aggregate Root must have it's own single repository.
  - Each repository should only manipulate data inside it's aggregate.
  - Repositories are responsible for change tracking, but never for saving changes.
- Use Case Services must save changes using `IUnitofWork` or similar interface. 
- Use Case Services must only directly manipulate a single aggregate. Manipulation of other aggregates should be 
indirect, through Events.
  - Eventual Consistency between aggreagates should be expected
- The application layer can instantiate an Aggregate Root manually or gets it from it's repository.

## API Layer Patterns
- ASP.NET Core Controller-Based Web APIs
- 

# Extra project and layer information
## App Project
- "Glues" modules together
- Contains configuration files (appsettings, launchsettings)

## Auth Module
- Provides application agnostic authentication.
- Since it is not coupled (not even conceptually) to any specific application, it won't contain role definitions.
- Works with short lived JWT tokens in body and Refresh Token (session id) in cookies. 

# Module's Api Tests
- Includes integration tests using WebApplicationFactory (in proccess communication, does not use sockets).

## TestsCommon
- Includes shared code to avoid repetition, including commonly used assertions (e.g. BadRequestAssertions class).