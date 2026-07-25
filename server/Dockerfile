FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /build

COPY ["src/Core/Core.csproj", "src/Core/"]
COPY ["src/Modules/Auth/Auth.Domain/Auth.Domain.csproj", "src/Modules/Auth/Auth.Domain/"]
COPY ["src/Modules/Auth/Auth.Application/Auth.Application.csproj", "src/Modules/Auth/Auth.Application/"]
COPY ["src/Modules/Auth/Auth.Api/Auth.Api.csproj", "src/Modules/Auth/Auth.Api/"]
COPY ["src/Modules/Auth/Auth.Infrastructure/Auth.Infrastructure.csproj", "src/Modules/Auth/Auth.Infrastructure/"]
COPY ["src/App/App.csproj", "src/App/"]

RUN dotnet restore src/App/App.csproj

COPY ["tests/TestsCommon/TestsCommon.csproj", "tests/TestsCommon/"]
RUN dotnet restore tests/TestsCommon/TestsCommon.csproj

COPY ["tests/Modules/Auth/Auth.Domain.Tests/Auth.Domain.Tests.csproj", "tests/Modules/Auth/Auth.Domain.Tests/"]
RUN dotnet restore tests/Modules/Auth/Auth.Domain.Tests/Auth.Domain.Tests.csproj

COPY ["tests/Modules/Auth/Auth.Application.Tests/Auth.Application.Tests.csproj", "tests/Modules/Auth/Auth.Application.Tests/"]
RUN dotnet restore tests/Modules/Auth/Auth.Application.Tests/Auth.Application.Tests.csproj

COPY ["tests/Modules/Auth/Auth.Api.Tests/Auth.Api.Tests.csproj", "tests/Modules/Auth/Auth.Api.Tests/"]
RUN dotnet restore tests/Modules/Auth/Auth.Api.Tests/Auth.Api.Tests.csproj

COPY . .

FROM build AS publish
RUN dotnet publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final

WORKDIR /app

COPY --from=publish /build/src/App/bin/Release/net8.0/publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "App.dll"]