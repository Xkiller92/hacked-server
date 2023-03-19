FROM mcr.microsoft.com/dotnet/sdk:7.0 as build

WORKDIR /src

COPY ./hacked-instance_handler.csproj .

RUN dotnet restore

COPY . .

RUN dotnet publish --no-restore -c Release -o /published src/hacked-instance_handler.csproj

FROM mcr.microsoft.com/dotnet/aspnet:7.0 as runtime

# Uncomment the line below if running with HTTPS
# ENV ASPNETCORE_URLS=https://+:443

WORKDIR /app

COPY --from=build /published .

ENTRYPOINT [ "dotnet", "StockData.dll" ]