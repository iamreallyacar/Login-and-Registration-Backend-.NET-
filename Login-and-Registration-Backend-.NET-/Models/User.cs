using System.ComponentModel.DataAnnotations;

namespace Login_and_Registration_Backend_.NET_.Models
{
	public class UserDto
	{
		public string Id { get; set; } = string.Empty;
		public string Username { get; set; } = string.Empty;
		public string Email { get; set; } = string.Empty;
	}

	public class RegisterRequest
	{
		[Required(ErrorMessage = "Username is required")]
		[StringLength(50, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 50 characters")]
		[RegularExpression(@"^[a-zA-Z0-9_]+$", ErrorMessage = "Username can only contain letters, numbers, and underscores")]
		public required string Username { get; set; }

		[Required(ErrorMessage = "Email is required")]
		[EmailAddress(ErrorMessage = "Invalid email format")]
		[StringLength(100, ErrorMessage = "Email cannot exceed 100 characters")]
		public required string Email { get; set; }

		[Required(ErrorMessage = "Password is required")]
		[StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters long")]
		public required string Password { get; set; }
	}
	
	public class LoginRequest
	{
		[Required(ErrorMessage = "Username is required")]
		[StringLength(50, ErrorMessage = "Username cannot exceed 50 characters")]
		public required string Username { get; set; }

		[Required(ErrorMessage = "Password is required")]
		[StringLength(100, ErrorMessage = "Password cannot exceed 100 characters")]
		public required string Password { get; set; }
	}
}