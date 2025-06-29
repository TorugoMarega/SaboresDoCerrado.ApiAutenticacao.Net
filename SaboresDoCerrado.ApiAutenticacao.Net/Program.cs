using Mapster;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using SaboresDoCerrado.ApiAutenticacao.Net.Data;
using SaboresDoCerrado.ApiAutenticacao.Net.Middleware;
using SaboresDoCerrado.ApiAutenticacao.Net.Repository;
using SaboresDoCerrado.ApiAutenticacao.Net.Service;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddEndpointsApiExplorer(); // Necessário para o Swagger coletar informações
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
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}
app.UsePathBase("/SaboresDoCerradoAuthApi");
app.UseHealthChecks("/hc");
app.UseAuthorization();
app.MapControllers();
app.UseMiddleware<GlobalExceptionHandlerMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.Run();