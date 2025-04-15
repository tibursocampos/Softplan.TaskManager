FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["TaskManager.API/TaskManager.API.csproj", "TaskManager.API/"]
COPY ["TaskManager.Core/TaskManager.Core.csproj", "TaskManager.Core/"]
COPY ["TaskManager.Infra/TaskManager.Infra.csproj", "TaskManager.Infra/"]
COPY ["TaskManager.API.Tests/TaskManager.API.Tests.csproj", "TaskManager.API.Tests/"]
COPY ["TaskManager.Core.Tests/TaskManager.Core.Tests.csproj", "TaskManager.Core.Tests/"]

RUN dotnet restore "TaskManager.API/TaskManager.API.csproj"

COPY . .

WORKDIR /src/TaskManager.API
RUN dotnet build "./TaskManager.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS test
WORKDIR /src

RUN dotnet test TaskManager.Core.Tests/TaskManager.Core.Tests.csproj -c $BUILD_CONFIGURATION --verbosity normal

RUN dotnet test TaskManager.API.Tests/TaskManager.API.Tests.csproj -c $BUILD_CONFIGURATION --verbosity normal

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "TaskManager.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TaskManager.API.dll"]
