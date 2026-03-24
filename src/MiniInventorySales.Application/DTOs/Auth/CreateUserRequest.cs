using MiniInventorySales.Application.Common;
using System.ComponentModel.DataAnnotations;

namespace MiniInventorySales.Application.DTOs.Auth
{
    public class CreateUserRequest
    {
        [Required(ErrorMessage = "Full name is required")]
        [StringLength(150, MinimumLength = 3, ErrorMessage = "Full name must be between 3 and 150 characters")]
        [RegularExpression(ValidationPatterns.FullName,
            ErrorMessage = "Full name can only contain letters, spaces, apostrophes, hyphens, and periods")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Enter a valid email address")]
        [StringLength(150, ErrorMessage = "Email cannot exceed 150 characters")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Username is required")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 50 characters")]
        [RegularExpression(ValidationPatterns.Username,
            ErrorMessage = "Username must start with a letter or number and can only contain letters, numbers, dot, underscore, or hyphen")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Phone number is required")]
        [StringLength(14, MinimumLength = 11, ErrorMessage = "Enter a valid Nigerian phone number")]
        [RegularExpression(ValidationPatterns.NigeriaPhoneNumber,
            ErrorMessage = "Enter a valid Nigerian phone number")]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be between 8 and 100 characters")]
        [RegularExpression(ValidationPatterns.StrongPassword,
            ErrorMessage = "Password must contain at least one uppercase letter, one lowercase letter, one number, and one special character")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Role id is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Select a valid role")]
        public int RoleId { get; set; }
    }
}
