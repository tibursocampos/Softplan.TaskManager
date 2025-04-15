FROM mcr.microsoft.com/dotnet/sdk:8.0

WORKDIR /src
COPY . .

RUN dotnet restore

RUN dotnet tool install -g dotnet-reportgenerator-globaltool
ENV PATH="${PATH}:/root/.dotnet/tools"

CMD bash -c "\
    dotnet test TaskManager.Core.Tests/TaskManager.Core.Tests.csproj \
        -c Release \
        --collect:\"XPlat Code Coverage\" \
        --results-directory ./TestResults/Core && \
    dotnet test TaskManager.API.Tests/TaskManager.API.Tests.csproj \
        -c Release \
        --collect:\"XPlat Code Coverage\" \
        --results-directory ./TestResults/API && \
    reportgenerator \
        -reports:\"./TestResults/Core/**/coverage.cobertura.xml\" \
        -targetdir:\"coveragereport/core\" \
        -reporttypes:TextSummary && \
    reportgenerator \
        -reports:\"./TestResults/API/**/coverage.cobertura.xml\" \
        -targetdir:\"coveragereport/api\" \
        -reporttypes:TextSummary && \
    cat coveragereport/core/Summary.txt && \
    cat coveragereport/api/Summary.txt"
