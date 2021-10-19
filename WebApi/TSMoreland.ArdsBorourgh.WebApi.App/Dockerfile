#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["TSMoreland.ArdsBorough.Api.App/TSMoreland.ArdsBorough.Api.App.csproj", "TSMoreland.ArdsBorough.Api.App/"]
RUN dotnet restore "TSMoreland.ArdsBorough.Api.App/TSMoreland.ArdsBorough.Api.App.csproj"
COPY . .
WORKDIR "/src/TSMoreland.ArdsBorough.Api.App"
RUN dotnet build "TSMoreland.ArdsBorough.Api.App.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TSMoreland.ArdsBorough.Api.App.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TSMoreland.ArdsBorough.Api.App.dll"]