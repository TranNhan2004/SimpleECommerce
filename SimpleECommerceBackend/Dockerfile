# =========================
# 1. Build stage
# =========================
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /app

COPY SimpleECommerce.sln .

COPY SimpleECommerce.Api/*.csproj SimpleECommerce.Api/
COPY SimpleECommerce.Application/*.csproj SimpleECommerce.Application/
COPY SimpleECommerce.Domain/*.csproj SimpleECommerce.Domain/
COPY SimpleECommerce.Infrastructure/*.csproj SimpleECommerce.Infrastructure/

RUN dotnet restore

COPY . .

RUN dotnet publish SimpleECommerce.Api/SimpleECommerce.Api.csproj \
    -c Release \
    -o /publish \
    /p:UseAppHost=false

# =========================
# 2. Runtime stage
# =========================
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

EXPOSE 8080

ENV ASPNETCORE_URLS=http://+:8080 \
    ASPNETCORE_ENVIRONMENT=Production

COPY --from=build /publish .

ENTRYPOINT ["dotnet", "SimpleECommerce.Api.dll"]
