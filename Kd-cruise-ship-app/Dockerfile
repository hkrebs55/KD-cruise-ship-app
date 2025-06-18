FROM mcr.microsoft.com/dotnet/runtime:9.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Kd-cruise-ship-app/Kd-cruise-ship-app.csproj", "Kd-cruise-ship-app/"]
RUN dotnet restore "Kd-cruise-ship-app/Kd-cruise-ship-app.csproj"
COPY . .
WORKDIR "/src/Kd-cruise-ship-app"
RUN dotnet build "./Kd-cruise-ship-app.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Kd-cruise-ship-app.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Kd-cruise-ship-app.dll"]
