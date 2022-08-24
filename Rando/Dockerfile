#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Rando/Rando.csproj", "Rando/"]
COPY ["Infrastructure/RockPaperOrleans.csproj", "Infrastructure/"]
COPY ["RockPaperOrleans.Abstractions/RockPaperOrleans.Abstractions.csproj", "RockPaperOrleans.Abstractions/"]
RUN dotnet restore "Rando/Rando.csproj"
COPY . .
WORKDIR "/src/Rando"
RUN dotnet build "Rando.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Rando.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Rando.dll"]