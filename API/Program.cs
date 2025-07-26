using FluentValidation;
using gerenciador.financas.API.Validators;
using gerenciador.financas.API.ViewModel.Cliente;
using gerenciador.financas.Application.Services;
using gerenciador.financas.Infra.Vendors;
using gerenciador.financas.Infra.Vendors.Repositories;
using Microsoft.OpenApi.Models;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Adiciona serviços ao container.
var connectionString = builder.Configuration.GetConnectionString("SqlServer");

//Dependências
builder.Services.AddScoped<ICategoriaService, CategoriaService>();
builder.Services.AddScoped<IContaService, ContaService>();
builder.Services.AddScoped<IDespesaService, DespesaService>();
builder.Services.AddScoped<IMetaFinanceiraService, MetaFinanceiraService>();
builder.Services.AddScoped<IPagamentoService, PagamentoService>();
builder.Services.AddScoped<IMovimentacaoFinanceiraService, MovimentacaoFinanceiraService>();
builder.Services.AddScoped<IReceitaService, ReceitaService>();
builder.Services.AddScoped<IUsuarioService, UsuarioService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<NotificationPool>();
builder.Services.AddScoped<ICategoriaRepository, CategoriaRepository>();
builder.Services.AddScoped<IContaRepository, ContaRepository>();
builder.Services.AddScoped<IDespesaRepository, DespesaRepository>();
builder.Services.AddScoped<IMetaFinanceiraRepository, MetaFinanceiraRepository>();
builder.Services.AddScoped<IPagamentoRepository, PagamentoRepository>();
builder.Services.AddScoped<IMovimentacaoFinanceiraRepository, MovimentacaoFinanceiraRepository>();
builder.Services.AddScoped<IReceitaRepository, ReceitaRepository>();
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddSingleton<ISqlServerConnectionHandler>(provider => new SqlServerConnectionHandler(connectionString));

//Validators
builder.Services.AddValidatorsFromAssemblyContaining<AtualizarDadosCadastraisRequestViewModelValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<AtualizarSenhaRequestViewModelValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<CadastrarUsuarioRequestViewModelValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<LoginRequestViewModelValidator>();

builder.Services.AddValidatorsFromAssemblyContaining<AtualizarContaRequestViewModel>();
builder.Services.AddValidatorsFromAssemblyContaining<CadastrarContaRequestViewModel>();

builder.Services.AddValidatorsFromAssemblyContaining<AtualizarMetodoPagamentoRequestViewModel>();
builder.Services.AddValidatorsFromAssemblyContaining<CadastrarMetodoPagamentoRequestViewModel>();

builder.Services.AddValidatorsFromAssemblyContaining<AtualizarCategoriaRequestViewModelValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<CadastrarCategoriaRequestViewModel>();


builder.Services.AddCors(options =>
{
    options.AddPolicy("Permitir",
        policy =>
        {
            policy.AllowAnyOrigin() 
                  .AllowAnyMethod() 
                  .AllowAnyHeader(); 
        });
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Gerenciador de Finanças API", Version = "v1" });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Gerenciador de Finanças API v1"));
}

//app.UseHttpsRedirection();
app.UseMiddleware<ValidationMiddleware>();
app.UseDefaultFiles();
app.UseStaticFiles();

app.UseCors("Permitir");

app.UseAuthorization();
app.MapControllers();




app.Run();