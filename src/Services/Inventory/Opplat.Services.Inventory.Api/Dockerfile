# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY src/Services/Inventory/Opplat.Services.Inventory.Api/Opplat.Services.Inventory.Api.csproj src/Services/Inventory/Opplat.Services.Inventory.Api/
COPY src/Opplat.Microservices.Shared/Opplat.Microservices.Shared.csproj src/Opplat.Microservices.Shared/
COPY src/Modules/Inventory/Domain/Opplat.Modules.Inventory.Domain.csproj src/Modules/Inventory/Domain/
COPY src/Modules/Inventory/Infrastructure/Opplat.Modules.Inventory.Infrastructure.csproj src/Modules/Inventory/Infrastructure/
COPY src/Opplat.Infrastructure/Opplat.Infrastructure.csproj src/Opplat.Infrastructure/
COPY src/Opplat.Shared/Opplat.Shared.csproj src/Opplat.Shared/

RUN dotnet restore src/Services/Inventory/Opplat.Services.Inventory.Api/Opplat.Services.Inventory.Api.csproj

COPY . .
RUN dotnet publish src/Services/Inventory/Opplat.Services.Inventory.Api/Opplat.Services.Inventory.Api.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
EXPOSE 8080
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Opplat.Services.Inventory.Api.dll"]
