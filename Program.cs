using Microsoft.OpenApi;
using System.Text;
using KanbanBackend.Data;
using KanbanBackend.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// ---------- 1. Database (PostgreSQL via Neon) ----------
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrWhiteSpace(connectionString))
    throw new InvalidOperationException("ConnectionStrings:DefaultConnection is not configured.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));

// ---------- 2. JWT Authentication ----------
var jwtSecret = builder.Configuration["Jwt:Secret"];
var jwtIssuer = builder.Configuration["Jwt:Issuer"];
var jwtAudience = builder.Configuration["Jwt:Audience"];

if (string.IsNullOrWhiteSpace(jwtSecret) || jwtSecret.Length < 32)
    throw new InvalidOperationException("Jwt:Secret is missing or shorter than 32 characters.");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtIssuer,
        ValidAudience = jwtAudience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret))
    };
});

builder.Services.AddAuthorization();

// ---------- 3. Custom services ----------
builder.Services.AddScoped<TokenService>();

// ---------- 4. CORS (origins config/env se aayenge) ----------
// Comma-separated allowed hai, jaise: "https://sprintboard.netlify.app,http://localhost:4200"
var allowedOrigins = (builder.Configuration["Cors:AllowedOrigin"] ?? "")
    .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
    .Select(o => o.TrimEnd('/'))
    .ToArray();

// Agar kuch configure nahi hai toh sirf local Angular allow hoga
if (allowedOrigins.Length == 0)
    allowedOrigins = new[] { "http://localhost:4200" };

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(allowedOrigins)
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// ---------- 5. Controllers + Swagger ----------
builder.Services.AddControllers()
    .AddJsonOptions(o => o.JsonSerializerOptions.ReferenceHandler =
        System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Description = "Login se mila token yahan paste karo"
    });

    options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference("Bearer", document)] = []
    });
});

var app = builder.Build();

// ---------- Middleware pipeline ----------
// Swagger: Render pe testing ke liye hamesha on (baad mein chaaho toh IsDevelopment() mein daal dena)
app.UseSwagger();
app.UseSwaggerUI();

// HTTPS redirection sirf local mein (Render khud HTTPS handle karta hai)
if (app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseCors("AllowFrontend"); // Authentication se pehle

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapGet("/debug/check-config", async (IConfiguration config) =>
{
    var result = new Dictionary<string, string>();

    // 1. Jwt Secret: HS256 ke liye kam se kam 32 characters chahiye
    var secret = config["Jwt:Secret"];
    result["Jwt__Secret"] = string.IsNullOrEmpty(secret) ? "MISSING"
        : secret.Length < 32 ? $"TOO SHORT ({secret.Length} chars, 32+ chahiye)"
        : $"OK ({secret.Length} chars)";

    // 2. Issuer / Audience: sirf set hain ya nahi
    result["Jwt__Issuer"] = string.IsNullOrEmpty(config["Jwt:Issuer"]) ? "MISSING" : "SET";
    result["Jwt__Audience"] = string.IsNullOrEmpty(config["Jwt:Audience"]) ? "MISSING" : "SET";

    // 3. Database: asli connection try karo
    try
    {
        var cs = config.GetConnectionString("DefaultConnection");
        if (string.IsNullOrEmpty(cs)) result["DB"] = "MISSING";
        else
        {
            await using var conn = new Npgsql.NpgsqlConnection(cs);
            await conn.OpenAsync();
            result["DB"] = "CONNECTED OK";
        }
    }
    catch (Exception ex)
    {
        result["DB"] = "FAILED: " + ex.GetType().Name + " | " + ex.Message
    + " | inner: " + ex.InnerException?.GetType().Name + " " + ex.InnerException?.Message;
    }

    return Results.Json(result);
});

app.Run();