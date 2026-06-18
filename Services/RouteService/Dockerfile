FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
RUN apt-get update \
    && apt-get install -y --no-install-recommends curl \
    && rm -rf /var/lib/apt/lists/*
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["RouteService.WebApi/RouteService.WebApi.csproj", "RouteService.WebApi/"]
COPY ["RouteService.Application/RouteService.Application.csproj", "RouteService.Application/"]
COPY ["RouteService.Domain/RouteService.Domain.csproj", "RouteService.Domain/"]
COPY ["RouteService.Infrastructure/RouteService.Infrastructure.csproj", "RouteService.Infrastructure/"]

RUN dotnet restore "RouteService.WebApi/RouteService.WebApi.csproj"

COPY . .
RUN dotnet publish "RouteService.WebApi/RouteService.WebApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "RouteService.WebApi.dll"]
