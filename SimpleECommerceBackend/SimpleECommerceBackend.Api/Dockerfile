FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["SimpleECommerceBackend.Api/SimpleECommerceBackend.Api.csproj", "SimpleECommerceBackend.Api/"]
COPY ["SimpleECommerceBackend.Application/SimpleECommerceBackend.Application.csproj", "SimpleECommerceBackend.Application/"]
COPY ["SimpleECommerceBackend.Domain/SimpleECommerceBackend.Domain.csproj", "SimpleECommerceBackend.Domain/"]
COPY ["SimpleECommerceBackend.Infrastructure/SimpleECommerceBackend.Infrastructure.csproj", "SimpleECommerceBackend.Infrastructure/"]
RUN dotnet restore "SimpleECommerceBackend.Api/SimpleECommerceBackend.Api.csproj"
COPY . .
WORKDIR "/src/SimpleECommerceBackend.Api"
RUN dotnet build "./SimpleECommerceBackend.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./SimpleECommerceBackend.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SimpleECommerceBackend.Api.dll"]
