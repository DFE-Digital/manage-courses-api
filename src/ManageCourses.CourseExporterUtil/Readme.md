## Exporter util

Extracts courses from a Manage Courses instance, converts them to Search and Compare courses and saves the result in a JSON.

Additionally included is a node script that pushes the result to a Search and Compare instance of your choice

### Run it

You will need:
- The connection details of the Manage Courses database you want to get data from
- The API Keys of your target Search and Compare deployment(s)


You also need to whitelist your IP in order to connect to the Manage Courses database. To do so for production,
1. Go to the Azure Portal
2. Find bat-prod-manage-courses-psql
3. Select Connection Security
4. Click Add Client IP
5. Click Save

To get all the secrets you'll need, you'll need Azure acces and run

```
az login
az webapp config appsecrets list -g bat-prod-rgroup -n bat-prod-manage-courses-api-app --output table

az webapp config appsecrets list -g bat-dev-rgroup     -n bat-dev-search-and-compare-api-app     --output table
az webapp config appsecrets list -g bat-staging-rgroup -n bat-staging-search-and-compare-api-app --output table
az webapp config appsecrets list -g bat-prod-rgroup    -n bat-prod-search-and-compare-api-app    --output table
```

Then paste the necessary secrets into a script like this:

```
cd src
cd ManageCourses.CourseExporterUtil

del out.json
set MANAGE_COURSES_POSTGRESQL_SERVICE_HOST=<redacted> && set PG_DATABASE=<redacted> && set PG_USERNAME=<redacted> && set PG_PASSWORD=<redacted> && dotnet run

cd SendToSearch
node index https://bat-dev-search-and-compare-api-app.azurewebsites.net <redacted>
node index https://bat-staging-search-and-compare-api-app.azurewebsites.net <redacted>
node index https://bat-prod-search-and-compare-api-app.azurewebsites.net <redacted>

```