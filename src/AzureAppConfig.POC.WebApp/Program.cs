using Azure.Identity;
using AzureAppConfig.POC.WebApp.FeatureManagement.Filters;
using AzureAppConfig.POC.WebApp.Models;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;
using Microsoft.FeatureManagement;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApp(builder.Configuration.GetSection("AzureAd"));

builder.Services.AddAuthorization(options =>
{
    // By default, all incoming requests will be authorized according to the default policy.
    options.FallbackPolicy = options.DefaultPolicy;
});
builder.Services.AddRazorPages()
    .AddMicrosoftIdentityUI();

var appConfigConnectionString = builder.Configuration.GetConnectionString("AppConfig");
var appConfigSnapshot = builder.Configuration.GetValue<string>("AppConfigSnapshot");

builder.Configuration.AddAzureAppConfiguration(options =>
{
    // Specify the connection string to the app configuration endpoint.
    // Connection string is attainable under the Azure Portal.
    options.Connect(appConfigConnectionString);
    
    // Filter the configuration you wish to load from Azure. Wildcards/sub-namespaces
    // can be used here to narrow down the configuration required for the app.
    options.Select("POCApp:*");

    // OPTIONAL: A sentinel key can be used to signal configuration changes to the
    // consuming app. When the sentinel key value changes, all configuration loaded
    // will be refreshed following a cache expiration period (default 30 seconds).
    options.ConfigureRefresh(refreshOptions =>
    {
        refreshOptions.Register("POCApp:SentinelKey", LabelFilter.Null, refreshAll: true);

        refreshOptions.SetCacheExpiration(TimeSpan.FromSeconds(5));
    });

    if (!string.IsNullOrWhiteSpace(appConfigSnapshot))
    {
        options.SelectSnapshot(appConfigSnapshot);
    }

    // Authenticate to Azure Key Vault instance using identity provided by Visual Studio.
    // Will need to use a different credential for test/production!
    options.ConfigureKeyVault(kv => kv.SetCredential(new DefaultAzureCredential()));

    options.UseFeatureFlags(featureFlagOptions =>
    {
        featureFlagOptions.Select(KeyFilter.Any , "dev");
        featureFlagOptions.CacheExpirationInterval = TimeSpan.FromSeconds(3);
    });
});

// Load all Setting configuration under Azure App Config, to a Settings model.
builder.Services.Configure<ConfigSettings>(builder.Configuration.GetSection("POCApp:ConfigSettings"));

// Include middleware dependencies for Azure App configuration
// Only required if using dynamic configuration/refresh.
builder.Services.AddAzureAppConfiguration();

builder.Services.AddFeatureManagement()
    .AddFeatureFilter<DiceRollFilter>()
    .WithTargeting<CurrentUserTargetingContextAccessor>();

builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

// Register middleware dependencies for Azure App Configuration.
// Only required if using dynamic configuration/refresh.
// IMPORTANT: Define this as early as possible, as to ensure the middleware
// is executed as soon as possible, and/or is not skipped by other middleware.
app.UseAzureAppConfiguration();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
