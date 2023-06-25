#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["wcc.rating.api/wcc.rating.api.csproj", "wcc.rating.api/"]
# COPY ["wcc.rating.kernel/wcc.rating.kernel.csproj", "wcc.rating.kernel/"]
# COPY ["wcc.rating.data/wcc.rating.data.csproj", "wcc.rating.data/"]
# COPY ["wcc.rating.integrations/wcc.rating.integrations.csproj", "wcc.rating.integrations/"]
# COPY ["wcc.rating/wcc.rating.csproj", "wcc.rating/"]
RUN dotnet restore "wcc.rating.api/wcc.rating.api.csproj"
COPY . .
WORKDIR "/src/wcc.rating.api"
RUN dotnet build "wcc.rating.api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "wcc.rating.api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
# COPY cert /usr/local/cert
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "wcc.rating.api.dll"]