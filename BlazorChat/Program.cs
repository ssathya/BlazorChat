using BlazorChat.Components;
using BlazorChat.Services;
using Microsoft.SemanticKernel;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var config = builder.Configuration;
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
SetupDependency(builder);
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