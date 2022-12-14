#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443
EXPOSE 30001
EXPOSE 11112

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Leaderboard/Leaderboard.csproj", "Leaderboard/"]
COPY ["Infrastructure/RockPaperOrleans.csproj", "Infrastructure/"]
COPY ["RockPaperOrleans.Abstractions/RockPaperOrleans.Abstractions.csproj", "RockPaperOrleans.Abstractions/"]
RUN dotnet restore "Leaderboard/Leaderboard.csproj"
COPY . .
WORKDIR "/src/Leaderboard"
RUN dotnet build "Leaderboard.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Leaderboard.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Leaderboard.dll"]