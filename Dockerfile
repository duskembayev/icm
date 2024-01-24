FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /build
ARG CONFIGURATION=Release

COPY ./src/ICM/*.csproj ./src/ICM/
COPY ./src/ICM.Models/*.csproj ./src/ICM.Models/
COPY ./src/ICM.Fetcher/*.csproj ./src/ICM.Fetcher/

COPY ICM.sln .
COPY Directory.Build.props .
COPY nuget.config .
COPY global.json .

RUN dotnet restore

COPY ./src/ ./src/

RUN dotnet publish ./src/ICM/ICM.csproj -o ./out/api/ --no-restore
RUN dotnet publish ./src/ICM.Fetcher/ICM.Fetcher.csproj -o ./out/fetcher/ --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS api
WORKDIR /app

COPY --from=build /build/out/api .

ENTRYPOINT ["dotnet", "ICM.dll"]

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS fetcher
WORKDIR /app

COPY --from=build /build/out/fetcher .

ENTRYPOINT ["dotnet", "ICM.Fetcher.dll"]
