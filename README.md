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

## Coding

It can be worked on in either VSCode or Visual Studio 2017 as preferred.

The project follows https://semver.org/ version numbering.

## Configuration

For additional details, refer to
https://docs.microsoft.com/en-us/aspnet/core/security/app-secrets?view=aspnetcore-2.1&tabs=windows

https://dfedigital.atlassian.net/wiki/spaces/BaT/pages/389349377/Manage+Courses+API

## Getting started from scratch

### Database setup

#### Mac/linux

```bash
./setup-pg-user.sh
```

### Setup Email functionality

```bash
cd src/ManageCourses.Api/
dotnet user-secrets set email:user the-user-email-address
```

Values available from [GOV.​UK Notify](https://www.notifications.service.gov.uk/)
```bash
cd src/ManageCourses.Api/
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

#### [Search And Compare Api](https://github.com/DFE-Digital/search-and-compare-api) related
```bash
# from .\src\ManageCourses.Api>
dotnet user-secrets set snc:api:key the-search-and-compare-api-key
dotnet user-secrets set snc:api:url the-search-and-compare-api-url
```

## Running the API locally

```bash
cd src/ManageCourses.Api/
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

## Error tracking

This app sends exceptions and errors into [Sentry](https://sentry.io). To enable the integration,
set the SENTRY_DSN environment variable.

# Running tests

```
cd tests\ManageCourses.Tests\
dotnet test
```

## Smoke tests

These need internet access and the following additional user secrets

- credentials:dfesignin:clientsecret
- credentials:dfesignin:username (User name of an existing account on the Dfe Sign in test server)
- credentials:dfesignin:password (...and corresponding password)

# Using the API

The API exposes swagger at `/swagger` thanks to [NSwag](https://github.com/RSuter/NSwag)

You can use the generators to generate client code from the API.
See: https://github.com/RSuter/NSwag/wiki#ways-to-use-the-toolchain

## Database Migrations

These are now managed by the sister project https://github.com/DFE-Digital/manage-courses-backend/

## Shutting down the service and showing the off line page.

Rename the file "app_offline.htm.example" in the root folder to "app_offline.htm"
