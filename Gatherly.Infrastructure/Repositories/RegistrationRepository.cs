using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gatherly.Domain.Entities;
using Gatherly.Domain.Interfaces;
using Gatherly.Infrastructure.Data; 
using Microsoft.EntityFrameworkCore;

namespace Gatherly.Infrastructure.Repositories;

public class RegistrationRepository : IRegistrationRepository
{
    private readonly ApplicationDbContext _context;

 
    public RegistrationRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Registration?> GetByIdAsync(Guid registrationId)
    {
        return await _context.Registrations
            .FirstOrDefaultAsync(r => r.RegistrationId == registrationId);
    }

    public async Task<Registration?> GetByUserAndEventAsync(Guid userId, Guid eventId)
    {
        return await _context.Registrations
            .FirstOrDefaultAsync(r => r.UserId == userId && r.EventId == eventId);
    }

    public async Task<IEnumerable<Registration>> GetByEventIdAsync(Guid eventId)
    {
        return await _context.Registrations
            .Where(r => r.EventId == eventId)
            .ToListAsync();
    }

    public async Task<IEnumerable<Registration>> GetByUserIdAsync(Guid userId)
    {
        return await _context.Registrations
            .Where(r => r.UserId == userId)
            .ToListAsync();
    }

    public async Task<bool> AddAsync(Registration registration)
    {
        await _context.Registrations.AddAsync(registration);

       
        int rowsAffected = await _context.SaveChangesAsync();
        return rowsAffected > 0;
    }

    public async Task<bool> UpdateAsync(Registration registration)
    {
        _context.Registrations.Update(registration);

        int rowsAffected = await _context.SaveChangesAsync();
        return rowsAffected > 0;
    }

    public async Task<int> GetCountByEventIdAsync(Guid eventId)
    {
        
        return await _context.Registrations
            .CountAsync(r => r.EventId == eventId && r.Status != "Cancelled");
    }
}