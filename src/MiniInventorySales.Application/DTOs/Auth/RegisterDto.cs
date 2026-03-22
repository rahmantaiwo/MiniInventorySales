using System.ComponentModel.DataAnnotations;

namespace MiniInventorySales.Application.DTOs.Auth
{
    public class RegisterDto
    {
        [Required, MaxLength(150)]
        public string FirstName { get; set; } = "";

        [Required, MaxLength(150)]
        public string LastName { get; set; } = "";

        [Required, EmailAddress, MaxLength(200)]
        public string Email { get; set; } = "";

        [Required, MinLength(6)]
        public string Password { get; set; } = "";

        [Required, MinLength(6)]
        public string ConfirmPassword { get; set; } = "";
    }
}
