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
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;

        public AuthController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            try
            {
                // Check if user already exists
                var existingUserByUsername = await _userService.GetUserByUsernameAsync(request.Username);
                var existingUserByEmail = await _userService.GetUserByEmailAsync(request.Email);

                if (existingUserByUsername != null || existingUserByEmail != null)
                {
                    return BadRequest(new { message = "Username or email already exists" });
                }

                // Register user using Identity
                var result = await _userService.RegisterUserAsync(request);
                
                if (result.Succeeded)
                {
                    return Ok(new { message = "User registered successfully" });
                }

                return BadRequest(new { 
                    message = "Registration failed", 
                    errors = result.Errors.Select(e => e.Description) 
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Registration error: " + ex.Message });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            try
            {
                var (success, user) = await _userService.ValidateUserAsync(request);
                
                if (!success || user == null)
                {
                    return Unauthorized(new { message = "Invalid username or password" });
                }

                var token = _userService.GenerateJwtToken(user);

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
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("test")]
        public ActionResult<string> Test()
        {
            return Ok("Server is running successfully!");
        }

        [Authorize]
        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null)
                {
                    return Unauthorized(new { message = "Invalid token" });
                }

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
            var redirectUrl = "http://localhost:5001/api/auth/oauth-success";
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