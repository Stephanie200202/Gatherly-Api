using System;
using System.Collections.Generic;
using System.Text;

namespace Gatherly.Applicaton.IServices
{
    public interface INotificationService
    {
        object GetUserNotifications(Guid userId, bool? readOnly);
        bool MarkAsRead(Guid notificationId);
        bool MarkAllAsRead(Guid userId);
    }
}
