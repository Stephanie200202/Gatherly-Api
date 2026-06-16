using Gatherly.Domain.Entities;

public interface INotificationRepository
{
    IEnumerable<Notification> GetByUserId(Guid userId);
    Notification? GetById(Guid notificationId);
    void Add(Notification notification);
    void Update(Notification notification);
    void MarkAllAsRead(Guid userId);
}