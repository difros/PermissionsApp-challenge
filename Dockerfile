FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY . .

# Restore dependencies
# WORKDIR /src/API/PermissionsApp
# RUN dotnet restore "./PermissionsApp.csproj"
WORKDIR /src
RUN dotnet restore "API/PermissionsApp/PermissionsApp.csproj"


# Build
# RUN dotnet build "./PermissionsApp.csproj" -c $BUILD_CONFIGURATION -o /app/build
WORKDIR /src/API/PermissionsApp
RUN dotnet build "PermissionsApp.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./PermissionsApp.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PermissionsApp.dll"]
