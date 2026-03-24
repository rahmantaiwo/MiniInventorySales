namespace MiniInventorySales.Domain.Entities
{
    public class Role : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public ICollection<AppUser> Users { get; set; } = new List<AppUser>();
    }
}
