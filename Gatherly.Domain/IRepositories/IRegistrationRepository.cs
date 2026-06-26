using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Gatherly.Domain.Entities;

namespace Gatherly.Domain.Interfaces
{
    public interface IRegistrationRepository
    {
        Task<Registration?> GetByIdAsync(Guid registrationId);
        Task<Registration?> GetByUserAndEventAsync(Guid userId, Guid eventId);
        Task<IEnumerable<Registration>> GetByEventIdAsync(Guid eventId);
        Task<IEnumerable<Registration>> GetByUserIdAsync(Guid userId);
        Task<bool> AddAsync(Registration registration);
        Task<bool> UpdateAsync(Registration registration);
        Task<int> GetCountByEventIdAsync(Guid eventId);
        Task<Registration?> GetByUserOrEmailAndEventAsync(Guid? userId, string? email, Guid eventId);
    }   
      
}