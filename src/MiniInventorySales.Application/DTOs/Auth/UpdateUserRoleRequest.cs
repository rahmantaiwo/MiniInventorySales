using System.ComponentModel.DataAnnotations;

namespace MiniInventorySales.Application.DTOs.Auth
{
    public class UpdateUserRoleRequest
    {
        [Required(ErrorMessage = "User id is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Invalid user id")]
        public int UserId { get; set; }

        [Required(ErrorMessage = "Role id is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Select a valid role")]
        public int RoleId { get; set; }
    }
}
