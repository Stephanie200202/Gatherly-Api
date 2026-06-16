using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Gatherly.Domain.Entities;

namespace Gatherly.Domain.Interfaces
{
    public interface IFeedbackRepository
    {
        Task<Feedback?> GetByIdAsync(Guid feedbackId);
        Task<Feedback?> GetByUserAndEventAsync(Guid userId, Guid eventId);
        Task<IEnumerable<Feedback>> GetByEventIdAsync(Guid eventId);
        Task<bool> AddAsync(Feedback feedback);
        Task<bool> DeleteAsync(Guid feedbackId);
    }
}