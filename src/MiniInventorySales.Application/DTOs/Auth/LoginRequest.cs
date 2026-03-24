using System.ComponentModel.DataAnnotations;

namespace MiniInventorySales.Application.DTOs.Auth
{
    public class LoginRequest
    {
        [Required(ErrorMessage = "Username or email is required")]
        [StringLength(150, ErrorMessage = "Username or email cannot exceed 150 characters")]
        public string EmailOrUsername { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be between 8 and 100 characters")]
        public string Password { get; set; } = string.Empty;
    }
}
