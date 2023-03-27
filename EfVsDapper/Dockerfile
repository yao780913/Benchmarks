FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["EfVsDapper/EfVsDapper.csproj", "EfVsDapper/"]
RUN dotnet restore "EfVsDapper/EfVsDapper.csproj"
COPY . .
WORKDIR "/src/EfVsDapper"
RUN dotnet build "EfVsDapper.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "EfVsDapper.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "EfVsDapper.dll"]
