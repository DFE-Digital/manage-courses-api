FROM microsoft/dotnet:sdk AS build-env
WORKDIR /app

COPY ./nuget.config .

RUN mkdir -p ./src/ManageCourses.Api
RUN mkdir -p ./src/ManageCourses.ApiClient
RUN mkdir -p ./src/ManageCourses.Domain

# Copy csproj and restore as distinct layers
COPY ./src/ManageCourses.Api/*.csproj ./src/ManageCourses.Api/
COPY ./src/ManageCourses.ApiClient/*.csproj ./src/ManageCourses.ApiClient/
COPY ./src/ManageCourses.Domain/*.csproj ./src/ManageCourses.Domain/

WORKDIR /app/src/ManageCourses.Api/
RUN dotnet restore

WORKDIR /app

# Copy everything else and build
COPY . ./

WORKDIR /app/src/ManageCourses.Api/
RUN dotnet publish -c Release -o out

# Build runtime image
FROM microsoft/dotnet:aspnetcore-runtime
WORKDIR /app
COPY --from=build-env /app/src/ManageCourses.Api/out .
ENTRYPOINT ["dotnet", "ManageCourses.Api.dll"]
