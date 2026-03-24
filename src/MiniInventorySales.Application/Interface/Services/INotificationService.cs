using MiniInventorySales.Application.DTOs.Notification;

namespace MiniInventorySales.Application.Interface.Services
{
    public interface INotificationService
    {
        Task CreateAsync(int userId, string title, string message);

        Task<List<NotificationDto>> GetUserNotificationsAsync(int userId);

        Task MarkAsReadAsync(int notificationId);

        Task MarkAllAsReadAsync(int userId);
    }
}
