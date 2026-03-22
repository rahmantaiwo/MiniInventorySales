using MiniInventorySales.Domain.Enums;

namespace MiniInventorySales.Domain.Entities
{
    public class AppUser : BaseEntity
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public bool IsActive { get; set; } = true;
        public UserRole Role { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<Sale> Sales { get; set; } = new List<Sale>();
    }
}
