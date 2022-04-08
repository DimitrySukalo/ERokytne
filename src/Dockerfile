FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build

WORKDIR /source
COPY ERokytne.Api/ERokytne.Api.csproj ERokytne.Api/
COPY ERokytne.Persistence/ERokytne.Persistence.csproj ERokytne.Persistence/
COPY ERokytne.Telegram/ERokytne.Telegram.csproj ERokytne.Telegram/
RUN dotnet restore ERokytne.Api/ERokytne.Api.csproj

COPY . .
RUN dotnet publish ERokytne.Api/ERokytne.Api.csproj -c release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
COPY --from=build /app ./
ENTRYPOINT ["dotnet", "ERokytne.Api.dll"]
EXPOSE 80