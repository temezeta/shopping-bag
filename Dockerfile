#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 5000
EXPOSE 5001

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["shopping-bag.csproj", "."]
RUN dotnet restore "./shopping-bag.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "shopping-bag.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "shopping-bag.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "shopping-bag.dll"]