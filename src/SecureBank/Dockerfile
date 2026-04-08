FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build-env
WORKDIR /app

COPY SecureBank.csproj ./
RUN dotnet restore

COPY . ./

ARG build=Release
RUN dotnet publish --output /app/out --configuration ${build} -p:EnvironmentName=Production;

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build-env /app/out ./

COPY entrypoint.sh ./entrypoint.sh
RUN chmod +x ./entrypoint.sh

# Expose the default ports
EXPOSE 80 81 5000 1080

# Set environment variables
ENV ASPNETCORE_URLS=http://+:80
ENV ASPNETCORE_ENVIRONMENT=Production

# Use the entrypoint script for flexible port configuration
ENTRYPOINT ["bash","entrypoint.sh"]