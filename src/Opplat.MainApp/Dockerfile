# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy project files first for layer caching of restore step
COPY src/Opplat.MainApp/Opplat.MainApp.csproj src/Opplat.MainApp/
COPY src/Opplat.Domain/Opplat.Domain.csproj src/Opplat.Domain/
COPY src/Opplat.Infrastructure/Opplat.Infrastructure.csproj src/Opplat.Infrastructure/
COPY src/Opplat.Shared/Opplat.Shared.csproj src/Opplat.Shared/
COPY src/Opplat.Microservices.Shared/Opplat.Microservices.Shared.csproj src/Opplat.Microservices.Shared/
COPY src/Modules/Sales/Domain/Opplat.Modules.Sales.Domain.csproj src/Modules/Sales/Domain/
COPY src/Modules/Sales/Application/Opplat.Modules.Sales.Application.csproj src/Modules/Sales/Application/
COPY src/Modules/Sales/Infrastructure/Opplat.Modules.Sales.Infrastructure.csproj src/Modules/Sales/Infrastructure/
COPY src/Modules/Inventory/Domain/Opplat.Modules.Inventory.Domain.csproj src/Modules/Inventory/Domain/
COPY src/Modules/Inventory/Application/Opplat.Modules.Inventory.Application.csproj src/Modules/Inventory/Application/
COPY src/Modules/Inventory/Infrastructure/Opplat.Modules.Inventory.Infrastructure.csproj src/Modules/Inventory/Infrastructure/

# Restore only the app (not the full solution, to avoid missing test project)
RUN dotnet restore src/Opplat.MainApp/Opplat.MainApp.csproj

# Copy full source and publish
COPY . .
RUN dotnet publish src/Opplat.MainApp/Opplat.MainApp.csproj -c Release -o /app/publish --no-restore

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
EXPOSE 8080
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Opplat.MainApp.dll"]
