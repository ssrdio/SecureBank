FROM mcr.microsoft.com/dotnet/sdk:10.0 AS securebank
WORKDIR /app

COPY src/SecureBank/SecureBank.csproj ./
RUN dotnet restore;

COPY src/SecureBank/ ./

RUN dotnet publish --output /app/out --configuration Release --runtime linux-musl-x64 --self-contained true -p:EnvironmentName=Production;

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS storeapi
WORKDIR /app

COPY src/StoreAPI/StoreAPI.csproj ./
RUN dotnet restore

COPY src/StoreAPI/ ./

RUN dotnet publish --output /app/out --configuration Release --runtime linux-musl-x64 --self-contained true -p:EnvironmentName=Production;

FROM postgres:16-alpine

USER root

# Install Node.js (needed for maildev) and bash and ICU libs (needed for .NET)
RUN apk add --no-cache nodejs bash icu-libs libgcc libstdc++

COPY dockerimage/entrypoint.sh ./entrypoint.sh
RUN chmod +x ./entrypoint.sh

COPY dockerimage/scripts ./scripts
RUN chmod +x -R scripts

COPY --from=securebank /app/out ./SecureBank
RUN mkdir -p /SecureBank/Ctf

COPY --from=storeapi /app/out ./StoreAPI
    
COPY --from=maildev/maildev:2.2.1 /home/node ./maildev

EXPOSE 80
EXPOSE 5000

EXPOSE 1025
EXPOSE 1080

EXPOSE 5432

ENTRYPOINT ["./entrypoint.sh"]