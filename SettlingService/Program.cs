using NLog;
using NLog.Extensions.Hosting;
using SettlingEngine;
using SettlingEngine.Constants;
using System.Runtime.InteropServices;

var builder = WebApplication.CreateBuilder(args);

// Determine base folder where the DLL is located
var baseDir = AppContext.BaseDirectory;

// Build absolute paths based on the running directory
var configAppPath = Path.Combine(baseDir, "Config", "appsettings.json");
var configLogPath = Path.Combine(baseDir, "Config", "nlog.config");


if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
{
    configAppPath = Path.GetFullPath(Path.Combine(baseDir, "..", "ConfigApp", "appsettings.json"));
    configLogPath = Path.GetFullPath(Path.Combine(baseDir, "..", "ConfigLog", "nlog.config"));
}

builder.Configuration.AddJsonFile(configAppPath, optional: false, reloadOnChange: true);
builder.Configuration.AddEnvironmentVariables();

// Init NLog
LogManager.Setup().LoadConfigurationFromFile(configLogPath);
builder.Host.UseNLog();

Console.WriteLine($"configLogPath: {configLogPath}");

ConfigData.Configure(builder.Configuration);

if (ConfigData.ServerSettingConfig.UseHttps)
{
    builder.WebHost.UseUrls($"https://*:{ConfigData.ServerSettingConfig.Port}");
}
else
{
    builder.WebHost.UseUrls($"http://*:{ConfigData.ServerSettingConfig.Port}");
}

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader();
        });
});

SettlingServiceStartup.RegisterServices(builder.Services, builder.Configuration);

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
// Static files với cache control cho dev
var staticFileOptions = new StaticFileOptions();
// if (app.Environment.IsDevelopment())
// {
// Disable cache cho dev để luôn load file mới
staticFileOptions.OnPrepareResponse = ctx =>
{
    ctx.Context.Response.Headers.Append("Cache-Control", "no-cache, no-store, must-revalidate");
    ctx.Context.Response.Headers.Append("Pragma", "no-cache");
    ctx.Context.Response.Headers.Append("Expires", "0");
};
// }
app.UseStaticFiles(staticFileOptions);

app.UseRouting();

app.UseCors("AllowAll");

app.UseAuthorization();

// Map controller routes
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Catch-all route để hỗ trợ SPA routing (React Router)
// Tất cả các routes không match sẽ về Home/Index, sau đó React Router sẽ xử lý
app.MapFallbackToController("Index", "Home");

app.Run();
