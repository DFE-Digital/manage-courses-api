# Manage Courses API project

[<img src="https://img.shields.io/nuget/v/GovUk.Education.ManageCourses.ApiClient.svg">](https://www.nuget.org/packages/GovUk.Education.ManageCourses.ApiClient/)
[<img src="https://api.travis-ci.org/DFE-Digital/manage-courses-api.svg?branch=master">](https://travis-ci.org/DFE-Digital/manage-courses-api?branch=master)

## About

This repo provides a dotnet core solution containing:

* An API for managing courses data.
* A Domain to describe the managing course data.
* Regression tests.

The main clients for this API and library are
* https://github.com/DFE-Digital/manage-courses-ui
* https://github.com/DFE-Digital/manage-courses-ucas-importer

## Coding

It can be worked on in either VSCode or Visual Studio 2017 as preferred.

The project follows https://semver.org/ version numbering.

## Configuration

An example of the config keys that are required for Secret Manager are available from:

	src\ManageCourses.Api\appsettings.SecretManager_Example.json


For additional details, refer to
https://docs.microsoft.com/en-us/aspnet/core/security/app-secrets?view=aspnetcore-2.1&tabs=windows

https://dfedigital.atlassian.net/wiki/spaces/BaT/pages/389349377/Manage+Courses+API


## Getting started from scratch

### Database setup

```bash
# from .\src\ManageCourses.Api>
dotnet user-secrets set MANAGE_COURSES_POSTGRESQL_SERVICE_HOST the-host (ie localhost)

dotnet user-secrets set MANAGE_COURSES_POSTGRESQL_SERVICE_PORT the-port (ie 5432)

dotnet user-secrets set PG_DATABASE the-database (will be created if missing and sufficient rights, e.g. 'manage')

dotnet user-secrets set PG_USERNAME the-user-you-created

dotnet user-secrets set PG_PASSWORD the-password-you-set
```

#### Create blank database via psql
```bash
# Connect to database server using
# [MANAGE_COURSES_POSTGRESQL_SERVICE_HOST]
# [MANAGE_COURSES_POSTGRESQL_SERVICE_PORT]
# [PG_USERNAME]

psql -h MANAGE_COURSES_POSTGRESQL_SERVICE_HOST -p [MANAGE_COURSES_POSTGRESQL_SERVICE_PORT]  -U [PG_USERNAME]

# Enter password using
# [PG_PASSWORD]

# Execute sql command using
# [PG_DATABASE]
CREATE DATABASE PG_DATABASE;
```

There's a script for setting up a postgres user and database: `setup-pg-user.sh`


### Setup Email functionality

```bash
# from .\src\ManageCourses.Api>
dotnet user-secrets set email:user the-user-email-address
```

Values available from [GOV.​UK Notify](https://www.notifications.service.gov.uk/)
```bash
# from .\src\ManageCourses.Api>
dotnet user-secrets set email:template_id the-template-id
dotnet user-secrets set email:api_key the-email-api-key
dotnet user-secrets set email:welcome_template_id the-welcome-template-id
```
### Setup Authentication

#### [UI](https://github.com/DFE-Digital/manage-courses-ui) related
```bash
# from .\src\ManageCourses.Api>
dotnet user-secrets set auth:oidc:userinfo_endpoint the-oidc-userinfo-endpoint
```

#### [Importer](https://github.com/DFE-Digital/manage-courses-ucas-importer) related
```bash
# from .\src\ManageCourses.Api>
dotnet user-secrets set api:key the-api-key
```

#### [Search And Compare Api](https://github.com/DFE-Digital/search-and-compare-api) related
```bash
# from .\src\ManageCourses.Api>
dotnet user-secrets set snc:api:key the-search-and-compare-api-key
dotnet user-secrets set snc:api:url the-search-and-compare-api-url
```



## Running the API locally

```bash
# from .\src\ManageCourses.Api>
dotnet run
```

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

Serilog has been configured to spit logs out to both the console
(for `dotnet run` testing & development locally) and Application Insights.

Set the `APPINSIGHTS_INSTRUMENTATIONKEY` environment variable to tell Serilog the application insights key.

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

You can set these by going to `tests\ManageCourses.Tests` and running `dotnet user-secrets set <key> <value>`.

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
- api:key (An arbitrary string to use as a ManageCourses API admin key)

Then run
```
cd tests\ManageCourses.Tests
dotnet test --filter TestCategory=Smoke
```

## Notes

An example of the config keys that are required for Secret Manager are available from:

	src\ManageCourses.Tests\appsettings.SecretManager_Example.json


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

### Rollback / down migration

    cd src\ManageCourses.Domain\
    dotnet ef --startup-project ..\ManageCourses.Api database update [previous migration name]

## Shutting down the service and showing the off line page.
Rename the file "app_offline.htm.example" in the root folder to "app_offline.htm"

