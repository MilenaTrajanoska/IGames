#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["IGames.Web/IGames.Web.csproj", "IGames.Web/"]
RUN dotnet restore "IGames.Web/IGames.Web.csproj"
COPY . .
WORKDIR "/src/IGames.Web"
RUN dotnet build "IGames.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "IGames.Web.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "IGames.Web.dll"]