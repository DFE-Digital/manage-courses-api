# Search and Compare API project

[<img src="https://img.shields.io/nuget/v/DFE.SearchAndCompare.Domain.svg">](https://www.nuget.org/packages/DFE.SearchAndCompare.Domain)

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

