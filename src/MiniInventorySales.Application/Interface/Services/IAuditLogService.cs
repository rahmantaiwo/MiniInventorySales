namespace MiniInventorySales.Application.Interface.Services
{
    public interface IAuditLogService
    {
        Task LogAsync(
            Guid? userId,
            string action,
            string entityName,
            string? entityId = null,
            string? oldValues = null,
            string? newValues = null,
            string? details = null,
            string? ipAddress = null,
            CancellationToken ct = default);
    }
}
