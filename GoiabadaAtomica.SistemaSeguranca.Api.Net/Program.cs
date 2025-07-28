using GoiabadaAtomica.ApiAutenticacao.Net.Data;
using GoiabadaAtomica.ApiAutenticacao.Net.Middleware;
using GoiabadaAtomica.SistemaSeguranca.Api.Net.Repository.Impl;
using GoiabadaAtomica.SistemaSeguranca.Api.Net.Repository.Interface;
using GoiabadaAtomica.SistemaSeguranca.Api.Net.Service.Impl;
using GoiabadaAtomica.SistemaSeguranca.Api.Net.Service.Interface;
using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Events;
using System.Reflection;
using System.Text;

// --- CONFIGURAÇÃO INICIAL DO SERILOG
// Isso permite que o Serilog logue até mesmo os erros de inicialização da aplicação.
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateBootstrapLogger();

Log.Information("Iniciando a API de Segurança");
var builder = WebApplication.CreateBuilder(args);

// Limpa os provedores de log padrão e usa o Serilog.
builder.Logging.ClearProviders();

builder.Host.UseSerilog((context, services, configuration) => configuration
    .ReadFrom.Configuration(context.Configuration) // Lê appsettings.json
    .ReadFrom.Services(services)
    .Enrich.FromLogContext());

// --- SWAGGER ---
// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer(); // Necessário para o Swagger coletar informações
builder.Services.AddSwaggerGen();
builder.Services.AddSwaggerGen(c =>
{
    // Configuração básica que você já pode ter
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Goiabada Atômica - API de Segurança", Version = "v1" });

    // 1. Define o esquema de segurança que a nossa API usa (Bearer Token JWT)
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Insira o token JWT: {seu token}"
    });

    // 2. Adiciona o requisito de segurança a todos os endpoints
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
            new string[] {}
        }
    });
});
// --- END SWAGGER ---
var connectionString = builder.Configuration.GetConnectionString("MysqlSegurancaGoiabadaAtomicaDsvConnection");

var typeAdapterConfig = TypeAdapterConfig.GlobalSettings;
typeAdapterConfig.Scan(Assembly.GetExecutingAssembly());

builder.Services.AddDbContext<ApplicationContext>(options =>
    options.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0, 29))));

builder.Services.AddSingleton(typeAdapterConfig);
builder.Services.AddScoped<IMapper, ServiceMapper>();

builder.Services.AddScoped<IRoleRepository, RoleRepositoryImpl>();
builder.Services.AddScoped<IRoleService, RoleServiceImpl>();

builder.Services.AddScoped<IUserRepository, UserRepositoryImpl>();

builder.Services.AddScoped<IAuthService, AuthServiceImpl>();

builder.Services.AddScoped<IUserService, UserServiceImpl>();

builder.Services.AddControllers();
builder.Services.AddHealthChecks();

// --- AUTENTICACAO ---
// Limpa o mapa de claims padrão para usar os nomes originais do JWT ("sub", "role", etc.)
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    // Em produção, esta opção deve ser 'true' para forçar HTTPS. Altere no config da aplicação
    options.RequireHttpsMetadata = builder.Configuration.GetValue<bool>("HttpsConfig:Enable");
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        // Valida se a chave usada para assinar o token é a mesma que a API conhece.
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(builder.Configuration["Jwt:Secret"])),

        // Valida se o emissor ("iss") do token é o que esperamos.
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],

        // Valida se o destinatário ("aud") do token é o que esperamos.
        ValidateAudience = true,
        ValidAudience = builder.Configuration["Jwt:Audience"],

        // Valida se o token não expirou.
        ValidateLifetime = true
    };

    options.Events = new JwtBearerEvents
    {
        // O evento OnChallenge é disparado quando uma autorização falha. Vamos adicionar uma mensagem customizada
        OnChallenge = context =>
        {
            // O HandleResponse() impede que a resposta padrão seja enviada.
            context.HandleResponse();
            context.Response.StatusCode = 401;
            context.Response.ContentType = "application/json";

            if (string.IsNullOrEmpty(context.Error))
            {
                context.Error = "Unauthorized";
            }
            if (string.IsNullOrEmpty(context.ErrorDescription))
            {
                context.ErrorDescription = "Você precisa de um token de autenticação válido para acessar este recurso.";
            }

            var response = new
            {
                statusCode = context.Response.StatusCode,
                message = context.ErrorDescription
            };

            return context.Response.WriteAsJsonAsync(response);
        }
    };

    /* VALIDADOR DE TOKEN
    options.Events = new JwtBearerEvents
    {
        // Este evento é chamado SE o token for validado com sucesso.
        OnTokenValidated = context =>
        {
            // Pegamos o logger para registrar o sucesso.
            var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
            logger.LogInformation("Token validado com sucesso. Claims presentes: {Claims}", context.Principal.Claims.Select(c => $"{c.Type}: {c.Value}"));
            return Task.CompletedTask;
        },
        // Este evento é chamado SE a autenticação falhar POR QUALQUER MOTIVO.
        OnAuthenticationFailed = context =>
        {
            var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
            // Logamos a EXCEÇÃO COMPLETA, que nos dirá o motivo exato da falha.
            logger.LogError(context.Exception, "Falha na autenticação do token.");
            return Task.CompletedTask;
        }
    };*/
});
// ------------------------------------------------------------ 

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}
// Este middleware do Serilog cria logs para cada requisição HTTP.
app.UseSerilogRequestLogging();

app.UsePathBase("/SecApi");
app.UseHealthChecks("/hc");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.UseMiddleware<GlobalExceptionHandlerMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.Run();