#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["GameController/GameController.csproj", "GameController/"]
COPY ["Infrastructure/RockPaperOrleans.csproj", "Infrastructure/"]
COPY ["RockPaperOrleans.Abstractions/RockPaperOrleans.Abstractions.csproj", "RockPaperOrleans.Abstractions/"]
COPY ["RockPaperOrleans.Grains/RockPaperOrleans.Grains.csproj", "RockPaperOrleans.Grains/"]
RUN dotnet restore "GameController/GameController.csproj"
COPY . .
WORKDIR "/src/GameController"
RUN dotnet build "GameController.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "GameController.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "GameController.dll"]