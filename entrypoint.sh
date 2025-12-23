#!/bin/bash
# entrypoint.sh - Script dinámico para Render

set -e  # Detener en caso de error

echo "=========================================="
echo "🚀 INICIANDO APP BOLETERIA API EN RENDER"
echo "=========================================="
echo "• Hora: $(date)"
echo "• Directorio: $(pwd)"
echo "• Usuario: $(whoami)"
echo ""

# ============================================
# 1. CONFIGURAR PUERTO DINÁMICO
# ============================================
export PORT=${PORT:-8080}
export ASPNETCORE_URLS=http://*:${PORT}

echo "=== CONFIGURACIÓN ==="
echo "• Puerto: ${PORT}"
echo "• Entorno: ${ASPNETCORE_ENVIRONMENT:-Production}"
echo "• URLs: ${ASPNETCORE_URLS}"

# ============================================
# 2. CONVERTIR DATABASE_URL DE RENDER
# ============================================
if [ -n "${DATABASE_URL}" ]; then
    echo ""
    echo "=== CONVIRTIENDO DATABASE_URL ==="
    echo "• DATABASE_URL detectada"
    
    # Parsear la URL de PostgreSQL de Render
    # Formato: postgresql://user:password@host:port/database
    
    # Extraer componentes
    DB_URL=${DATABASE_URL}
    
    # Si empieza con postgresql://, convertir a formato .NET
    if [[ $DB_URL == postgresql://* ]]; then
        # Remover el prefijo
        DB_URL=${DB_URL#postgresql://}
        
        # Separar usuario:contraseña y el resto
        USER_PASS=${DB_URL%%@*}
        REST=${DB_URL#*@}
        
        # Separar usuario y contraseña
        DB_USER=${USER_PASS%%:*}
        DB_PASS=${USER_PASS#*:}
        
        # Separar host:puerto y base de datos
        HOST_PORT=${REST%%/*}
        DB_NAME=${REST#*/}
        
        # Separar host y puerto
        DB_HOST=${HOST_PORT%%:*}
        DB_PORT=${HOST_PORT#*:}
        
        # Si no hay puerto, usar 5432 por defecto
        if [ "$DB_PORT" = "$DB_HOST" ]; then
            DB_PORT="5432"
        fi
        
        # Crear connection string para .NET
        CONNECTION_STRING="Host=${DB_HOST};Port=${DB_PORT};Database=${DB_NAME};Username=${DB_USER};Password=${DB_PASS};SSL Mode=Require;Trust Server Certificate=true;"
        
        export ConnectionStrings__DefaultConnection="${CONNECTION_STRING}"
        
        echo "• Host: ${DB_HOST}"
        echo "• Puerto: ${DB_PORT}"
        echo "• Database: ${DB_NAME}"
        echo "• Usuario: ${DB_USER}"
        echo "• Connection String configurada ✓"
    fi
elif [ -n "${ConnectionStrings__DefaultConnection}" ]; then
    echo ""
    echo "=== CONEXIÓN A DB ==="
    echo "• Usando ConnectionStrings__DefaultConnection existente"
else
    echo ""
    echo "⚠️ ADVERTENCIA: No hay configuración de base de datos"
    echo "• DATABASE_URL: ${DATABASE_URL:-No configurada}"
    echo "• ConnectionStrings__DefaultConnection: ${ConnectionStrings__DefaultConnection:-No configurada}"
fi

# ============================================
# 3. EJECUTAR MIGRACIONES (OPCIONAL)
# ============================================
if [ -n "${ConnectionStrings__DefaultConnection}" ]; then
    echo ""
    echo "=== VERIFICANDO MIGRACIONES ==="
    
    # Verificar si tenemos herramientas de EF
    if command -v dotnet-ef &> /dev/null || dotnet ef --help &> /dev/null; then
        echo "• Ejecutando migraciones..."
        
        # Intento 1: Usar dotnet-ef si está disponible
        if command -v dotnet-ef &> /dev/null; then
            dotnet-ef database update || echo "• dotnet-ef falló, intentando alternativa..."
        fi
        
        # Intento 2: Usar dotnet ef
        dotnet ef database update --verbose || echo "• Migración automática falló"
    else
        echo "• Herramientas EF no disponibles, omitiendo migración automática"
        echo "• Sugerencia: Agrega 'DotNetCoreToolsVersion' a tu .csproj"
    fi
else
    echo ""
    echo "⚠️ OMITIENDO MIGRACIONES - Sin conexión a DB"
fi

# ============================================
# 4. VERIFICAR ARCHIVOS DE LA APLICACIÓN
# ============================================
echo ""
echo "=== VERIFICANDO APLICACIÓN ==="
echo "• Archivo DLL principal:"
if [ -f "AppBoleteriaApi.dll" ]; then
    echo "  ✓ AppBoleteriaApi.dll encontrado"
    ls -la AppBoleteriaApi.dll
else
    echo "  ✗ ERROR: AppBoleteriaApi.dll NO encontrado"
    echo "  Archivos en directorio:"
    ls -la
    exit 1
fi

echo ""
echo "• Archivos de configuración:"
for config_file in appsettings.json appsettings.Production.json appsettings.Docker.json; do
    if [ -f "$config_file" ]; then
        echo "  ✓ $config_file"
    fi
done

# ============================================
# 5. INICIAR LA APLICACIÓN
# ============================================
echo ""
echo "=========================================="
echo "🚀 INICIANDO APLICACIÓN .NET"
echo "=========================================="
echo "• Comando: dotnet AppBoleteriaApi.dll"
echo "• Puerto: ${PORT}"
echo "• PID: $$"
echo "• Hora de inicio: $(date)"
echo "=========================================="

# Ejecutar la aplicación
exec dotnet AppBoleteriaApi.dll