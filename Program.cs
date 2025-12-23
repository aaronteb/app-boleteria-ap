using AppBoleteriaApi.Data;
using AppBoleteriaApi.Repositories;
using AppBoleteriaApi.Services;
using AppBoleteriaApi.Middleware;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System; // ← AÑADIR ESTO

var builder = WebApplication.CreateBuilder(args);

// ============================================
// ✅ CONFIGURACIÓN DINÁMICA PARA RENDER (AGREGA ESTO)
// ============================================
var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
var urls = $"http://*:{port}";

Console.WriteLine($"=== CONFIGURACIÓN PARA RENDER ===");
Console.WriteLine($"• Puerto: {port}");
Console.WriteLine($"• URLs: {urls}");
Console.WriteLine($"• Entorno: {Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development"}");

// Configurar Kestrel para escuchar en todas las interfaces
builder.WebHost.UseUrls(urls);
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(int.Parse(port));
    options.Limits.MaxRequestBodySize = 52428800; // 50MB
});

// Si Render proporciona DATABASE_URL, convertir a ConnectionStrings__DefaultConnection
var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
if (!string.IsNullOrEmpty(databaseUrl))
{
    Console.WriteLine($"• Convirtiendo DATABASE_URL de Render...");

    try
    {
        // Parsear DATABASE_URL de Render (postgresql://user:pass@host:port/db)
        var uri = new Uri(databaseUrl);
        var userInfo = uri.UserInfo.Split(':');

        var connectionString = $"Host={uri.Host};Port={uri.Port};Database={uri.AbsolutePath.TrimStart('/')};" +
                               $"Username={userInfo[0]};Password={userInfo[1]};" +
                               $"SSL Mode=Require;Trust Server Certificate=true;";

        builder.Configuration["ConnectionStrings:DefaultConnection"] = connectionString;
        Console.WriteLine($"• Connection String configurada desde DATABASE_URL");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"⚠️ Error convirtiendo DATABASE_URL: {ex.Message}");
    }
}
else
{
    Console.WriteLine($"• Usando ConnectionStrings:DefaultConnection del appsettings");
}
Console.WriteLine($"=================================");

// =======================
// 🔌 1. CORS - AGREGAR DOMINIO DE RENDER
// =======================
builder.Services.AddCors(options =>
{
    options.AddPolicy("ReactNativeApp",
        policy =>
        {
            policy.WithOrigins(
                    "http://localhost:19006",     // Expo web
                    "http://localhost:19000",     // Expo dev server
                    "http://10.0.2.2:5000",       // Android emulator
                    "http://192.168.180.146:19000", // Tu IP local
                    "http://192.168.180.146:5000",  // Tu IP local API
                    "https://*.onrender.com",     // ✅ AGREGAR ESTO para Render
                    "http://*.onrender.com"       // ✅ AGREGAR ESTO para Render HTTP
                )
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
        });
});

// =======================
// 🔌 2. Add DbContext
// =======================
builder.Services.AddDbContext<AppDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

    if (string.IsNullOrEmpty(connectionString))
    {
        Console.WriteLine("⚠️ ADVERTENCIA: ConnectionStrings:DefaultConnection está vacío");
        connectionString = "Host=localhost;Database=boleteria;Username=postgres;Password=postgres";
    }

    Console.WriteLine($"• Database: {connectionString.Split(';')[0].Replace("Host=", "")}");

    options.UseNpgsql(connectionString, npgsqlOptions =>
    {
        npgsqlOptions.MigrationsHistoryTable("__EFMigrationsHistory", "public");
        npgsqlOptions.EnableRetryOnFailure(3, TimeSpan.FromSeconds(5), null);
    });
});

// =======================
// 🏢 3. Add Tenant Service (Scoped)
// =======================
builder.Services.AddScoped<ITenantService, TenantService>();

// =======================
// 🔐 4. Add JWT Authentication (CON SEGURIDAD PARA PRODUCCIÓN)
// =======================
var jwtKey = Environment.GetEnvironmentVariable("Jwt__Key") ?? builder.Configuration["Jwt:Key"];
var jwtIssuer = Environment.GetEnvironmentVariable("Jwt__Issuer") ?? builder.Configuration["Jwt:Issuer"] ?? "app-boleteria-api";
var jwtAudience = Environment.GetEnvironmentVariable("Jwt__Audience") ?? builder.Configuration["Jwt:Audience"] ?? "app-boleteria-api";

if (string.IsNullOrEmpty(jwtKey))
{
    Console.WriteLine("⚠️ ADVERTENCIA: JWT Key no configurada. Usando clave temporal para desarrollo.");
    jwtKey = "clave-temporal-super-secreta-para-desarrollo-solo-64-chars-1234567890";
}

var key = Encoding.ASCII.GetBytes(jwtKey);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false; // Render maneja HTTPS
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = true,
        ValidIssuer = jwtIssuer,
        ValidateAudience = true,
        ValidAudience = jwtAudience,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});

// =======================
// 📌 5. Add Repositories
// =======================
builder.Services.AddScoped<ICompanyRepository, CompanyRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IEventRepository, EventRepository>();
builder.Services.AddScoped<ITicketTypeRepository, TicketTypeRepository>();
builder.Services.AddScoped<ITicketRepository, TicketRepository>();
builder.Services.AddScoped<IVenueRepository, VenueRepository>();

// =======================
// ⚙ 6. Add Services
// =======================
builder.Services.AddScoped<ICompanyService, CompanyService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IEventService, EventService>();
builder.Services.AddScoped<ITicketTypeService, TicketTypeService>();
builder.Services.AddScoped<ITicketService, TicketService>();
builder.Services.AddScoped<IMenuRouteService, MenuRouteService>();
builder.Services.AddScoped<IVenueService, VenueService>();
builder.Services.AddHttpContextAccessor();

// =======================
// 🌐 7. Add Controllers
// =======================
builder.Services.AddControllers();

// =======================
// 📘 8. Swagger (HABILITAR EN PRODUCCIÓN TAMBIÉN)
// =======================
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Boleteria API",
        Version = "v1",
        Description = "API para sistema de boletería"
    });
});

var app = builder.Build();

// =======================
// 🔧 9. Middleware Pipeline
// =======================

// ✅ HABILITAR SWAGGER EN PRODUCCIÓN TAMBIÉN (útil para debug)
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Boleteria API v1");
    options.RoutePrefix = "api-docs"; // Accede en /api-docs
});

// Health check endpoint simple
app.MapGet("/health", () =>
{
    return Results.Ok(new
    {
        status = "healthy",
        timestamp = DateTime.UtcNow,
        service = "Boleteria API",
        version = "1.0"
    });
});

app.MapGet("/", () =>
{
    return Results.Redirect("/api-docs");
});

// Solo redirigir HTTPS si no estamos en Render
if (!app.Environment.IsDevelopment() && !app.Environment.EnvironmentName.Contains("Render"))
{
    app.UseHttpsRedirection();
}

app.UseCors("ReactNativeApp");
app.UseMiddleware<TenantMiddleware>();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// Log final de configuración
Console.WriteLine($"🚀 Aplicación iniciada en: {string.Join(", ", urls)}");
Console.WriteLine($"🌍 Entorno: {app.Environment.EnvironmentName}");
Console.WriteLine($"📅 Hora: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC");

app.Run();