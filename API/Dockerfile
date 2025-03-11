FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base

USER root

WORKDIR /app

RUN mkdir -p /app/images && chown -R app:app /app/images

USER app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["API.csproj", "./"]
RUN dotnet restore "./API.csproj"

COPY . .
RUN dotnet build "API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "API.dll"]