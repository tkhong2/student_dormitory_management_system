FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy tất cả các project
COPY ["RoomBuildingService.API/RoomBuildingService.API.csproj", "RoomBuildingService.API/"]
COPY ["RoomBuildingService.Application/RoomBuildingService.Application.csproj", "RoomBuildingService.Application/"]
COPY ["RoomBuildingService.Domain/RoomBuildingService.Domain.csproj", "RoomBuildingService.Domain/"]
COPY ["RoomBuildingService.Infrastructure/RoomBuildingService.Infrastructure.csproj", "RoomBuildingService.Infrastructure/"]

RUN dotnet restore "RoomBuildingService.API/RoomBuildingService.API.csproj"

COPY . .

RUN dotnet publish "RoomBuildingService.API/RoomBuildingService.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "RoomBuildingService.API.dll"]