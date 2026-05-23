FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY apps/api/src/BonAirCampaigns.Api/BonAirCampaigns.Api.csproj apps/api/src/BonAirCampaigns.Api/
RUN dotnet restore apps/api/src/BonAirCampaigns.Api/BonAirCampaigns.Api.csproj
COPY apps/api/src/BonAirCampaigns.Api/ apps/api/src/BonAirCampaigns.Api/
RUN dotnet publish apps/api/src/BonAirCampaigns.Api/BonAirCampaigns.Api.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "BonAirCampaigns.Api.dll"]
