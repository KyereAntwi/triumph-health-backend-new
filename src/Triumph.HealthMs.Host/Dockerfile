FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Triumph.HealthMs.Host/Triumph.HealthMs.Host.csproj", "src/Triumph.HealthMs.Host/"]
COPY ["src/Triumph.HealthMs.Commands/Triumph.HealthMs.Commands.csproj", "src/Triumph.HealthMs.Commands/"]
COPY ["src/Triumph.HealthMs.ExternalServices/Triumph.HealthMs.ExternalServices.csproj", "src/Triumph.HealthMs.ExternalServices/"]
COPY ["src/Triumph.HealthMs.Core/Triumph.HealthMs.Core.csproj", "src/Triumph.HealthMs.Core/"]
COPY ["src/Triumph.HealthMs.Persistence/Triumph.HealthMs.Persistence.csproj", "src/Triumph.HealthMs.Persistence/"]
COPY ["src/Triumph.HealthMs.Queries/Triumph.HealthMs.Queries.csproj", "src/Triumph.HealthMs.Queries/"]
RUN dotnet restore "src/Triumph.HealthMs.Host/Triumph.HealthMs.Host.csproj"
COPY . .
WORKDIR "/src/src/Triumph.HealthMs.Host"
RUN dotnet build "./Triumph.HealthMs.Host.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Triumph.HealthMs.Host.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Triumph.HealthMs.Host.dll"]
