FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Bakery.Site.csproj", "./"]
RUN dotnet restore "Bakery.Site.csproj"
COPY . .
WORKDIR "/src"
RUN dotnet build "Bakery.Site.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Bakery.Site.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
EXPOSE 5050:80
VOLUME ["/app/App_Data","/app/Upload"]
ENTRYPOINT ["dotnet", "Bakery.Site.dll"]
