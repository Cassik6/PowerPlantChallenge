#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:5.0 AS base
WORKDIR /app
COPY ..
RUN dotnet test	

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["PowerPlantChallenge.Tests/PowerPlantChallenge.Tests.csproj", "PowerPlantChallenge.Tests/"]
COPY ["PowerPlantChallenge/PowerPlantChallenge.csproj", "PowerPlantChallenge/"]
RUN dotnet restore "PowerPlantChallenge.Tests/PowerPlantChallenge.Tests.csproj"
COPY . .
WORKDIR "/src/PowerPlantChallenge.Tests"
RUN dotnet build "PowerPlantChallenge.Tests.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PowerPlantChallenge.Tests.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PowerPlantChallenge.Tests.dll"]