# Use the official .NET 8.0 SDK image for building
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy solution file
COPY EV_SCMMS.sln ./

# Copy project files
COPY src/EV_SCMMS.Core/EV_SCMMS.Core.csproj ./src/EV_SCMMS.Core/
COPY src/EV_SCMMS.Infrastructure/EV_SCMMS.Infrastructure.csproj ./src/EV_SCMMS.Infrastructure/
COPY src/EV_SCMMS.WebAPI/EV_SCMMS.WebAPI.csproj ./src/EV_SCMMS.WebAPI/

# Restore dependencies
RUN dotnet restore EV_SCMMS.sln

# Copy all source files
COPY src/ ./src/

# Build the application
RUN dotnet build EV_SCMMS.sln -c Release --no-restore

# Publish the WebAPI project
RUN dotnet publish src/EV_SCMMS.WebAPI/EV_SCMMS.WebAPI.csproj -c Release -o /app/publish --no-build

# Use the official .NET 8.0 runtime image for running
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Create a non-root user
RUN groupadd -r appuser && useradd -r -g appuser appuser

# Copy published application from build stage
COPY --from=build /app/publish .

# Change ownership of the app directory to appuser
RUN chown -R appuser:appuser /app

# Switch to non-root user
USER appuser

# Expose port 5020 (matches launchSettings.json)
EXPOSE 5020

# Set environment variables
ENV ASPNETCORE_URLS=http://+:5020
ENV ASPNETCORE_ENVIRONMENT=Production

# Health check
HEALTHCHECK --interval=30s --timeout=10s --start-period=60s --retries=3 \
    CMD curl -f http://localhost:5020/health || exit 1

# Entry point
ENTRYPOINT ["dotnet", "EV_SCMMS.WebAPI.dll"]