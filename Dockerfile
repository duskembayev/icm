FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /build
ARG CONFIGURATION=Release

COPY ./src/ICM/*.csproj ./src/ICM/
COPY ./src/ICM.Models/*.csproj ./src/ICM.Models/
COPY ICM.sln .
COPY Directory.Build.props .
COPY nuget.config .
COPY global.json .

RUN dotnet restore

COPY ./src/ ./src/

RUN dotnet publish ./src/ICM/ICM.csproj -o ./out/api/ --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS api
WORKDIR /app

COPY --from=build /build/out/api .

ENTRYPOINT ["dotnet", "ICM.dll"]
