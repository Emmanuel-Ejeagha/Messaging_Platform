FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["Directory.Build.props", "."]
COPY ["Directory.Packages.props", "."]
COPY ["src/MessagingPlatform.API/MessagingPlatform.API.csproj", "src/MessagingPlatform.API/"]
COPY ["src/MessagingPlatform.Application/MessagingPlatform.Application.csproj", "src/MessagingPlatform.Application/"]
COPY ["src/MessagingPlatform.Domain/MessagingPlatform.Domain.csproj", "src/MessagingPlatform.Domain/"]
COPY ["src/MessagingPlatform.Infrastructure/MessagingPlatform.Infrastructure.csproj", "src/MessagingPlatform.Infrastructure/"]

RUN dotnet restore "src/MessagingPlatform.API/MessagingPlatform.API.csproj"

COPY . .
WORKDIR "/src/src/MessagingPlatform.API"
RUN dotnet build "MessagingPlatform.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "MessagingPlatform.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# HTTP only - no HTTPS in container (use reverse proxy for TLS termination)
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_HTTP_PORTS=8080

HEALTHCHECK --interval=30s --timeout=3s --start-period=40s --retries=3 \
    CMD curl --fail http://localhost:8080/swagger/index.html || exit 1

ENTRYPOINT ["dotnet", "MessagingPlatform.API.dll"]
