using Microsoft.EntityFrameworkCore;
using Ordem_Servicos_Web.Binders;
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

// Model Binder para SmartDecimal
builder.Services.AddControllers(options =>
{
    // Binder para valores monetários e quantidades
    options.ModelBinderProviders.Insert(0, new SmartDecimalBinderProvider());

    // Binder para datas em múltiplos formatos
    options.ModelBinderProviders.Insert(0, new DateBinderProvider());
});


// MVC + filtros globais
builder.Services.AddControllersWithViews(options =>
{
    options.Filters.AddService<VerificaBancoFilter>();
});

// Serilog (configuração única)
builder.Host.UseSerilog((ctx, lc) => lc
    .MinimumLevel.Information()
    .WriteTo.Console()
    .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day)
    .WriteTo.MySQL(
        connectionString: builder.Configuration.GetConnectionString("MySqlConnection"),
        tableName: "Logs"
    )
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
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();   // habilitar sessão
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();