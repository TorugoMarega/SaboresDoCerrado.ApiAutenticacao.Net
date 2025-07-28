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

// --- CONFIGURA��O INICIAL DO SERILOG
// Isso permite que o Serilog logue at� mesmo os erros de inicializa��o da aplica��o.
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateBootstrapLogger();

Log.Information("Iniciando a API de Seguran�a");
var builder = WebApplication.CreateBuilder(args);

// Limpa os provedores de log padr�o e usa o Serilog.
builder.Logging.ClearProviders();

builder.Host.UseSerilog((context, services, configuration) => configuration
    .ReadFrom.Configuration(context.Configuration) // L� appsettings.json
    .ReadFrom.Services(services)
    .Enrich.FromLogContext());

// --- SWAGGER ---
// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer(); // Necess�rio para o Swagger coletar informa��es
builder.Services.AddSwaggerGen();
builder.Services.AddSwaggerGen(c =>
{
    // Configura��o b�sica que voc� j� pode ter
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Goiabada At�mica - API de Seguran�a", Version = "v1" });

    // 1. Define o esquema de seguran�a que a nossa API usa (Bearer Token JWT)
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Insira o token JWT: {seu token}"
    });

    // 2. Adiciona o requisito de seguran�a a todos os endpoints
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
// Limpa o mapa de claims padr�o para usar os nomes originais do JWT ("sub", "role", etc.)
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    // Em produ��o, esta op��o deve ser 'true' para for�ar HTTPS. Altere no config da aplica��o
    options.RequireHttpsMetadata = builder.Configuration.GetValue<bool>("HttpsConfig:Enable");
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        // Valida se a chave usada para assinar o token � a mesma que a API conhece.
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(builder.Configuration["Jwt:Secret"])),

        // Valida se o emissor ("iss") do token � o que esperamos.
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],

        // Valida se o destinat�rio ("aud") do token � o que esperamos.
        ValidateAudience = true,
        ValidAudience = builder.Configuration["Jwt:Audience"],

        // Valida se o token n�o expirou.
        ValidateLifetime = true
    };

    options.Events = new JwtBearerEvents
    {
        // O evento OnChallenge � disparado quando uma autoriza��o falha. Vamos adicionar uma mensagem customizada
        OnChallenge = context =>
        {
            // O HandleResponse() impede que a resposta padr�o seja enviada.
            context.HandleResponse();
            context.Response.StatusCode = 401;
            context.Response.ContentType = "application/json";

            if (string.IsNullOrEmpty(context.Error))
            {
                context.Error = "Unauthorized";
            }
            if (string.IsNullOrEmpty(context.ErrorDescription))
            {
                context.ErrorDescription = "Voc� precisa de um token de autentica��o v�lido para acessar este recurso.";
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
        // Este evento � chamado SE o token for validado com sucesso.
        OnTokenValidated = context =>
        {
            // Pegamos o logger para registrar o sucesso.
            var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
            logger.LogInformation("Token validado com sucesso. Claims presentes: {Claims}", context.Principal.Claims.Select(c => $"{c.Type}: {c.Value}"));
            return Task.CompletedTask;
        },
        // Este evento � chamado SE a autentica��o falhar POR QUALQUER MOTIVO.
        OnAuthenticationFailed = context =>
        {
            var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
            // Logamos a EXCE��O COMPLETA, que nos dir� o motivo exato da falha.
            logger.LogError(context.Exception, "Falha na autentica��o do token.");
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
// Este middleware do Serilog cria logs para cada requisi��o HTTP.
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