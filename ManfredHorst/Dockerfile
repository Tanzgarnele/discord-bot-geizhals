#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:6.0.0-focal-arm32v7 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["ManfredHorst/ManfredHorst.csproj", "ManfredHorst/"]
COPY ["DataAccessLibrary/DataAccessLibrary.csproj", "DataAccessLibrary/"]
RUN dotnet restore "ManfredHorst/ManfredHorst.csproj"
COPY . .
WORKDIR "/src/ManfredHorst"
RUN dotnet build "ManfredHorst.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ManfredHorst.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ManfredHorst.dll"]