# Get base image and set the source directory
FROM mcr.microsoft.com/dotnet/sdk:8.0-jammy AS build
WORKDIR /source

# Copy .csproj to /source and restore as distinct layers
COPY "./TestApp.Api/TestApp.Api.csproj" .
RUN dotnet restore

# Copy everything else to /source and build the app
# Put the published app in /app directory
COPY "./TestApp.Api/." .
RUN dotnet publish --no-restore -o /app

# Final stage
FROM mcr.microsoft.com/dotnet/nightly/aspnet:8.0-jammy-chiseled-composite
WORKDIR /app

# Copy published app from build stage's /app directory into final stage's /app directory
COPY --from=build /app .

# Provide env variables
ENV ASPNETCORE_ENVIRONMENT Production

# This is how you run the app
# Tells Docker to configure the container to run as an executable
# When container starts, this command runs
ENTRYPOINT ["./TestApp.Api"]