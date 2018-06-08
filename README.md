# Search and Compare API project

## About

This repo provides a dotnet core solution containing:

* An API for managing course data.
* A Domain to describe the managing course data.
* Regression tests.

The main client for this API and library is
* https://github.com/DFE-Digital/manage-courses-ui
* https://github.com/DFE-Digital/manage-courses-ucas-importer

## Coding

It can be worked on in either VSCode or Visual Studio 2017 as preferred.

The domain project follows https://semver.org/ version numbering.

## Running the API locally

In a windows command prompt:

    cd src\api
    set ASPNETCORE_URLS=http://*:6001 && dotnet run

## Config

Refer to
https://docs.microsoft.com/en-us/aspnet/core/security/app-secrets?view=aspnetcore-2.1&tabs=windows

# Using the API

The API exposes swagger at `/swagger` thanks to [NSwag](https://github.com/RSuter/NSwag)

You can use the generators to generate client code from the API.
See: https://github.com/RSuter/NSwag/wiki#ways-to-use-the-toolchain

## Database Migration Setup

From command prompt
X:\repos\dfe\manage-courses-api\src\ManageCourses.Domain>

`dotnet ef --startup-project ../ManageCourses.Api migrations add [migration name]`
