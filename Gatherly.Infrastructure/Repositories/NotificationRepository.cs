using System;
using System.Collections.Generic;
using System.Linq;
using Gatherly.Domain.Entities;
using Gatherly.Domain.Interfaces; // Where your INotificationRepository interface lives
using Gatherly.Infrastructure.Data; // Where your ApplicationDbContext lives

namespace Gatherly.Infrastructure.Repositories;

public class NotificationRepository : INotificationRepository
{
    private readonly ApplicationDbContext _context;

    public NotificationRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public void Add(Notification notification)
    {
        _context.Notifications.Add(notification);
        _context.SaveChanges();
    }

    public Notification? GetById(Guid notificationId)
    {
        // Finds a single notification matching the provided primary key
        return _context.Notifications
            .FirstOrDefault(n => n.NotificationId == notificationId);
    }

    public IEnumerable<Notification> GetByUserId(Guid userId)
    {
        // Pulls all notifications belonging to a specific user
        return _context.Notifications
            .Where(n => n.UserId == userId)
            .ToList();
    }

    public void Update(Notification notification)
    {
        _context.Notifications.Update(notification);
        _context.SaveChanges();
    }

    public void MarkAllAsRead(Guid userId)
    {
        // 1. Fetch all unread notifications for this specific user
        var unreadNotifications = _context.Notifications
            .Where(n => n.UserId == userId && !n.IsRead)
            .ToList();

        if (unreadNotifications.Any())
        {
            // 2. Loop through and change the state tracking flag locally
            foreach (var notification in unreadNotifications)
            {
                notification.IsRead = true;
            }

            // 3. Commit the block updates to SQL Server in a single round-trip
            _context.SaveChanges();
        }
    }
}