using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gatherly.Domain.Entities;
using Gatherly.Domain.IRepositories;
using Gatherly.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Gatherly.Infrastructure.Repositories;

public class AnnouncementRepository : IAnnouncementRepository
{
    private readonly ApplicationDbContext _context;

    public AnnouncementRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Announcement announcement)
    {
        await _context.Announcements.AddAsync(announcement);
        await _context.SaveChangesAsync(); 
    }

    public async Task<IEnumerable<Announcement>> GetByEventIdAsync(Guid eventId)
    {
        return await _context.Announcements
            .Where(a => a.EventId == eventId)
            .ToListAsync(); 
    }
}