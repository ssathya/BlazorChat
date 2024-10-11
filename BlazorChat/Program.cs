using BlazorChat.Components;
using BlazorChat.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.SemanticKernel;
using Models.DBModels;
using Serilog;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var config = builder.Configuration;
//Logging
StringBuilder filePath = new();
filePath.Append(Path.GetTempPath() + "/");
filePath.Append("BlazorChat-.log");
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(config)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File(filePath.ToString(), rollingInterval: RollingInterval.Day, retainedFileCountLimit: 3)
    .CreateLogger();
builder.Services.AddLogging(c =>
{
    c.SetMinimumLevel(LogLevel.Debug);
    c.AddSerilog(Log.Logger);
});
//Caching
builder.Services.AddOutputCache(cfg =>
{
    cfg.AddBasePolicy(bldr =>
    {
        bldr.With(r => r.HttpContext.Request.Path.StartsWithSegments("/"));
        bldr.Expire(TimeSpan.FromMinutes(5));
    });
    cfg.AddPolicy("ShortCache", bldr =>
    {
        bldr.Expire(TimeSpan.FromSeconds(25));
    });
});
Log.Logger.Information("Application Starting");
//Connect to service
builder.Services.AddKernel();
try
{
    builder.Services.AddAzureOpenAIChatCompletion(
        deploymentName: config["AzureAi:DEPLOYMENT_MODEL"] ?? ""
        , endpoint: config["AzureAi:AZURE_OPEN_AI_ENDPOINT"] ?? ""
        , apiKey: config["AzureAi:AZURE_OPEN_AI_KEY"] ?? ""
        , modelId: config["AzureAi:ModelId"]);
}
catch (ArgumentException ex)
{
    Console.WriteLine(ex.Message);
    throw;
}
//Connect to Database
string connectionString = config["ConnectionString:DefaultConnection"] ?? "";
builder.Services.AddDbContextFactory<BlazorChatContext>(options =>
{
    options.UseNpgsql(connectionString);
});
SetupDependency(builder);
builder.Services.AddSingleton<IFunctionInvocationFilter>(new RetryFilter(config["AzureAi:ModelId"]!));
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();

static void SetupDependency(WebApplicationBuilder builder)
{
    builder.Services.AddScoped(sp => KernelPluginFactory.CreateFromType<NewsPlugin>(serviceProvider: sp));
    builder.Services.AddScoped(sp => KernelPluginFactory.CreateFromType<ArchivePlugin>(serviceProvider: sp));
    builder.Services.AddScoped(sp => KernelPluginFactory.CreateFromType<WeatherPlugin>(serviceProvider: sp));
    builder.Services.AddScoped(sp => KernelPluginFactory.CreateFromType<StockServicePlugin>(serviceProvider: sp));
    builder.Services.AddScoped(sp => KernelPluginFactory.CreateFromType<CurrencyService>(serviceProvider: sp));
    builder.Services.AddHttpClient();
}