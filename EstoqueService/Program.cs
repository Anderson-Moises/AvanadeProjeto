using EstoqueService.Data;
using EstoqueService.Services;
using EstoqueService.Mensagem; // RabbitMQConsumer
using Microsoft.EntityFrameworkCore;
using Common.Jwt;
using Microsoft.OpenApi.Models;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Detecta se está rodando no Docker
bool runningInDocker = Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true";

// Escolhe a connection string certa
string connectionString = runningInDocker
    ? builder.Configuration.GetConnectionString("DockerConnection")!
    : builder.Configuration.GetConnectionString("LocalConnection")!;

// DbContext MySQL
builder.Services.AddDbContext<EstoqueContext>(options =>
    options.UseMySql(
        connectionString,
        new MySqlServerVersion(new Version(8, 0, 43)),
        mySqlOptions => mySqlOptions.EnableRetryOnFailure()
    )
);

// Serviços
builder.Services.AddScoped<IProdutoService, ProdutoService>();

// Acesso à configuração
var configuration = builder.Configuration;

// ✅ RabbitMQConsumer registrado como HostedService
builder.Services.AddSingleton<RabbitMQConsumer>(sp =>
{
    return new RabbitMQConsumer(
        host: configuration["RabbitMQ:Host"]!,
        port: int.Parse(configuration["RabbitMQ:Port"]!),
        username: configuration["RabbitMQ:Username"]!,
        password: configuration["RabbitMQ:Password"]!,
        serviceProvider: sp
    );
});
builder.Services.AddHostedService(sp => sp.GetRequiredService<RabbitMQConsumer>());

// JWT
builder.Services.Configure<JwtSettings>(configuration.GetSection("Jwt"));
builder.Services.AddSingleton<JwtService>();
var jwtSettings = configuration.GetSection("Jwt").Get<JwtSettings>()
    ?? throw new InvalidOperationException("Configuração JWT não encontrada.");

builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = "VendasService",
            ValidAudience = "EstoqueService",
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key))
        };
    });

builder.Services.AddAuthorization();

// Controllers e Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Estoque API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Insira o token JWT no formato: Bearer {seu_token}",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
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

var app = builder.Build();

// Middleware
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Estoque API V1");
    c.RoutePrefix = string.Empty;
});

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Migrações
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<EstoqueContext>();

    try
    {
        var pendingMigrations = context.Database.GetPendingMigrations();
        if (pendingMigrations.Any())
        {
            Console.WriteLine("⏳ Aplicando migrações pendentes...");
            context.Database.Migrate();
            Console.WriteLine("✅ Migrações aplicadas com sucesso!");
        }
        else
        {
            Console.WriteLine("✅ Nenhuma migração pendente detectada. Banco já atualizado.");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"⚠️ Erro ao aplicar migrações: {ex.Message}");
    }
}

app.Run("http://0.0.0.0:5118");
