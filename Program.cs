using AppBoleteriaApi.Data;
using AppBoleteriaApi.Repositories;
using AppBoleteriaApi.Services;
using AppBoleteriaApi.Middleware;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// =======================
// 1. CONFIGURAR PUERTO
// =======================
var isDevelopment = builder.Environment.IsDevelopment();
string port;

if (isDevelopment)
{
    port = "5237";
    builder.WebHost.UseUrls($"http://0.0.0.0:{port}");
    Console.WriteLine($"🔧 Modo Desarrollo - Puerto: {port}");
}
else
{
    port = Environment.GetEnvironmentVariable("PORT") ?? "10000";
    builder.WebHost.UseUrls($"http://*:{port}");
    Console.WriteLine($"🚀 Modo Producción (Render) - Puerto: {port}");
}

Console.WriteLine($"🌍 Entorno: {builder.Environment.EnvironmentName}");

// =======================
// 2. CORS - DINÁMICO SEGÚN ENTORNO
// =======================
builder.Services.AddCors(options =>
{
    options.AddPolicy("ReactNativeApp",
        policy =>
        {
            // PERMITE EXPLÍCITAMENTE LAS IPs LOCALES
            policy.WithOrigins(
                    "http://localhost:8081",          // Metro bundler
                    "http://localhost:19006",         // Expo
                    "http://192.168.180.146:8081",    // Tu IP con Metro
                    "http://192.168.180.146:19006",   // Tu IP con Expo
                    "http://127.0.0.1:8081",          // Localhost alternativo
                    "http://0.0.0.0:8081",            // Todas las interfaces
                    "capacitor://localhost",          // Capacitor
                    "ionic://localhost"               // Ionic
                )
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();

            Console.WriteLine("🌐 CORS: Configurado para React Native");
        });
});
// =======================
// 3. DATABASE CONTEXT - CONEXIÓN PARA RENDER
// =======================
builder.Services.AddDbContext<AppDbContext>(options =>
{
    string connectionString;

    var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");

    if (!string.IsNullOrEmpty(databaseUrl))
    {
        var uri = new Uri(databaseUrl);
        var userInfo = uri.UserInfo.Split(':');

        var host = uri.Host;
        var portDb = uri.Port > 0 ? uri.Port : 5432;
        var database = uri.AbsolutePath.TrimStart('/');
        var username = userInfo[0];
        var password = userInfo[1];

        connectionString = $"Host={host};Port={portDb};Database={database};Username={username};Password={password};SSL Mode=Require;Trust Server Certificate=true;";

        Console.WriteLine($"✅ Usando base de datos de Render: {host}");
        Console.WriteLine($"📊 Base de datos: {database}");
    }
    else
    {
        connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ??
                          "Host=localhost;Database=boleteria;Username=postgres;Password=postgres";
        Console.WriteLine($"⚠️  Usando conexión local");
    }

    options.UseNpgsql(connectionString, npgsqlOptions =>
    {
        npgsqlOptions.MigrationsHistoryTable("__EFMigrationsHistory", "public");
        npgsqlOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(30),
            errorCodesToAdd: null);
    });
});

// =======================
// 4. JWT AUTHENTICATION
// =======================
var jwtKey = Environment.GetEnvironmentVariable("Jwt__Key") ??
             builder.Configuration["Jwt:Key"];

if (string.IsNullOrWhiteSpace(jwtKey))
    jwtKey = "default-jwt-key-for-development-32-characters-long";

var key = Encoding.ASCII.GetBytes(jwtKey);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });

// =======================
// 5. REPOSITORIES & SERVICES
// =======================
builder.Services.AddScoped<ITenantService, TenantService>();
builder.Services.AddScoped<ICompanyRepository, CompanyRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IEventRepository, EventRepository>();
builder.Services.AddScoped<ITicketTypeRepository, TicketTypeRepository>();
builder.Services.AddScoped<ITicketRepository, TicketRepository>();
builder.Services.AddScoped<IVenueRepository, VenueRepository>();

builder.Services.AddScoped<ICompanyService, CompanyService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IEventService, EventService>();
builder.Services.AddScoped<ITicketTypeService, TicketTypeService>();
builder.Services.AddScoped<ITicketService, TicketService>();
builder.Services.AddScoped<IMenuRouteService, MenuRouteService>();
builder.Services.AddScoped<IVenueService, VenueService>();

// ⬇️⬇️⬇️ NUEVAS LÍNEAS PARA PAYPHONE ⬇️⬇️⬇️
builder.Services.AddHttpClient<IPayPhoneService, PayPhoneService>();
builder.Services.AddScoped<IPayPhoneService, PayPhoneService>();
// ⬆️⬆️⬆️ FIN LÍNEAS NUEVAS ⬆️⬆️⬆️

builder.Services.AddHttpContextAccessor();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// =======================
// 6. MIDDLEWARE PIPELINE
// =======================
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("ReactNativeApp");
app.UseMiddleware<TenantMiddleware>();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// =======================
// 7. ENDPOINTS PARA RENDER
// =======================
app.MapGet("/", () =>
{
    Console.WriteLine($"✅ Health check recibido en / - {DateTime.UtcNow}");
    return Results.Ok(new
    {
        status = "healthy",
        service = "Boleteria API",
        timestamp = DateTime.UtcNow,
        environment = app.Environment.EnvironmentName,
        port = port
    });
});

app.MapGet("/health", () =>
{
    Console.WriteLine($"✅ Health check recibido en /health - {DateTime.UtcNow}");
    return "OK";
});

app.MapGet("/swagger-ui", () => Results.Redirect("/swagger"));

Console.WriteLine($"✅ Aplicación lista en http://0.0.0.0:{port}");
Console.WriteLine($"💳 PayPhone: Integración lista");
app.Run();