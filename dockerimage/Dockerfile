FROM mcr.microsoft.com/dotnet/sdk:6.0 AS securebank
WORKDIR /app

COPY src/SecureBank/SecureBank.csproj ./
RUN dotnet restore;

COPY src/SecureBank/ ./

RUN dotnet publish --output /app/out --configuration Release --runtime linux-x64 -p:EnvironmentName=Production;

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS storeapi
WORKDIR /app

COPY src/StoreAPI/StoreAPI.csproj ./
RUN dotnet restore

COPY src/StoreAPI/ ./

RUN dotnet publish --output /app/out --configuration Release --runtime linux-x64 -p:EnvironmentName=Production;

FROM mcr.microsoft.com/mssql/server:2019-CU3-ubuntu-16.04

USER root

RUN apt-get update \
    && apt-get install -y curl \
    && curl -sL https://deb.nodesource.com/setup_13.x | bash - \
    && apt-get install -y nodejs

COPY dockerimage/entrypoint.sh ./entrypoint.sh
RUN chmod o+x ./entrypoint.sh

COPY dockerimage/scripts ./scripts
RUN chmod +x -R scripts

COPY --from=securebank /app/out ./SecureBank
RUN mkdir /SecureBank/Ctf

COPY --from=storeapi /app/out ./StoreAPI
    
COPY --from=maildev/maildev:1.1.0 /usr/src/app ./maildev

EXPOSE 80
EXPOSE 5000

EXPOSE 1025
EXPOSE 1080

EXPOSE 1433

ENTRYPOINT ["./entrypoint.sh"]