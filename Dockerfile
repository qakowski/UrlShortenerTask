FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env
WORKDIR /app

RUN apt-get update \
    && apt-get install -y --no-install-recommends \
       unzip \
    && rm -rf /var/lib/apt/lists/* \
    && curl -sSL https://aka.ms/getvsdbgsh | bash /dev/stdin -v latest -l /vsdbg

EXPOSE 4022

COPY ./src /src
RUN dotnet restore /src/UrlShortener.WebApplication/UrlShortener.WebApplication.csproj

COPY . .
WORKDIR /src/UrlShortener.WebApplication
ENV DockerBuild=true
RUN dotnet publish -c Release -o /src/UrlShortener.WebApplication/publish


# Final stage, build the runtime image
FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app

#COPY --from=build-env /app/server.crt /app/server.key ./
COPY --from=build-env /src/UrlShortener.WebApplication/publish ./

ENTRYPOINT ["dotnet", "UrlShortener.WebApplication.dll"]