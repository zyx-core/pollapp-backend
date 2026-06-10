# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 5000

# Explicitly set the urls so the container listens on port 5000 inside
ENV ASPNETCORE_URLS=http://+:5000

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["PollApp.Backend.csproj", "."]
RUN dotnet restore "./PollApp.Backend.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "./PollApp.Backend.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./PollApp.Backend.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# We need to make sure the uploads directory exists and is writable
RUN mkdir -p /app/wwwroot/uploads
USER root
RUN chown -R app:app /app/wwwroot
USER app

ENTRYPOINT ["dotnet", "PollApp.Backend.dll"]
