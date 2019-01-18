#!/usr/bin/env bash
echo "Deploying ucas-importer..."

ApiKey=$1
Source=$2

dotnet pack ./src/ManageCourses.ApiClient
dotnet nuget push ./src/ManageCourses.ApiClient/bin/**/*.nupkg -k $ApiKey -s $Source || echo "Nuget deploy skipped"
