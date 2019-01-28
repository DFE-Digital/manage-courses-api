#!/bin/bash
set -e

echo "Deploying ucas-importer..."

# script for deploying webjobs to any environment, choosing the matching deploy password environment variable

# arg 1: environment to deploy to
env=$1

# convert env to uppercase to match environment variable case
upperenv=`echo "$env" | tr '[:lower:]' '[:upper:]'`

eval "password=\$APP_CREDENTIALS_${upperenv}_PWD"

deployZip=deploy.zip

echo "Removing old $deployZip if any..."
[ -e $deployZip ] && rm $deployZip

echo "running dotnet publish..."
dotnet publish src/ManageCourses.UcasCourseImporter/importer --configuration Release

echo "creating zip..."
zip $deployZip -qr src/ManageCourses.UcasCourseImporter/importer/bin/Release/netcoreapp2.1/publish

echo "uploading to azure WebJob..."
curl -X PUT -u "\$bat-$env-manage-courses-api-app:$password" --data-binary @$deployZip --header "Content-Type: application/zip" --header "Content-Disposition: attachment; filename=$deployZip" "https://bat-$env-manage-courses-api-app.scm.azurewebsites.net/api/triggeredwebjobs/ucas-import/"

echo "Deploy complete."
