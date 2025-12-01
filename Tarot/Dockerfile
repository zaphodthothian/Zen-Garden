FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY src/DyadicMiddleware.Api/DyadicMiddleware.Api.csproj DyadicMiddleware.Api/
RUN dotnet restore DyadicMiddleware.Api/DyadicMiddleware.Api.csproj
COPY src/DyadicMiddleware.Api/. DyadicMiddleware.Api/
WORKDIR /src/DyadicMiddleware.Api
RUN dotnet publish -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "DyadicMiddleware.Api.dll"]
