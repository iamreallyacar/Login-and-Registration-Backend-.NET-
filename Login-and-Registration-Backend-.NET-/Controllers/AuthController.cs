using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using Login_and_Registration_Backend_.NET_.Services;
using Login_and_Registration_Backend_.NET_.Models;

namespace Login_and_Registration_Backend_.NET_.Controllers
{
    /// <summary>
    /// Authentication controller handling user registration, login, and profile management.
    /// Provides endpoints for traditional username/password authentication and OAuth integration.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;

        /// <summary>
        /// Initializes a new instance of the AuthController with dependency injection.
        /// </summary>
        /// <param name="userService">Service for user management operations</param>
        public AuthController(IUserService userService)
        {
            _userService = userService;
        }        /// <summary>
        /// Registers a new user account with username, email, and password.
        /// Validates that username and email are unique before creating the account.
        /// </summary>
        /// <param name="request">Registration request containing username, email, and password</param>
        /// <returns>Success message or validation errors</returns>
        /// <response code="200">User registered successfully</response>
        /// <response code="400">Registration failed due to validation errors or duplicate user</response>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            try
            {
                // Check if user already exists by username or email to prevent duplicates
                var existingUserByUsername = await _userService.GetUserByUsernameAsync(request.Username);
                var existingUserByEmail = await _userService.GetUserByEmailAsync(request.Email);

                if (existingUserByUsername != null || existingUserByEmail != null)
                {
                    return BadRequest(new { message = "Username or email already exists" });
                }

                // Register user using ASP.NET Core Identity with password validation
                var result = await _userService.RegisterUserAsync(request);
                
                if (result.Succeeded)
                {
                    return Ok(new { message = "User registered successfully" });
                }

                // Return validation errors from Identity (password complexity, etc.)
                return BadRequest(new { 
                    message = "Registration failed", 
                    errors = result.Errors.Select(e => e.Description) 
                });
            }            catch (Exception ex)
            {
                // Log the exception in a production environment
                return BadRequest(new { message = "Registration error: " + ex.Message });
            }
        }

        /// <summary>
        /// Authenticates a user with username and password, returning a JWT token on success.
        /// Updates the user's last login timestamp upon successful authentication.
        /// </summary>
        /// <param name="request">Login request containing username and password</param>
        /// <returns>User information and JWT token, or error message</returns>
        /// <response code="200">Login successful with user data and JWT token</response>
        /// <response code="401">Invalid username or password</response>
        /// <response code="400">Request processing error</response>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            try
            {
                // Validate user credentials using ASP.NET Core Identity
                var (success, user) = await _userService.ValidateUserAsync(request);
                
                if (!success || user == null)
                {
                    return Unauthorized(new { message = "Invalid username or password" });
                }                // Generate JWT token for the authenticated user
                var token = _userService.GenerateJwtToken(user);

                // Return user information and token for client-side storage
				return Ok(new
				{
					User = new UserDto
					{
						Id = user.Id,
						Username = user.UserName ?? string.Empty,
						Email = user.Email ?? string.Empty
					},
					Token = token
				});
            }
            catch (Exception ex)
            {
                // Log the exception in a production environment
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Simple health check endpoint to verify API availability.
        /// Can be used for monitoring and load balancer health checks.
        /// </summary>
        /// <returns>Success message indicating server is running</returns>
        /// <response code="200">Server is operational</response>
        [HttpGet("test")]
        public ActionResult<string> Test()
        {
            return Ok("Server is running successfully!");
        }        /// <summary>
        /// Retrieves the current user's profile information.
        /// Requires valid JWT authentication with user ID claim.
        /// </summary>
        /// <returns>User profile information</returns>
        /// <response code="200">User profile data</response>
        /// <response code="401">Invalid or missing JWT token</response>
        /// <response code="404">User not found in database</response>
        /// <response code="400">Request processing error</response>
        [Authorize]
        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            try
            {
                // Extract user ID from JWT token claims
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null)
                {
                    return Unauthorized(new { message = "Invalid token" });
                }

                // Retrieve user from database using ID from token
                var user = await _userService.GetUserByIdAsync(userIdClaim.Value);
                if (user == null)
                {
                    return NotFound(new { message = "User not found" });
                }

                return Ok(new
                {
                    user = new UserDto
                    {
                        Id = user.Id,
                        Username = user.UserName ?? string.Empty,
                        Email = user.Email ?? string.Empty
                    }
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("google-login")]
        public IActionResult GoogleLogin(string? returnUrl = null)
        {
            var redirectUrl = $"{Request.Scheme}://{Request.Host}/api/auth/oauth-success";
			var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        [HttpGet("oauth-success")]
        [AllowAnonymous]
        public async Task<IActionResult> OAuthSuccess()
        {
            try
            {
                var authenticateResult = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                if (!authenticateResult.Succeeded || authenticateResult.Principal == null)
                {
                    return Redirect("http://localhost:5174/oauth-success?error=OAuth%20authentication%20failed");
                }

                var email = authenticateResult.Principal.FindFirst(ClaimTypes.Email)?.Value;
                var name = authenticateResult.Principal.FindFirst(ClaimTypes.Name)?.Value;
                
                if (string.IsNullOrEmpty(email))
                {
                    return Redirect("http://localhost:5174/oauth-success?error=No%20email%20returned%20from%20Google");
                }

                // Find or create user
                var user = await _userService.GetUserByEmailAsync(email);
                if (user == null)
                {
                    var registerRequest = new RegisterRequest
                    {
                        Username = name ?? email,
                        Email = email,
                        Password = Guid.NewGuid().ToString() // Generate random password for OAuth users
                    };

                    var result = await _userService.RegisterUserAsync(registerRequest);
                    if (!result.Succeeded)
                    {
                        return Redirect("http://localhost:5174/oauth-success?error=Failed%20to%20create%20user");
                    }

                    user = await _userService.GetUserByEmailAsync(email);
                }

                if (user == null)
                {
                    return Redirect("http://localhost:5174/oauth-success?error=User%20creation%20failed");
                }

                var token = _userService.GenerateJwtToken(user);
                return Redirect($"http://localhost:5174/oauth-success?token={token}");
            }
            catch (Exception ex)
            {
                return Redirect($"http://localhost:5174/oauth-success?error={Uri.EscapeDataString(ex.Message)}");
            }
        }
    }
}