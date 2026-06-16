using Gatherly.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Gatherly.Domain.IRepositories
{
    public interface IAnnouncementRepository
    {
        Task AddAsync(Announcement announcement);
        Task<IEnumerable<Announcement>> GetByEventIdAsync(Guid eventId);
    }
}