FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy tất cả các project
COPY ["BillingMaintenanceService.API/BillingMaintenanceService.API.csproj", "BillingMaintenanceService.API/"]
COPY ["BillingMaintenanceService.Application/BillingMaintenanceService.Application.csproj", "BillingMaintenanceService.Application/"]
COPY ["BillingMaintenanceService.Domain/BillingMaintenanceService.Domain.csproj", "BillingMaintenanceService.Domain/"]
COPY ["BillingMaintenanceService.Infrastructure/BillingMaintenanceService.Infrastructure.csproj", "BillingMaintenanceService.Infrastructure/"]

RUN dotnet restore "BillingMaintenanceService.API/BillingMaintenanceService.API.csproj"

COPY . .

RUN dotnet publish "BillingMaintenanceService.API/BillingMaintenanceService.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "BillingMaintenanceService.API.dll"]
