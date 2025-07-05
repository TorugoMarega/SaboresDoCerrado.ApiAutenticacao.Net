using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SaboresDoCerrado.ApiAutenticacao.Net.Data;
using SaboresDoCerrado.ApiAutenticacao.Net.Middleware;
using SaboresDoCerrado.ApiAutenticacao.Net.Repository;
using SaboresDoCerrado.ApiAutenticacao.Net.Service;
using System.Reflection;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddEndpointsApiExplorer(); // Necess�rio para o Swagger coletar informa��es
builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration.GetConnectionString("MysqlSegurancaSaboresDoCerradoDsvConnection");

var typeAdapterConfig = TypeAdapterConfig.GlobalSettings;
typeAdapterConfig.Scan(Assembly.GetExecutingAssembly());

builder.Services.AddDbContext<ContextoAplicacao>(options =>
    options.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0, 29))));

builder.Services.AddSingleton(typeAdapterConfig);
builder.Services.AddScoped<IMapper, ServiceMapper>();

builder.Services.AddScoped<IPerfilRepository, PerfilRepository>();
builder.Services.AddScoped<IPerfilService, PerfilService>();

builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();

builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.AddScoped<IUsuarioService, UsuarioService>();

builder.Services.AddControllers();
builder.Services.AddHealthChecks();

// --- AUTENTICACAO ---
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    // Em produ��o, esta op��o deve ser 'true' para for�ar HTTPS.
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
});
// ------------------------------------------------------------ 

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}
app.UsePathBase("/SaboresDoCerradoAuthApi");
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