using Microsoft.AspNetCore.Identity;
using Login_and_Registration_Backend_.NET_.Models;
using Login_and_Registration_Backend_.NET_.Models.DTOs;

namespace Login_and_Registration_Backend_.NET_.Services
{
    /// <summary>
    /// Interface for user management services
    /// </summary>
    public interface IUserService
    {
        /// <summary>
        /// Generates a JWT token for the specified user
        /// </summary>
        /// <param name="user">The user to generate the token for</param>
        /// <returns>JWT token string</returns>
        string GenerateJwtToken(ApplicationUser user);

        /// <summary>
        /// Gets the JWT token expiration time
        /// </summary>
        /// <returns>Token expiration date and time</returns>
        DateTime GetTokenExpiration();

        /// <summary>
        /// Retrieves a user by their ID
        /// </summary>
        /// <param name="id">The user ID</param>
        /// <returns>The user if found, null otherwise</returns>
        Task<ApplicationUser?> GetUserByIdAsync(string id);

        /// <summary>
        /// Retrieves a user by their email address
        /// </summary>
        /// <param name="email">The user's email address</param>
        /// <returns>The user if found, null otherwise</returns>
        Task<ApplicationUser?> GetUserByEmailAsync(string email);

        /// <summary>
        /// Retrieves a user by their username
        /// </summary>
        /// <param name="username">The username</param>
        /// <returns>The user if found, null otherwise</returns>
        Task<ApplicationUser?> GetUserByUsernameAsync(string username);        /// <summary>
        /// Registers a new user with the provided information
        /// </summary>
        /// <param name="request">Registration request containing user details</param>
        /// <returns>Identity result indicating success or failure</returns>
        Task<IdentityResult> RegisterUserAsync(RegisterRequestDto request);

        /// <summary>
        /// Validates user credentials and returns authentication result
        /// </summary>
        /// <param name="request">Login request containing username and password</param>
        /// <returns>Tuple indicating success and the user object if successful</returns>
        Task<(bool Success, ApplicationUser? User)> ValidateUserAsync(LoginRequestDto request);

        /// <summary>
        /// Hashes a password for a user
        /// </summary>
        /// <param name="user">The user</param>
        /// <param name="password">The plain text password</param>
        /// <returns>Hashed password</returns>
        string HashPassword(ApplicationUser user, string password);

        /// <summary>
        /// Verifies a password against a hashed password
        /// </summary>
        /// <param name="user">The user</param>
        /// <param name="providedPassword">The provided password</param>
        /// <param name="hashedPassword">The stored hashed password</param>
        /// <returns>True if password is valid, false otherwise</returns>
        bool VerifyPassword(ApplicationUser user, string providedPassword, string hashedPassword);
    }
}