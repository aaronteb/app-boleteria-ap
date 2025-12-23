#!/bin/bash
set -e

echo "=========================================="
echo "🚀 APP BOLETERIA API - RENDER DEPLOYMENT"
echo "=========================================="
echo "• Startup Time: $(date)"
echo "• Working Dir: $(pwd)"
echo "• User: $(whoami)"
echo ""

# ============================================
# 1. CONFIGURE PORT FOR RENDER
# ============================================
export PORT=${PORT:-8080}
export ASPNETCORE_URLS=http://*:${PORT}

echo "=== NETWORK CONFIG ==="
echo "• PORT: ${PORT}"
echo "• ASPNETCORE_URLS: ${ASPNETCORE_URLS}"
echo "• Environment: ${ASPNETCORE_ENVIRONMENT:-Production}"

# ============================================
# 2. CONVERT RENDER'S DATABASE_URL
# ============================================
if [ -n "${DATABASE_URL}" ]; then
    echo ""
    echo "=== DATABASE CONFIG ==="
    
    # Parse DATABASE_URL format: postgresql://user:password@host:port/database
    DB_URL=${DATABASE_URL}
    
    # Remove postgresql:// prefix
    DB_URL=${DB_URL#postgresql://}
    
    # Extract user:password
    USER_PASS=${DB_URL%%@*}
    DB_USER=${USER_PASS%%:*}
    DB_PASS=${USER_PASS#*:}
    
    # Extract host:port/database
    HOST_PORT_DB=${DB_URL#*@}
    HOST_PORT=${HOST_PORT_DB%%/*}
    DB_NAME=${HOST_PORT_DB#*/}
    
    # Extract host and port
    DB_HOST=${HOST_PORT%%:*}
    DB_PORT=${HOST_PORT#*:}
    
    # Default port if not specified
    if [ "$DB_PORT" = "$HOST_PORT" ]; then
        DB_PORT="5432"
    fi
    
    # Create .NET connection string
    CONNECTION_STRING="Host=${DB_HOST};Port=${DB_PORT};Database=${DB_NAME};Username=${DB_USER};Password=${DB_PASS};SSL Mode=Require;Trust Server Certificate=true;"
    
    export ConnectionStrings__DefaultConnection="${CONNECTION_STRING}"
    
    echo "✓ DATABASE_URL converted to ConnectionStrings__DefaultConnection"
    echo "• Host: ${DB_HOST}:${DB_PORT}"
    echo "• Database: ${DB_NAME}"
    echo "• User: ${DB_USER}"
    
elif [ -n "${ConnectionStrings__DefaultConnection}" ]; then
    echo ""
    echo "=== DATABASE CONFIG ==="
    echo "✓ Using existing ConnectionStrings__DefaultConnection"
else
    echo ""
    echo "⚠️ WARNING: No database configuration found"
    echo "• DATABASE_URL: ${DATABASE_URL:-Not set}"
fi

# ============================================
# 3. VALIDATE APPLICATION FILES
# ============================================
echo ""
echo "=== APPLICATION VALIDATION ==="

# Check for required files
if [ -f "AppBoleteriaApi.dll" ]; then
    echo "✓ AppBoleteriaApi.dll"
else
    echo "✗ AppBoleteriaApi.dll - MISSING"
    exit 1
fi

if [ -f "appsettings.json" ]; then
    echo "✓ appsettings.json"
else
    echo "✗ appsettings.json - MISSING"
    exit 1
fi

# ============================================
# 4. START THE APPLICATION
# ============================================
echo ""
echo "=========================================="
echo "🚀 STARTING .NET APPLICATION"
echo "=========================================="
echo "• Command: dotnet AppBoleteriaApi.dll"
echo "• Port: ${PORT}"
echo "• Environment: ${ASPNETCORE_ENVIRONMENT:-Production}"
echo "• Process ID: $$"
echo "• Time: $(date)"
echo "=========================================="

# Start the application
exec dotnet AppBoleteriaApi.dll
