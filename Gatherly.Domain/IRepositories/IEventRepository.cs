using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Gatherly.Domain.Entities; // Structural representation of core components

namespace Gatherly.Domain.Interfaces
{
    public interface IEventRepository
    {
        Task<Event?> GetByIdAsync(Guid eventId);
        Task<Tuple<IEnumerable<Event>, int>> GetPublicEventsAsync(int page, int pageSize, string? category, string? search, string? date);
        Task<IEnumerable<Event>> GetEventsByOrganizerAsync(Guid organizerId);
        Task<bool> AddAsync(Event eventEntity);
        Task<bool> UpdateAsync(Event eventEntity);
        Task<bool> DeleteAsync(Guid eventId);
    }
}