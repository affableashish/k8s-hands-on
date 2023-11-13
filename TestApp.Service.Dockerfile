FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build
WORKDIR /source

COPY "./TestApp.Api/TestApp.Api.csproj" .
COPY "./TestApp.Service/TestApp.Service.csproj" .
RUN dotnet restore

COPY "./TestApp.Service/." .
RUN dotnet publish --no-restore -o /app

FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["./TestApp.Service"]