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
    // En desarrollo local, usa el puerto 5237
    port = "5237";
    builder.WebHost.UseUrls($"http://0.0.0.0:{port}");
    Console.WriteLine($"🔧 Modo Desarrollo - Puerto: {port}");
}
else
{
    // En Render, usa el puerto de la variable de entorno
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
            if (builder.Environment.IsDevelopment())
            {
                // ✅ En desarrollo, permite cualquier origen
                policy.SetIsOriginAllowed(_ => true)
                      .AllowAnyHeader()
                      .AllowAnyMethod()
                      .AllowCredentials();

                Console.WriteLine("🌐 CORS: Permitiendo TODOS los orígenes (Desarrollo)");
            }
            else
            {
                // En producción, solo orígenes específicos
                policy.WithOrigins(
                        "https://*.onrender.com",
                        "http://*.onrender.com"
                    )
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();

                Console.WriteLine("🔒 CORS: Solo orígenes de Render (Producción)");
            }
        });
});

// =======================
// 3. DATABASE CONTEXT - CONEXIÓN PARA RENDER
// =======================
builder.Services.AddDbContext<AppDbContext>(options =>
{
    string connectionString;

    // 1. Obtener la URL de Render
    var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");

    if (!string.IsNullOrEmpty(databaseUrl))
    {
        // Parsear la URL de Render: postgresql://user:password@host:port/database
        var uri = new Uri(databaseUrl);
        var userInfo = uri.UserInfo.Split(':');

        var host = uri.Host;
        var portDb = uri.Port > 0 ? uri.Port : 5432;
        var database = uri.AbsolutePath.TrimStart('/');
        var username = userInfo[0];
        var password = userInfo[1];

        // Construir cadena de conexión para Npgsql
        connectionString = $"Host={host};Port={portDb};Database={database};Username={username};Password={password};SSL Mode=Require;Trust Server Certificate=true;";

        Console.WriteLine($"✅ Usando base de datos de Render: {host}");
        Console.WriteLine($"📊 Base de datos: {database}");
    }
    else
    {
        // 2. Fallback a conexión local
        connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ??
                          "Host=localhost;Database=boleteria;Username=postgres;Password=postgres";
        Console.WriteLine($"⚠️  Usando conexión local");
    }

    // Configurar Npgsql con la cadena de conexión
    options.UseNpgsql(connectionString, npgsqlOptions =>
    {
        npgsqlOptions.MigrationsHistoryTable("__EFMigrationsHistory", "public");
        // Configurar reintentos para producción
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

builder.Services.AddHttpContextAccessor();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpContextAccessor();

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

// 1. Endpoint de health check para Render (EN LA RAÍZ)
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

// 2. Endpoint adicional simple
app.MapGet("/health", () =>
{
    Console.WriteLine($"✅ Health check recibido en /health - {DateTime.UtcNow}");
    return "OK";
});

// 3. Redirección para Swagger
app.MapGet("/swagger-ui", () => Results.Redirect("/swagger"));

Console.WriteLine($"✅ Aplicación lista en http://0.0.0.0:{port}");
app.Run();