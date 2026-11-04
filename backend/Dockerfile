FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY backend/src/StadsLens.Api/StadsLens.Api.csproj backend/src/StadsLens.Api/
COPY backend/src/StadsLens.Application/StadsLens.Application.csproj backend/src/StadsLens.Application/
COPY backend/src/StadsLens.Contracts/StadsLens.Contracts.csproj backend/src/StadsLens.Contracts/
COPY backend/src/StadsLens.Domain/StadsLens.Domain.csproj backend/src/StadsLens.Domain/
COPY backend/src/StadsLens.Infrastructure/StadsLens.Infrastructure.csproj backend/src/StadsLens.Infrastructure/
RUN dotnet restore backend/src/StadsLens.Api/StadsLens.Api.csproj

COPY backend/src/ backend/src/
RUN dotnet publish backend/src/StadsLens.Api/StadsLens.Api.csproj --configuration Release --no-restore --output /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "StadsLens.Api.dll"]
