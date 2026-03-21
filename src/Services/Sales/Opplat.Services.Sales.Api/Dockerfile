# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY src/Services/Sales/Opplat.Services.Sales.Api/Opplat.Services.Sales.Api.csproj src/Services/Sales/Opplat.Services.Sales.Api/
COPY src/Opplat.Microservices.Shared/Opplat.Microservices.Shared.csproj src/Opplat.Microservices.Shared/
COPY src/Modules/Sales/Domain/Opplat.Modules.Sales.Domain.csproj src/Modules/Sales/Domain/
COPY src/Modules/Sales/Infrastructure/Opplat.Modules.Sales.Infrastructure.csproj src/Modules/Sales/Infrastructure/
COPY src/Opplat.Infrastructure/Opplat.Infrastructure.csproj src/Opplat.Infrastructure/
COPY src/Opplat.Shared/Opplat.Shared.csproj src/Opplat.Shared/

RUN dotnet restore src/Services/Sales/Opplat.Services.Sales.Api/Opplat.Services.Sales.Api.csproj

COPY . .
RUN dotnet publish src/Services/Sales/Opplat.Services.Sales.Api/Opplat.Services.Sales.Api.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
EXPOSE 8080
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Opplat.Services.Sales.Api.dll"]
