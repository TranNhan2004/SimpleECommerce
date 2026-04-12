# =========================
# 1. Build stage
# =========================
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /app

COPY SimpleECommerceBackend.Api/*.csproj SimpleECommerceBackend.Api/
COPY SimpleECommerceBackend.Application/*.csproj SimpleECommerceBackend.Application/
COPY SimpleECommerceBackend.Domain/*.csproj SimpleECommerceBackend.Domain/
COPY SimpleECommerceBackend.Infrastructure/*.csproj SimpleECommerceBackend.Infrastructure/

RUN dotnet restore SimpleECommerceBackend.Api/SimpleECommerceBackend.Api.csproj

COPY . .

RUN dotnet publish SimpleECommerceBackend.Api/SimpleECommerceBackend.Api.csproj \
    -c Release \
    -o /publish \
    /p:UseAppHost=false

# =========================
# 2. Runtime stage
# =========================
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

EXPOSE 5001

ENV ASPNETCORE_URLS=http://+:5001 \
    ASPNETCORE_ENVIRONMENT=Production

COPY --from=build /publish .

ENTRYPOINT ["dotnet", "SimpleECommerceBackend.Api.dll"]
