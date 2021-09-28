#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
COPY ["Botsboxadminbot/Files/", "/filestop/"]
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["Botsboxadminbot/Botsboxadminbot.csproj", "Botsboxadminbot/"]
RUN dotnet restore "Botsboxadminbot/Botsboxadminbot.csproj"
COPY . .
WORKDIR "/src/Botsboxadminbot"
RUN dotnet build "Botsboxadminbot.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Botsboxadminbot.csproj" -c Release -o /app/publish

FROM base AS final
RUN mkdir /postgresdata
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Botsboxadminbot.dll"]