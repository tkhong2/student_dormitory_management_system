FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy tất cả các project
COPY ["ContractStudentService.API/ContractStudentService.API.csproj", "ContractStudentService.API/"]
COPY ["ContractStudentService.Application/ContractStudentService.Application.csproj", "ContractStudentService.Application/"]
COPY ["ContractStudentService.Domain/ContractStudentService.Domain.csproj", "ContractStudentService.Domain/"]
COPY ["ContractStudentService.Infrastructure/ContractStudentService.Infrastructure.csproj", "ContractStudentService.Infrastructure/"]

RUN dotnet restore "ContractStudentService.API/ContractStudentService.API.csproj"

COPY . .

RUN dotnet publish "ContractStudentService.API/ContractStudentService.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "ContractStudentService.API.dll"]
