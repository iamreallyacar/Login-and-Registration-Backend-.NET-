using System.Security.Claims;
using Login_and_Registration_Backend_.NET_.Models;

namespace Login_and_Registration_Backend_.NET_.Services
{
    /// <summary>
    /// Interface for JWT token generation and validation services
    /// </summary>
    public interface IJwtService
    {
        /// <summary>
        /// Generates a JWT token for the specified user
        /// </summary>
        /// <param name="user">The user to generate the token for</param>
        /// <returns>JWT token string</returns>
        string GenerateJwtToken(ApplicationUser user);

        /// <summary>
        /// Validates a JWT token and returns the claims principal
        /// </summary>
        /// <param name="token">The JWT token to validate</param>
        /// <returns>Claims principal if valid, null otherwise</returns>
        ClaimsPrincipal? ValidateToken(string token);

        /// <summary>
        /// Gets the expiration time for a JWT token
        /// </summary>
        /// <returns>Token expiration date and time</returns>
        DateTime GetTokenExpiration();
    }
}