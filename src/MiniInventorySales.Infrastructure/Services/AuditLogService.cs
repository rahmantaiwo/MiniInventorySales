using Microsoft.Extensions.Logging;
using MiniInventorySales.Application.Interface.Services;
using MiniInventorySales.Domain.Entities;
using MiniInventorySales.Infrastructure.Persistence;

namespace MiniInventorySales.Infrastructure.Services
{
    public class AuditLogService : IAuditLogService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<AuditLogService> _logger;

        public AuditLogService(AppDbContext context, ILogger<AuditLogService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task LogAsync(
            Guid? userId,
            string action,
            string entityName,
            string? entityId = null,
            string? oldValues = null,
            string? newValues = null,
            string? details = null,
            string? ipAddress = null,
            CancellationToken ct = default)
        {
            try
            {
                var auditLog = new AuditLog
                {
                    UserId = userId,
                    Action = action,
                    EntityName = entityName,
                    EntityId = entityId,
                    OldValues = oldValues,
                    NewValues = newValues,
                    Details = details,
                    IpAddress = ipAddress
                };

                _context.AuditLogs.Add(auditLog);
                await _context.SaveChangesAsync(ct);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to write audit log for {Action} on {EntityName}", action, entityName);
            }
        }
    }
}
