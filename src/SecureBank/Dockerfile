FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env
WORKDIR /app

COPY SecureBank.csproj ./
RUN dotnet restore

COPY . ./

ARG build=Release
RUN dotnet publish --output /app/out --configuration ${build} -p:EnvironmentName=Production;

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS runtime
WORKDIR /app
COPY --from=build-env /app/out ./

COPY entrypoint.sh ./entrypoint.sh

#ENTRYPOINT ["dotnet", "SecureBank.dll"]

# More universal way of invoking entrypoint
ENTRYPOINT ["bash","entrypoint.sh"]