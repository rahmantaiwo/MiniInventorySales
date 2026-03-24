using System.ComponentModel.DataAnnotations;

namespace MiniInventorySales.Application.DTOs.Auth
{
    public class UpdateUserRoleRequest
    {
        [Required(ErrorMessage = "User id is required")]
        public Guid UserId { get; set; }

        [Required(ErrorMessage = "Role id is required")]
        public Guid RoleId { get; set; }
    }
}
