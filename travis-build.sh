#!/usr/bin/env bash
set -ev
dotnet restore ./src/ManageCourses.Api/ManageCourses.Api.csproj
dotnet restore ./src/ManageCourses.Domain/ManageCourses.Domain.csproj
dotnet restore ./tests/ManageCourses.Tests/ManageCourses.Tests.csproj

dotnet test ./tests/ManageCourses.Tests/ManageCourses.Tests.csproj
dotnet test ./tests/ManageCourses.Tests/ManageCourses.Tests.csproj --filter TestCategory="Integration"
