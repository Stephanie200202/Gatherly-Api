using System;
using System.Collections.Generic;
using System.Linq;
using Gatherly.Applicaton.IServices;
using Gatherly.Domain.Entities;
using Gatherly.Domain.Interfaces; // Adjust to your actual repository namespace

namespace Gatherly.Application.Services;

public class NotificationService : INotificationService
{
    private readonly INotificationRepository _notificationRepository;

    public NotificationService(INotificationRepository notificationRepository)
    {
        _notificationRepository = notificationRepository;
    }

    public object GetUserNotifications(Guid userId, bool? readOnly)
    {
        // 1. Fetch all raw notifications for the user from the repository
        var notifications = _notificationRepository.GetByUserId(userId) ?? Enumerable.Empty<Notification>();

        // 2. Apply optional filtering based on the 'readOnly' parameter
        if (readOnly.HasValue)
        {
            notifications = notifications.Where(n => n.IsRead == readOnly.Value);
        }

        var notificationList = notifications.ToList();

        // 3. Return a clean object containing the list and metadata (like unread count)
        return new
        {
            totalCount = notificationList.Count,
            unreadCount = notificationList.Count(n => !n.IsRead),
            notifications = notificationList.Select(n => new
            {
                n.NotificationId, // Ensure this property name matches your Entity
                n.Title,
                n.Message,
                n.IsRead,
                n.CreatedAt
            })
        };
    }

    public bool MarkAsRead(Guid notificationId)
    {
        var notification = _notificationRepository.GetById(notificationId);
        if (notification == null)
        {
            return false;
        }

        notification.IsRead = true;
        _notificationRepository.Update(notification);
        return true;
    }

    public bool MarkAllAsRead(Guid userId)
    {
        // Aligned with the interface's boolean return requirement
        try
        {
            _notificationRepository.MarkAllAsRead(userId);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }
}