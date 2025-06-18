using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Login_and_Registration_Backend_.NET_.Models;
using Login_and_Registration_Backend_.NET_.Data;
using Microsoft.Extensions.Logging;

namespace Login_and_Registration_Backend_.NET_.Services
{
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

		public string GenerateJwtToken(ApplicationUser user)
		{
			return _jwtService.GenerateJwtToken(user);
		}

		public async Task<ApplicationUser?> GetUserByIdAsync(string id)
		{
			return await _userManager.FindByIdAsync(id);
		}

		public async Task<ApplicationUser?> GetUserByEmailAsync(string email)
		{
			return await _userManager.FindByEmailAsync(email);
		}

		public async Task<ApplicationUser?> GetUserByUsernameAsync(string username)
		{
			return await _userManager.FindByNameAsync(username);
		}

		public async Task<IdentityResult> RegisterUserAsync(RegisterRequest request)
		{
			var user = new ApplicationUser
			{
				UserName = request.Username,
				Email = request.Email,
				CreatedAt = DateTime.UtcNow
			};

			return await _userManager.CreateAsync(user, request.Password);
		}
		public async Task<(bool Success, ApplicationUser? User)> ValidateUserAsync(LoginRequest request)
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

		public string HashPassword(ApplicationUser user, string password)
		{
			return _userManager.PasswordHasher.HashPassword(user, password);
		}

		public bool VerifyPassword(ApplicationUser user, string providedPassword, string hashedPassword)
		{
			var result = _userManager.PasswordHasher.VerifyHashedPassword(user, hashedPassword, providedPassword);
			return result == PasswordVerificationResult.Success;
		}
    }
}