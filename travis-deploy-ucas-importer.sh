#!/usr/bin/env bash

deployZip=deploy.zip

echo "Removing old $deployZip if any..."
[ -e $deployZip ] && rm $deployZip

echo "running dotnet publish..."
dotnet publish src/ManageCourses.UcasCourseImporter/importer --configuration Release

echo "creating zip..."
zip $deployZip -r src/ManageCourses.UcasCourseImporter/importer/bin/Release/netcoreapp2.1/publish

echo "uploading to azure WebJob..."
curl -X PUT -u "$1" --data-binary @$deployZip --header "Content-Type: application/zip" --header "Content-Disposition: attachment; filename=$deployZip" https://$2.scm.azurewebsites.net/api/triggeredwebjobs/ucas-import/

echo "Deploy complete."
