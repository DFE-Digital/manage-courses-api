# Manage Courses API project

## About

This repo provides a dotnet core solution containing:

* An API for managing courses data.
* A Domain to describe the managing course data.
* Regression tests.

The main client for this API and library is
* https://github.com/DFE-Digital/manage-courses-ui
* https://github.com/DFE-Digital/manage-courses-ucas-importer

## Coding

It can be worked on in either VSCode or Visual Studio 2017 as preferred.

The project follows https://semver.org/ version numbering.

## Running the API locally

In a windows command prompt:

    cd src/ManageCourses.Api
    dotnet run

## Config

An example of the config keys that are required for Secret Manager are available from:

	src\ManageCourses.Api\appsettings.SecretManager_Example.json

For additional details, refer to
https://docs.microsoft.com/en-us/aspnet/core/security/app-secrets?view=aspnetcore-2.1&tabs=windows

https://dfedigital.atlassian.net/wiki/spaces/BaT/pages/389349377/Manage+Courses+API

## Logging

Logging is configured in `appsettings.json`, and values in there can be overridden with environment variables.

Powershell:

    $env:Serilog:MinimumLevel="Debug"
    dotnet run

Command prompt

    set Serilog:MinimumLevel=Debug
    dotnet run

For more information see:

* https://github.com/serilog/serilog-settings-configuration
* https://nblumhardt.com/2016/07/serilog-2-minimumlevel-override/

# Running tests

## Unit tests

```
cd tests\ManageCourses.Tests\
dotnet test
```

## Integration tests

You will need to provide a postgresql server. A default localhost installation works. For everything else you can set the following overrides as user secrets:

- MANAGE_COURSES_POSTGRESQL_SERVICE_HOST (the host of the PostgreSQL server)
- MANAGE_COURSES_POSTGRESQL_SERVICE_PORT (its port)
- PG_USERNAME (the log in user)
- PG_PASSWORD (...and corresponding password)
- PG_DATABASE (the database to use)

You can set these by going to `tests\ManageCourses.Tests` and running `dotnet user-secrets add <key> <value>`.

Then run 
```
cd tests\ManageCourses.Tests
dotnet test --filter TestCategory=Integration
```

## Smoke tests

These need internet access and the following additional user secrets

- credentials:dfesignin:clientid (Client ID for the dfe signin test oauth server)
- credentials:dfesignin:clientsecret (...and corresponding secret)
- credentials:dfesignin:host (... and domain name of the test oath server)
- credentials:dfesignin:redirect_host (... and domain name:port of the server to be redirected to - needs to be whitelisted by the test oauth server!)
- auth:oidc:userinfo_endpoint (the user_info endpoint of the dfe signin test oauth server, e.g. https://signin-test-oidc-as.azurewebsites.net/me)
- credentials:dfesignin:username (User name of an existing account on the Dfe Sign in test server)
- credentials:dfesignin:password (...and corresponding password)

Then run
```
cd tests\ManageCourses.Tests
dotnet test --filter TestCategory=Smoke
```

# Using the API

The API exposes swagger at `/swagger` thanks to [NSwag](https://github.com/RSuter/NSwag)

You can use the generators to generate client code from the API.
See: https://github.com/RSuter/NSwag/wiki#ways-to-use-the-toolchain

## Database Migrations

### Add a migration

Make your changes to the model, then from command prompt:

    cd src\ManageCourses.Domain\
    dotnet ef --startup-project ..\ManageCourses.Api migrations add [migration name]

### Drop the database

    cd src\ManageCourses.Domain\
    dotnet ef --startup-project ..\ManageCourses.Api database drop

### Run the migrations

    cd src\ManageCourses.Domain\
    dotnet ef --startup-project ..\ManageCourses.Api database update

If this doesn't work try running the sln in Visual Studio.

