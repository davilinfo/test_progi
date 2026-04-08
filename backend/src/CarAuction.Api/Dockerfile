FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["src/CarAuction.Api/CarAuction.Api.csproj", "src/CarAuction.Api/"]
RUN dotnet restore "src/CarAuction.Api/CarAuction.Api.csproj"

COPY . .
WORKDIR "/src/src/CarAuction.Api"
RUN dotnet build "CarAuction.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CarAuction.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Development

COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CarAuction.Api.dll"]
