using MiniInventorySales.Domain.Enums;

namespace MiniInventorySales.Domain.Entities
{
    public class AppUser : BaseEntity
    {
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public int RoleId { get; set; }
        public Role? Role { get; set; }
        public string? PhoneNumber { get; set; }
        public bool IsActive { get; set; } 
        public DateTime? LastLoginAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public ICollection<Product> CreatedProducts { get; set; } = new List<Product>();
        public ICollection<Product> UpdatedProducts { get; set; } = new List<Product>();
        public ICollection<Sale> Sales { get; set; } = new List<Sale>();
    }
}
