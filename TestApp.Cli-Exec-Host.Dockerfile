FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build
WORKDIR /source

COPY "./TestApp.Cli/TestApp.Cli.csproj" .
RUN dotnet restore

COPY "./TestApp.Cli/." .
RUN dotnet publish --no-restore -o /app

FROM mcr.microsoft.com/dotnet/runtime:8.0-alpine
WORKDIR /background
COPY "./keep_alive.sh" .
# Ensure the file is executable
RUN chmod +x /background/keep_alive.sh

WORKDIR /app
COPY --from=build /app .

# When container starts, this command runs
ENTRYPOINT ["/background/keep_alive.sh"]