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
    set ASPNETCORE_URLS=http://*:6001 && dotnet run

## Config

An example of the config keys that are required for Secret Manager are available from:

	src\ManageCourses.Api\appsettings.SecretManager_Example.json

For additional details, refer to
https://docs.microsoft.com/en-us/aspnet/core/security/app-secrets?view=aspnetcore-2.1&tabs=windows

https://dfedigital.atlassian.net/wiki/spaces/BaT/pages/389349377/Manage+Courses+API

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

