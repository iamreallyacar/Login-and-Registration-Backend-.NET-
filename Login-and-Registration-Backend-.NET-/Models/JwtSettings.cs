using System.ComponentModel.DataAnnotations;

namespace Login_and_Registration_Backend_.NET_.Models
{
    /// <summary>
    /// Configuration settings for JWT token generation and validation
    /// </summary>
    public class JwtSettings
    {
        public const string SectionName = "Jwt";

        [Required(ErrorMessage = "JWT Key is required")]
        [MinLength(32, ErrorMessage = "JWT Key must be at least 32 characters long")]
        public string Key { get; set; } = string.Empty;

        [Required(ErrorMessage = "JWT Issuer is required")]
        public string Issuer { get; set; } = string.Empty;

        [Required(ErrorMessage = "JWT Audience is required")]
        public string Audience { get; set; } = string.Empty;

        [Range(1, 168, ErrorMessage = "ExpiryInHours must be between 1 and 168 hours (1 week)")]
        public int ExpiryInHours { get; set; } = 24;

        /// <summary>
        /// Gets the token expiry time as a TimeSpan
        /// </summary>
        public TimeSpan ExpiryTimeSpan => TimeSpan.FromHours(ExpiryInHours);
    }
}
