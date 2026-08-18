# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Services/Catalog.Mongo/Catalog.API/Catalog.Mongo.API.csproj", "Services/Catalog.Mongo/Catalog.API/"]
COPY ["Platform/Platform.API/Platform.API.csproj", "Platform/Platform.API/"]
COPY ["Platform/Platform.Application/Platform.Application.csproj", "Platform/Platform.Application/"]
COPY ["Platform/Platform.Core/Platform.Core.csproj", "Platform/Platform.Core/"]
COPY ["Platform/Platform.Infrastructure/Platform.Infrastructure.csproj", "Platform/Platform.Infrastructure/"]
COPY ["Services/Catalog.Mongo/Catalog.Application/Catalog.Mongo.Application.csproj", "Services/Catalog.Mongo/Catalog.Application/"]
COPY ["Services/Catalog.Mongo/Catalog.Core/Catalog.Mongo.Core.csproj", "Services/Catalog.Mongo/Catalog.Core/"]
COPY ["Services/Catalog.Mongo/Catalog.Infrastructure/Catalog.Mongo.Infrastructure.csproj", "Services/Catalog.Mongo/Catalog.Infrastructure/"]
RUN dotnet restore "./Services/Catalog.Mongo/Catalog.API/Catalog.Mongo.API.csproj"
COPY . .
WORKDIR "/src/Services/Catalog.Mongo/Catalog.API"
RUN dotnet build "./Catalog.Mongo.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Catalog.Mongo.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Catalog.Mongo.API.dll"]