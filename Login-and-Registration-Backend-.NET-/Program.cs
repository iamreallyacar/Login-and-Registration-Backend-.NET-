using Login_and_Registration_Backend_.NET_.Configuration;
using Login_and_Registration_Backend_.NET_.Data;
using Login_and_Registration_Backend_.NET_.Extensions;
using Login_and_Registration_Backend_.NET_.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Configure server URLs
builder.WebHost.UseUrls("http://localhost:5000", "https://localhost:7001");

// Validate configuration on startup
ConfigurationHelper.ValidateConfiguration(builder.Configuration);

// Add services using extension methods for better organization
builder.Services.AddDatabase(builder.Configuration);
builder.Services.AddIdentityServices();
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddOAuthProviders(builder.Configuration);
builder.Services.AddCorsPolicy();
builder.Services.AddApplicationServices();

// Add API services
builder.Services.AddControllers();
builder.Services.AddOpenApi();

// Add health checks
builder.Services.AddHealthChecks();

var app = builder.Build();

// Ensure database is created and up to date
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    var startupLogger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    
    try
    {
        context.Database.EnsureCreated();
        startupLogger.LogInformation("Database initialized successfully");
        
        // Log OAuth provider status
        var oauthStatus = ConfigurationHelper.GetOAuthProviderStatus(builder.Configuration);
        foreach (var provider in oauthStatus)
        {
            startupLogger.LogInformation("OAuth Provider {Provider}: {Status}", 
                provider.Key, provider.Value ? "Configured" : "Not Configured");
        }
    }
    catch (Exception ex)
    {
        startupLogger.LogError(ex, "An error occurred while initializing the database");
        throw;
    }
}

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseDeveloperExceptionPage();
}
else
{
    // Use global exception handling in production
    app.UseGlobalExceptionHandling();
}

// Middleware pipeline
app.UseHttpsRedirection();
app.UseCors("AllowFrontend");
app.UseAuthentication();
app.UseAuthorization();

// Add health check endpoint
app.MapHealthChecks("/health");

// Map controllers
app.MapControllers();

// Log startup information
var logger = app.Services.GetRequiredService<ILogger<Program>>();
logger.LogInformation("Application started successfully");
logger.LogInformation("Environment: {Environment}", app.Environment.EnvironmentName);
logger.LogInformation("Frontend URL: {FrontendUrl}", 
    ConfigurationHelper.GetFrontendUrl(builder.Configuration, app.Environment));

app.Run();
