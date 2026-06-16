using Gatherly.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Gatherly.Applicaton.IServices
{
    public interface IAnnouncementService
    {
        object Broadcast(Guid eventId, string title, string message, string priority);
        IEnumerable<Announcement> GetAnnouncements(Guid eventId);
    }
}
