FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build
WORKDIR /source

COPY "./TestApp.Cli/TestApp.Cli.csproj" .
RUN dotnet restore

COPY "./TestApp.Cli/." .
RUN dotnet publish --no-restore -o /app

FROM mcr.microsoft.com/dotnet/runtime:8.0-alpine
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["./TestApp.Cli"]