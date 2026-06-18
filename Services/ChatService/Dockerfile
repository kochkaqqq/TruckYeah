FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
RUN apt-get update \
    && apt-get install -y --no-install-recommends curl \
    && rm -rf /var/lib/apt/lists/*
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["ChatService.WebApi/ChatService.WebApi.csproj", "ChatService.WebApi/"]
COPY ["ChatService.Application/ChatService.Application.csproj", "ChatService.Application/"]
COPY ["ChatService.Domain/ChatService.Domain.csproj", "ChatService.Domain/"]
COPY ["ChatService.Infrastructure/ChatService.Infrastructure.csproj", "ChatService.Infrastructure/"]

RUN dotnet restore "ChatService.WebApi/ChatService.WebApi.csproj"

COPY . .
RUN dotnet publish "ChatService.WebApi/ChatService.WebApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "ChatService.WebApi.dll"]
