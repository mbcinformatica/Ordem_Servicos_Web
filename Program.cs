<<<<<<< HEAD
using Microsoft.EntityFrameworkCore;
using Ordem_Servicos_Web.Data;
using Ordem_Servicos_Web.Filters;
using Ordem_Servicos_Web.Models;
using Ordem_Servicos_Web.Services;
using Ordem_Servicos_Web.Services.Interfaces;
using Serilog;
using Log = Serilog.Log;

var builder = WebApplication.CreateBuilder(args);

// Serviços
builder.Services.AddScoped<PermissaoService>();
builder.Services.AddScoped<VerificaBancoFilter>();

// Registrar ImageService
builder.Services.AddScoped<IImageService, ImageService>();

// registra IHttpContextAccessor
builder.Services.AddHttpContextAccessor();

// Session
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// MVC + filtros globais
builder.Services.AddControllersWithViews(options =>
{
    options.Filters.AddService<VerificaBancoFilter>();
});

// Serilog (configuração única)
builder.Host.UseSerilog((ctx, lc) => lc
=======
using Microsoft.AspNetCore.Server.IISIntegration; // necessário para IISDefaults
using Microsoft.EntityFrameworkCore;
using Ordem_Servicos_Web.Data;
using Serilog;
using Serilog.Debugging;

var builder = WebApplication.CreateBuilder(args);

// Configuração Serilog (logs em arquivo e MySQL)
Log.Logger = new LoggerConfiguration()
>>>>>>> 2ddd7e7f7bdc32e421f23a847a01c8ab48c6f1d7
    .MinimumLevel.Information()
    .WriteTo.Console()
    .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day)
    .WriteTo.MySQL(
        connectionString: builder.Configuration.GetConnectionString("MySqlConnection"),
        tableName: "Logs"
    )
<<<<<<< HEAD
);

Log.Information("Aplicação Iniciada - Teste de Log no Banco MySQL");

// Registro do LogService
builder.Services.AddScoped<LogService>();

builder.Services.AddScoped<MySqlBackupService>(sp =>
    new MySqlBackupService(
        builder.Configuration.GetConnectionString("DefaultConnection") ?? string.Empty
    )
);

// DbContext (corrigida versão do MySQL)
builder.Services.AddDbContext<MeuDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        new MySqlServerVersion(new Version(9, 4, 0)), // versão válida
        mySqlOptions => mySqlOptions.EnableStringComparisonTranslations()
    ));

// Serviços CNPJ
builder.Services.AddHttpClient<ICnpjService<Cliente>, CnpjService<Cliente>>();
builder.Services.AddHttpClient<ICnpjService<Fornecedor>, CnpjService<Fornecedor>>();

// Registrar EntidadesService
builder.Services.AddScoped<EntidadesService>();

var app = builder.Build();

// Pipeline
=======
    .CreateLogger();
SelfLog.Enable(msg =>Console.WriteLine(msg));
Log.Information("Aplicação iniciada - teste de log no banco MySQL");

// Configura Serilog: console + arquivo
builder.Host.UseSerilog((ctx, lc) => lc
    .WriteTo.Console()
    .WriteTo.File("Logs/clientes.log", rollingInterval: RollingInterval.Day)
);

// Serviços MVC
builder.Services.AddControllersWithViews();

// Autenticação Windows via IIS/IIS Express
builder.Services.AddAuthentication(IISDefaults.AuthenticationScheme);

builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = options.DefaultPolicy;
});

// Configuração do DbContext com MySQL (banco principal)
builder.Services.AddDbContext<MeuDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        new MySqlServerVersion(new Version(8, 0, 36)) // ajuste conforme versão do MySQL
    )
);

// Filtro global para verificar banco
builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add<Ordem_Servicos_Web.Filters.VerificaBancoFilter>();
});

var app = builder.Build();

// Pipeline de requisição HTTP
>>>>>>> 2ddd7e7f7bdc32e421f23a847a01c8ab48c6f1d7
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
<<<<<<< HEAD
app.UseStaticFiles();

app.UseRouting();

app.UseSession();   // habilitar sessão
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();
=======
app.UseStaticFiles(); // necessário para servir CSS, JS e imagens

app.UseRouting();

app.UseAuthentication(); // deve vir antes de Authorization
app.UseAuthorization();

// Rotas MVC padrão
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
>>>>>>> 2ddd7e7f7bdc32e421f23a847a01c8ab48c6f1d7
