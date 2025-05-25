using gerenciador.financas.Application.Services;
using gerenciador.financas.Infra.Vendors.Repositories;
using Microsoft.AspNetCore.Connections;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Adiciona serviços ao container.
var connectionString = builder.Configuration.GetConnectionString("SqlServer");
builder.Services.AddSingleton<ISqlServerConnectionHandler>(provider => new SqlServerConnectionHandler(connectionString));
builder.Services.AddScoped<IClienteRepository, ClienteRepository>();
builder.Services.AddScoped<IClienteService, ClienteService>();

builder.Services.AddControllers(); // Habilita controllers
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Gerenciador de Finanças API", Version = "v1" });
});

var app = builder.Build();

// Configura o pipeline de requisições HTTP.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Gerenciador de Finanças API v1"));
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();



app.Run();
