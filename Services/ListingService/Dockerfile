FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
RUN apt-get update \
    && apt-get install -y --no-install-recommends curl \
    && rm -rf /var/lib/apt/lists/*
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["ListingService.WebApi/ListingService.WebApi.csproj", "ListingService.WebApi/"]
COPY ["ListingService.Application/ListingService.Application.csproj", "ListingService.Application/"]
COPY ["ListingService.Domain/ListingService.Domain.csproj", "ListingService.Domain/"]
COPY ["ListingService.Infrastructure/ListingService.Infrastructure.csproj", "ListingService.Infrastructure/"]

RUN dotnet restore "ListingService.WebApi/ListingService.WebApi.csproj"

COPY . .
RUN dotnet publish "ListingService.WebApi/ListingService.WebApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "ListingService.WebApi.dll"]
