FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["TaskManager.API/TaskManager.API.csproj", "TaskManager.API/"]
COPY ["TaskManager.Core/TaskManager.Core.csproj", "TaskManager.Core/"]
COPY ["TaskManager.Infra/TaskManager.Infra.csproj", "TaskManager.Infra/"]

RUN dotnet restore "TaskManager.API/TaskManager.API.csproj"

COPY . .

WORKDIR /src/TaskManager.API
RUN dotnet publish "TaskManager.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 8080
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "TaskManager.API.dll"]
