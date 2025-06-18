using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Login_and_Registration_Backend_.NET_.Models;
using Login_and_Registration_Backend_.NET_.Models.DTOs;
using Login_and_Registration_Backend_.NET_.Data;
using Microsoft.Extensions.Logging;

namespace Login_and_Registration_Backend_.NET_.Services
{
	/// <summary>
	/// Service for user management operations including registration, authentication, and profile management
	/// </summary>
	public class UserService : IUserService
	{
		private readonly UserManager<ApplicationUser> _userManager;
    	private readonly SignInManager<ApplicationUser> _signInManager;
		private readonly IJwtService _jwtService;
		private readonly ApplicationDbContext _dbContext;
		private readonly ILogger<UserService> _logger;

		public UserService(
			UserManager<ApplicationUser> userManager,
			SignInManager<ApplicationUser> signInManager,
			IJwtService jwtService,
			ApplicationDbContext dbContext,
			ILogger<UserService> logger)
		{
			_userManager = userManager;
			_signInManager = signInManager;
			_jwtService = jwtService;
			_dbContext = dbContext;
			_logger = logger;
		}

		/// <summary>
		/// Generates a JWT token for the specified user
		/// </summary>
		/// <param name="user">The user to generate the token for</param>
		/// <returns>JWT token string</returns>
		public string GenerateJwtToken(ApplicationUser user)
		{
			return _jwtService.GenerateJwtToken(user);
		}

		/// <summary>
		/// Gets the JWT token expiration time
		/// </summary>
		/// <returns>Token expiration date and time</returns>
		public DateTime GetTokenExpiration()
		{
			return _jwtService.GetTokenExpiration();
		}

		/// <summary>
		/// Retrieves a user by their ID
		/// </summary>
		/// <param name="id">The user ID</param>
		/// <returns>The user if found, null otherwise</returns>
		public async Task<ApplicationUser?> GetUserByIdAsync(string id)
		{
			return await _userManager.FindByIdAsync(id);
		}

		/// <summary>
		/// Retrieves a user by their email address
		/// </summary>
		/// <param name="email">The user's email address</param>
		/// <returns>The user if found, null otherwise</returns>
		public async Task<ApplicationUser?> GetUserByEmailAsync(string email)
		{
			return await _userManager.FindByEmailAsync(email);
		}

		/// <summary>
		/// Retrieves a user by their username
		/// </summary>
		/// <param name="username">The username</param>
		/// <returns>The user if found, null otherwise</returns>
		public async Task<ApplicationUser?> GetUserByUsernameAsync(string username)
		{
			return await _userManager.FindByNameAsync(username);
		}		/// <summary>
		/// Registers a new user with the provided information
		/// </summary>
		/// <param name="request">Registration request containing user details</param>
		/// <returns>Identity result indicating success or failure</returns>
		public async Task<IdentityResult> RegisterUserAsync(RegisterRequestDto request)
		{
			var user = new ApplicationUser
			{
				UserName = request.Username,
				Email = request.Email,
				CreatedAt = DateTime.UtcNow
			};

			var result = await _userManager.CreateAsync(user, request.Password);
			
			if (result.Succeeded)
			{
				_logger.LogInformation("User {Username} registered successfully", request.Username);
			}
			else
			{
				_logger.LogWarning("Failed to register user {Username}: {Errors}", 
					request.Username, string.Join(", ", result.Errors.Select(e => e.Description)));
			}

			return result;
		}

		/// <summary>
		/// Validates user credentials and returns authentication result
		/// </summary>
		/// <param name="request">Login request containing username and password</param>
		/// <returns>Tuple indicating success and the user object if successful</returns>
		public async Task<(bool Success, ApplicationUser? User)> ValidateUserAsync(LoginRequestDto request)
		{
			try
			{
				var user = await _userManager.FindByNameAsync(request.Username);
				if (user == null)
				{
					_logger.LogWarning("Login attempt with non-existent username: {Username}", request.Username);
					return (false, null);
				}

				var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);
				if (result.Succeeded)
				{
					user.LastLoginAt = DateTime.UtcNow;
					await _userManager.UpdateAsync(user);
					_logger.LogInformation("Successful login for user: {Username}", request.Username);
					return (true, user);
				}

				if (result.IsLockedOut)
				{
					_logger.LogWarning("Login attempt for locked out user: {Username}", request.Username);
				}
				else
				{
					_logger.LogWarning("Failed login attempt for user: {Username}", request.Username);
				}

				return (false, null);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error during user validation for username: {Username}", request.Username);
				return (false, null);
			}
		}

		/// <summary>
		/// Hashes a password for a user
		/// </summary>
		/// <param name="user">The user</param>
		/// <param name="password">The plain text password</param>
		/// <returns>Hashed password</returns>
		public string HashPassword(ApplicationUser user, string password)
		{
			return _userManager.PasswordHasher.HashPassword(user, password);
		}

		/// <summary>
		/// Verifies a password against a hashed password
		/// </summary>
		/// <param name="user">The user</param>
		/// <param name="providedPassword">The provided password</param>
		/// <param name="hashedPassword">The stored hashed password</param>
		/// <returns>True if password is valid, false otherwise</returns>
		public bool VerifyPassword(ApplicationUser user, string providedPassword, string hashedPassword)
		{
			var result = _userManager.PasswordHasher.VerifyHashedPassword(user, hashedPassword, providedPassword);
			return result == PasswordVerificationResult.Success;
		}
    }
}