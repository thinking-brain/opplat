# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY src/Opplat.AdminApi/Opplat.AdminApi.csproj src/Opplat.AdminApi/
COPY src/Opplat.Application.Abstractions/Opplat.Application.Abstractions.csproj src/Opplat.Application.Abstractions/
COPY src/Opplat.Application/Opplat.Application.csproj src/Opplat.Application/
COPY src/Opplat.Domain/Opplat.Domain.csproj src/Opplat.Domain/
COPY src/Opplat.Infrastructure/Opplat.Infrastructure.csproj src/Opplat.Infrastructure/
COPY src/Opplat.Shared/Opplat.Shared.csproj src/Opplat.Shared/

RUN dotnet restore src/Opplat.AdminApi/Opplat.AdminApi.csproj

COPY . .
RUN dotnet publish src/Opplat.AdminApi/Opplat.AdminApi.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
EXPOSE 8080
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Opplat.AdminApi.dll"]
