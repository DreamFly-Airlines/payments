FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

ARG GITHUB_USERNAME
RUN --mount=type=secret,id=github_personal_access_token \
    GITHUB_PERSONAL_ACCESS_TOKEN="$(cat /run/secrets/github_personal_access_token)" && \
    dotnet nuget add source "https://nuget.pkg.github.com/DreamFly-Airlines/index.json" \
    --name "github" \
    --username $GITHUB_USERNAME \
    --password $GITHUB_PERSONAL_ACCESS_TOKEN \
    --store-password-in-clear-text

ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Payments.Api/Payments.Api.csproj", "src/Payments.Api/"]
COPY ["src/Payments.Application/Payments.Application.csproj", "src/Payments.Application/"]
COPY ["src/Payments.Domain/Payments.Domain.csproj", "src/Payments.Domain/"]
COPY ["src/Payments.Infrastructure/Payments.Infrastructure.csproj", "src/Payments.Infrastructure/"]
    
RUN dotnet restore "src/Payments.Api/Payments.Api.csproj"
COPY . .
WORKDIR "/src/src/Payments.Api"
RUN dotnet build "./Payments.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Payments.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Payments.Api.dll"]
