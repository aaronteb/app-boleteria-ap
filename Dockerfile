# Etapa de compilación
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copiar y restaurar dependencias primero (caché eficiente)
COPY ["AppBoleteriaApi.csproj", "."]
RUN dotnet restore "AppBoleteriaApi.csproj"

# Copiar el resto del código
COPY . .
RUN dotnet build "AppBoleteriaApi.csproj" -c Release -o /app/build
RUN dotnet publish "AppBoleteriaApi.csproj" -c Release -o /app/publish

# Etapa de ejecución (imagen más liviana)
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

# Instalar cliente PostgreSQL y herramientas útiles
RUN apt-get update && \
    apt-get install -y \
    postgresql-client \
    curl \
    && rm -rf /var/lib/apt/lists/*

# Copiar archivos publicados
COPY --from=build /app/publish .

# ✅ CONFIGURACIÓN PARA RENDER
EXPOSE 8080
ENV PORT=8080
ENV ASPNETCORE_URLS=http://*:${PORT}
ENV ASPNETCORE_ENVIRONMENT=Production
ENV DOTNET_RUNNING_IN_CONTAINER=true

# ✅ COMANDO SIMPLE - SIN ENTRYPOINT COMPLEJO
CMD ["dotnet", "AppBoleteriaApi.dll"]
