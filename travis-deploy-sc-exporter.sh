#!/usr/bin/env bash
echo "Deploying sc-exporter..."

deployZip=deploy.zip

echo "Removing old $deployZip if any..."
[ -e $deployZip ] && rm $deployZip

echo "running dotnet publish..."
dotnet publish src/ManageCourses.CourseExporterUtil --configuration Release

echo "creating zip..."
zip $deployZip -r src/ManageCourses.CourseExporterUtil/bin/Release/netcoreapp2.1/publish

echo "uploading to azure WebJob..."
curl -X PUT -u "$1" --data-binary @$deployZip --header "Content-Type: application/zip" --header "Content-Disposition: attachment; filename=$deployZip" https://$2.scm.azurewebsites.net/api/triggeredwebjobs/sc-exporter/

echo "Deploy complete."
