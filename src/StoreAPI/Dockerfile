FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build-env
WORKDIR /app

COPY StoreAPI.csproj ./
RUN dotnet restore

COPY . ./

ARG build=Debug
RUN dotnet publish --output /app/out --configuration ${build} -p:EnvironmentName=Production;

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS runtime
WORKDIR /app
COPY --from=build-env /app/out ./

COPY entrypoint.sh ./entrypoint.sh
RUN chmod +x ./entrypoint.sh

#ENTRYPOINT ["dotnet", "StoreAPI.dll"]
ENTRYPOINT ["./entrypoint.sh"]
