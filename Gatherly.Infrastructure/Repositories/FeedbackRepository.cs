using Gatherly.Domain.Entities;     
using Gatherly.Domain.Interfaces;
using Gatherly.Infrastructure.Data;    
using Microsoft.EntityFrameworkCore;

namespace Gatherly.Infrastructure.Repositories;

public class FeedbackRepository : IFeedbackRepository
{
    private readonly ApplicationDbContext _context;

    public FeedbackRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Feedback?> GetByIdAsync(Guid feedbackId)
    {
        
        return await _context.Feedbacks
            .FirstOrDefaultAsync(f => f.FeedbackId == feedbackId);
    }

    public async Task<Feedback?> GetByUserAndEventAsync(Guid userId, Guid eventId)
    {
       
        return await _context.Feedbacks
            .FirstOrDefaultAsync(f => f.UserId == userId && f.EventId == eventId);
    }

    public async Task<IEnumerable<Feedback>> GetByEventIdAsync(Guid eventId)
    {
        
        return await _context.Feedbacks
            .Where(f => f.EventId == eventId)
            .ToListAsync();
    }

    public async Task<bool> AddAsync(Feedback feedback)
    {
        await _context.Feedbacks.AddAsync(feedback);
        int rowsAffected = await _context.SaveChangesAsync();

       
        return rowsAffected > 0;
    }

    public async Task<bool> DeleteAsync(Guid feedbackId)
    {
        var feedback = await _context.Feedbacks.FindAsync(feedbackId);
        if (feedback == null)
        {
            return false;
        }

        _context.Feedbacks.Remove(feedback);
        int rowsAffected = await _context.SaveChangesAsync();

        return rowsAffected > 0;
    }
}