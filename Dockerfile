# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj files first for layer caching
COPY CESIZen_API/CESIZen_API.csproj CESIZen_API/

RUN dotnet restore CESIZen_API/CESIZen_API.csproj

# Copy source code (excluding .env via .dockerignore)
COPY CESIZen_API/ CESIZen_API/

RUN dotnet publish CESIZen_API/CESIZen_API.csproj \
    -c Release \
    -o /app/publish \
    -p:ErrorOnDuplicatePublishOutputFiles=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "CESIZen_API.dll"]
