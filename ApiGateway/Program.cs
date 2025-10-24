using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using System.Text;
using Common.Jwt; // Certifique-se de que essa classe está acessível

var builder = WebApplication.CreateBuilder(args);

// 🔧 Carrega o arquivo de configuração do Ocelot
builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);

// 🔐 Carrega as configurações JWT
var jwtSettingsSection = builder.Configuration.GetSection("Jwt");
builder.Services.Configure<JwtSettings>(jwtSettingsSection);

var jwtSettings = jwtSettingsSection.Get<JwtSettings>()
    ?? throw new InvalidOperationException("Configuração JWT não encontrada.");

// 🔐 Configura autenticação JWT
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ClockSkew = TimeSpan.Zero,
            ValidIssuer = "VendasService",
            ValidAudience = "EstoqueService",
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSettings.Key))
        };
    });

builder.Services.AddAuthorization();

// 🔁 Configura Ocelot
builder.Services.AddOcelot();

var app = builder.Build();

// 🔐 Middleware de autenticação/autorização
app.UseAuthentication();
app.UseAuthorization();

// 🚪 Inicia Ocelot
await app.UseOcelot();

// 🌐 Define a URL de execução do ApiGateway
app.Run("http://0.0.0.0:5103");

