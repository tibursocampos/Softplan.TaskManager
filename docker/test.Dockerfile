FROM mcr.microsoft.com/dotnet/sdk:8.0 AS test

ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY . .

RUN dotnet restore

RUN dotnet tool install -g dotnet-reportgenerator-globaltool
ENV PATH="${PATH}:/root/.dotnet/tools"

RUN dotnet test TaskManager.Core.Tests/TaskManager.Core.Tests.csproj \
    -c $BUILD_CONFIGURATION \
    --collect:"XPlat Code Coverage" \
    --results-directory ./TestResults/Core \
    --verbosity normal \
 && reportgenerator \
    -reports:TestResults/Core/**/coverage.cobertura.xml \
    -targetdir:coveragereport/core \
    -reporttypes:TextSummary

RUN dotnet test TaskManager.API.Tests/TaskManager.API.Tests.csproj \
    -c $BUILD_CONFIGURATION \
    --collect:"XPlat Code Coverage" \
    --results-directory ./TestResults/API \
    --verbosity normal \
 && reportgenerator \
    -reports:TestResults/API/**/coverage.cobertura.xml \
    -targetdir:coveragereport/api \
    -reporttypes:TextSummary
