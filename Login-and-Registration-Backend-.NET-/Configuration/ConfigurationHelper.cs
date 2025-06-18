namespace Login_and_Registration_Backend_.NET_.Configuration
{
    /// <summary>
    /// Configuration helper class for organizing application settings
    /// </summary>
    public static class ConfigurationHelper
    {
        /// <summary>
        /// Validates required configuration settings on startup
        /// </summary>
        /// <param name="configuration">The application configuration</param>
        /// <exception cref="InvalidOperationException">Thrown when required settings are missing</exception>
        public static void ValidateConfiguration(IConfiguration configuration)
        {
            var requiredSettings = new Dictionary<string, string>
            {
                ["Jwt:Key"] = "JWT signing key",
                ["Jwt:Issuer"] = "JWT issuer",
                ["Jwt:Audience"] = "JWT audience",
                ["ConnectionStrings:DefaultConnection"] = "Database connection string"
            };

            var missingSettings = new List<string>();

            foreach (var setting in requiredSettings)
            {
                var value = configuration[setting.Key];
                if (string.IsNullOrEmpty(value))
                {
                    missingSettings.Add($"{setting.Key} ({setting.Value})");
                }
            }

            if (missingSettings.Any())
            {
                throw new InvalidOperationException(
                    $"Missing required configuration settings: {string.Join(", ", missingSettings)}");
            }

            // Validate JWT key length
            var jwtKey = configuration["Jwt:Key"];
            if (jwtKey != null && jwtKey.Length < 32)
            {
                throw new InvalidOperationException("JWT Key must be at least 32 characters long for security");
            }
        }

        /// <summary>
        /// Gets OAuth provider configuration status
        /// </summary>
        /// <param name="configuration">The application configuration</param>
        /// <returns>Dictionary of provider names and their configuration status</returns>
        public static Dictionary<string, bool> GetOAuthProviderStatus(IConfiguration configuration)
        {
            return new Dictionary<string, bool>
            {
                ["Google"] = !string.IsNullOrEmpty(configuration["Authentication:Google:ClientId"]) &&
                           !string.IsNullOrEmpty(configuration["Authentication:Google:ClientSecret"]),
                
                ["Microsoft"] = !string.IsNullOrEmpty(configuration["Authentication:Microsoft:ClientId"]) &&
                              !string.IsNullOrEmpty(configuration["Authentication:Microsoft:ClientSecret"])
            };
        }

        /// <summary>
        /// Gets the frontend URL based on environment
        /// </summary>
        /// <param name="configuration">The application configuration</param>
        /// <param name="environment">The hosting environment</param>
        /// <returns>The frontend URL</returns>
        public static string GetFrontendUrl(IConfiguration configuration, IWebHostEnvironment environment)
        {
            if (environment.IsDevelopment())
            {
                return configuration["Frontend:DevelopmentUrl"] ?? "http://localhost:5174";
            }
            
            return configuration["Frontend:ProductionUrl"] ?? "https://yourdomain.com";
        }
    }
}
