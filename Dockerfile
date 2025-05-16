FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["InternshipAPI.csproj", "./"]
RUN dotnet restore "./InternshipAPI.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "InternshipAPI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "InternshipAPI.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Make port configurable via environment variable
ENV ASPNETCORE_URLS=http://+:$PORT
ENV PORT=10000

# Disable HTTPS redirection for Render deployment
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "InternshipAPI.dll"]
