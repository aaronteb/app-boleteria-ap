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
// 1. CONFIGURAR PUERTO PARA RENDER
// =======================
var port = Environment.GetEnvironmentVariable("PORT") ?? "10000"; // CAMBIO AQUÍ: 8080 → 10000
builder.WebHost.UseUrls($"http://*:{port}");

Console.WriteLine($"🚀 Configurando para Render - Puerto: {port}");

// =======================
// 2. CORS
// =======================
builder.Services.AddCors(options =>
{
    options.AddPolicy("ReactNativeApp",
        policy => policy.WithOrigins(
                    "http://localhost:19006",
                    "http://localhost:19000", 
                    "http://10.0.2.2:5000",
                    "http://192.168.180.146:19000",
                    "http://192.168.180.146:5000",
                    "https://*.onrender.com",
                    "http://*.onrender.com"
                )
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials());
});

// =======================
// 3. DATABASE CONTEXT
// =======================
builder.Services.AddDbContext<AppDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? 
                          "Host=localhost;Database=boleteria;Username=postgres;Password=postgres";
    
    options.UseNpgsql(connectionString, npgsqlOptions =>
    {
        npgsqlOptions.MigrationsHistoryTable("__EFMigrationsHistory", "public");
    });
});

// =======================
// 4. JWT AUTHENTICATION
// =======================
var jwtKey = Environment.GetEnvironmentVariable("Jwt__Key") ?? 
             builder.Configuration["Jwt:Key"] ?? 
             "default-jwt-key-for-development-32-characters-long";

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
// 7. ENDPOINTS PARA RENDER (ÚNICO CAMBIO NECESARIO)
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
        environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"
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

Console.WriteLine($"✅ Aplicación lista en puerto: {port}");
app.Run();
