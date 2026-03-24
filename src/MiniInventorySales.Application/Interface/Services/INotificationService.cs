using MiniInventorySales.Application.DTOs.Notification;

namespace MiniInventorySales.Application.Interface.Services
{
    public interface INotificationService
    {
        Task CreateAsync(Guid userId, string title, string message);

        Task<List<NotificationDto>> GetUserNotificationsAsync(Guid userId);

        Task MarkAsReadAsync(Guid notificationId);

        Task MarkAllAsReadAsync(Guid userId);
    }
}
