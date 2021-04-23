FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app


FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
#COPY ["DragonTracker/DragonTracker.csproj", "DragonTracker/"]
COPY ["DragonTracker.csproj", ""]
#RUN dotnet restore "DragonTracker/DragonTracker.csproj"
RUN dotnet restore "./DragonTracker.csproj"
COPY . .
#WORKDIR "/src/DragonTracker"
WORKDIR "/src/."
RUN dotnet build "DragonTracker.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DragonTracker.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
CMD ASPNETCORE_URLS=http://*:$PORT dotnet DragonTracker.dll