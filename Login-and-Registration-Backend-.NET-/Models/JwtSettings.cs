using System.ComponentModel.DataAnnotations;

namespace Login_and_Registration_Backend_.NET_.Models
{
    public class JwtSettings
    {
        [Required]
        [MinLength(32, ErrorMessage = "JWT Key must be at least 32 characters long")]
        public string Key { get; set; } = string.Empty;

        [Required]
        public string Issuer { get; set; } = string.Empty;

        [Required]
        public string Audience { get; set; } = string.Empty;

        [Range(1, 365, ErrorMessage = "ExpiryDays must be between 1 and 365")]
        public int ExpiryDays { get; set; } = 7;
    }
}
