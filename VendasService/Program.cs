using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using VendasService.Services;
using VendasService.Mensagem;
using Common.Jwt;
using VendasService.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models; // Se estiver usando a mesma classe JwtSettings

var builder = WebApplication.CreateBuilder(args);

// Escolher a connection string com base em variável de ambiente
// Você pode setar LOCAL=true para rodar localmente
var isLocal = builder.Configuration.GetValue<bool>("LOCAL");
var connectionString = isLocal
    ? builder.Configuration.GetConnectionString("LocalConnection")
    : builder.Configuration.GetConnectionString("DockerConnection");

// Configura o DbContext com retry em caso de falha
builder.Services.AddDbContext<PedidoContext>(options =>
    options.UseMySql(
        connectionString,
        new MySqlServerVersion(new Version(8, 0, 43)),
        mySqlOptions => mySqlOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(10),
            errorNumbersToAdd: null
        )
    )
);

// ✅ Configuração do JWT (lendo do appsettings.json)
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));
builder.Services.AddSingleton<JwtService>(); // Serviço para gerar tokens

// ✅ Configurações JWT
var jwtSettingsSection = builder.Configuration.GetSection("Jwt");
builder.Services.Configure<JwtSettings>(jwtSettingsSection);
var jwtSettings = jwtSettingsSection.Get<JwtSettings>()!
    ?? throw new InvalidOperationException("Configuração JWT não encontrada.");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = "VendasService",
            ValidAudience = "EstoqueService",
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSettings.Key))
        };
    });

builder.Services.AddAuthorization();

// ✅ Injeção de dependência dos serviços
builder.Services.AddScoped<IPedidoService, PedidoService>();
builder.Services.AddScoped<IRabbitMQPublisher, RabbitMQPublisher>();
builder.Services.AddScoped<IPrecoService, PrecoService>();


builder.Services.AddHttpClient<IProdutoService, ProdutoServiceHttp>(client =>
{
    client.BaseAddress = new Uri("http://estoque-service:5118/");
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(
c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Vendas API", Version = "v1" });

    // ✅ Adiciona suporte ao JWT no Swagger
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

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// 🔧 Executa migrações automáticas no MySQL ao iniciar
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<PedidoContext>();

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

app.Run("http://0.0.0.0:5119");
