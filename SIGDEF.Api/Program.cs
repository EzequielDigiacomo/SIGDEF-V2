using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Pagos.Config;
using Pagos.Extensions;
using SIGDEF.AccesoDatos;
using SIGDEF.Controllers;
using SIGDEF.Entidades.Extensions;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

var frontendUrl = Environment.GetEnvironmentVariable("FRONTEND_URL")
    ?? (builder.Environment.IsDevelopment()
        ? "http://localhost:7112"
        : "https://tufrontend.onrender.com"); // URL por defecto

// Add services to the container.
builder.Services.AddControllers();

// 🔹 CONFIGURAR MERCADO PAGO SETTINGS
builder.Services.Configure<MercadoPagoSettings>(
    builder.Configuration.GetSection("MercadoPagoSettings"));

builder.Services.Configure<CloudinarySettings>(
    builder.Configuration.GetSection("CloudinarySettings"));

builder.Services.AddSingleton<SIGDEF.Services.CloudinaryService>();

// 🔹 REGISTRAR SERVICIOS DE PAGO
builder.Services.AddPaymentServices(builder.Configuration);

// 🔹 CONFIGURAR EL DBCONTEXT (PostgreSQL)
builder.Services.AddDbContext<SIGDeFContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// 🔹 CONFIGURAR LÍMITES DE TAMAÑO DE ARCHIVO
builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 10 * 1024 * 1024; // 10MB
    options.ValueLengthLimit = int.MaxValue;
    options.MultipartHeadersLengthLimit = int.MaxValue;
});

// 🔹 CONFIGURAR KESTREL PARA ACEPTAR ARCHIVOS MÁS GRANDES
builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.MaxRequestBodySize = 10 * 1024 * 1024; // 10MB
    options.Limits.RequestHeadersTimeout = TimeSpan.FromMinutes(2);
    options.Limits.KeepAliveTimeout = TimeSpan.FromMinutes(2);
});

// 🔹 CONFIGURAR AUTENTICACIÓN JWT
ConfigureAuthentication(builder);

// 🔹 CONFIGURAR SWAGGER CON JWT
ConfigureSwagger(builder);

// 🔹 AGREGAR CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.WithOrigins(frontendUrl)
               .AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// 🔹 LOG DE INICIO
Console.WriteLine("========================================");
Console.WriteLine("🚀 SIGDEF API INICIANDO...");
Console.WriteLine("========================================");

// 🔹 MIDDLEWARE SIMPLIFICADO - Solo logging, no manejo de excepciones
app.Use(async (context, next) =>
{
    var requestPath = context.Request.Path;
    var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();

    logger.LogInformation($"📥 REQUEST: {context.Request.Method} {requestPath}");

    try
    {
        await next();
        logger.LogInformation($"✅ RESPONSE: {context.Response.StatusCode} para {requestPath}");
    }
    catch (OperationCanceledException)
    {
        // Cliente canceló la petición - esto es normal, no es un error crítico
        logger.LogWarning($"⚠️ Cliente canceló la petición: {requestPath}");

        if (!context.Response.HasStarted)
        {
            context.Response.StatusCode = 499; // Client Closed Request
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync("{\"error\":\"Petición cancelada\"}");
        }
    }
    catch (Exception ex)
    {
        // Log pero deja que el ExceptionHandler lo maneje
        logger.LogError(ex, $"❌ Error en petición: {requestPath}");
        throw; // Re-lanzar para que el ExceptionHandler lo capture
    }
});

// 🔹 MANEJO GLOBAL DE EXCEPCIONES
app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
        var exceptionHandlerPathFeature = context.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerPathFeature>();
        var exception = exceptionHandlerPathFeature?.Error;

        // No cerrar el servidor por errores de cliente
        if (exception is OperationCanceledException ||
            exception is IOException)
        {
            logger.LogWarning(exception, "⚠️ Error de cliente (no crítico)");
            context.Response.StatusCode = 400;
        }
        else
        {
            logger.LogError(exception, "❌ Error del servidor");
            context.Response.StatusCode = 500;
        }

        context.Response.ContentType = "application/json";

        await context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(new
        {
            error = exception?.Message ?? "Error desconocido",
            type = exception?.GetType().Name
        }));
    });
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "SIGDEF API v1");
        c.RoutePrefix = "swagger";
        c.DefaultModelsExpandDepth(-1);
    });
}

// 🔹 USAR CORS
app.UseCors("AllowAll");
app.UseCors(builder => builder
    .WithOrigins("https://mi-frontend-react.onrender.com")
    .AllowAnyMethod()
    .AllowAnyHeader());

// 🔹 IMPORTANTE: Authentication ANTES de Authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

Console.WriteLine("========================================");
Console.WriteLine("✅ SIGDEF API LISTA");
Console.WriteLine($"🌐 Escuchando en: http://0.0.0.0:5000");
Console.WriteLine($"📚 Swagger disponible en: http://localhost:5000/swagger");
Console.WriteLine("========================================");

app.Run();

Console.WriteLine("========================================");
Console.WriteLine("🛑 SIGDEF API DETENIDA");
Console.WriteLine("========================================");


// ---------------------------------------------
// MÉTODO: CONFIGURAR AUTENTICACIÓN JWT
// ---------------------------------------------
void ConfigureAuthentication(WebApplicationBuilder builder)
{
    var jwtSettings = builder.Configuration.GetSection("Jwt");
    var key = Encoding.UTF8.GetBytes(jwtSettings["Key"] ?? "ClavePorDefectoParaDesarrollo1234567890");

    builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = true,
            ValidIssuer = jwtSettings["Issuer"] ?? "SIGDEF.API",
            ValidateAudience = true,
            ValidAudience = jwtSettings["Audience"] ?? "SIGDEF.CLIENT",
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });
}


// ---------------------------------------------
// MÉTODO: CONFIGURAR SWAGGER + JWT
// ---------------------------------------------
void ConfigureSwagger(WebApplicationBuilder builder)
{
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = "SIGDEF API",
            Version = "v1",
            Description = "API para Sistema de Gestión Deportiva",
            Contact = new OpenApiContact
            {
                Name = "SIGDEF Team",
                Email = "soporte@sigdef.com"
            }
        });

        c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Description = "JWT Authorization header usando Bearer. Ej: 'Bearer 12345abcdef'",
            Name = "Authorization",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.ApiKey,
            Scheme = "Bearer",
            BearerFormat = "JWT"
        });

        c.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                Array.Empty<string>()
            }
        });
    });
}