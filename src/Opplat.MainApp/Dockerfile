# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy project files first for layer caching of restore step
COPY src/Opplat.MainApp/Opplat.MainApp.csproj src/Opplat.MainApp/
COPY src/Opplat.Domain/Opplat.Domain.csproj src/Opplat.Domain/
COPY src/Opplat.Infrastructure/Opplat.Infrastructure.csproj src/Opplat.Infrastructure/
COPY src/Opplat.Shared/Opplat.Shared.csproj src/Opplat.Shared/

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
