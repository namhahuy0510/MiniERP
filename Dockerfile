# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj và restore
COPY MiniERP/*.csproj ./MiniERP/
RUN dotnet restore ./MiniERP/MiniERP.csproj

# Copy toàn bộ source và build
COPY . .
WORKDIR /src/MiniERP
RUN dotnet publish MiniERP.csproj -c Release -o /app/publish

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# Render sẽ cấp biến môi trường PORT
ENV ASPNETCORE_URLS=http://0.0.0.0:${PORT}

ENTRYPOINT ["dotnet", "MiniERP.dll"]
