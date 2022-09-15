FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
EXPOSE 30003
EXPOSE 11115
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Rascals/Rascals.csproj", "Rascals/"]
COPY ["Infrastructure/RockPaperOrleans.csproj", "Infrastructure/"]
COPY ["RockPaperOrleans.Abstractions/RockPaperOrleans.Abstractions.csproj", "RockPaperOrleans.Abstractions/"]
RUN dotnet restore "Rascals/Rascals.csproj"
COPY . .
WORKDIR "/src/Rascals"
RUN dotnet build "Rascals.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Rascals.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Rascals.dll"]