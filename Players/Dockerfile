FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
EXPOSE 30002
EXPOSE 11113
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Players/Players.csproj", "Players/"]
COPY ["Infrastructure/RockPaperOrleans.csproj", "Infrastructure/"]
COPY ["RockPaperOrleans.Abstractions/RockPaperOrleans.Abstractions.csproj", "RockPaperOrleans.Abstractions/"]
RUN dotnet restore "Players/Players.csproj"
COPY . .
WORKDIR "/src/Players"
RUN dotnet build "Players.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Players.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Players.dll"]