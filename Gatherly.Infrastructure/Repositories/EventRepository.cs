using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gatherly.Domain.Entities;
using Gatherly.Domain.Interfaces;
using Gatherly.Infrastructure.Data; // Adjust to your actual ApplicationDbContext namespace
using Microsoft.EntityFrameworkCore;

namespace Gatherly.Infrastructure.Repositories;

public class EventRepository : IEventRepository
{
    private readonly ApplicationDbContext _context;

   
    public EventRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    
    public async Task<Event?> GetByIdAsync(Guid eventId)
    {
        return await _context.Events
            .FirstOrDefaultAsync(e => e.EventId == eventId);
    }

   
    public async Task<Tuple<IEnumerable<Event>, int>> GetPublicEventsAsync(int page, int pageSize, string? category, string? search, string? date)
    {
       
        var query = _context.Events.Where(e => e.Visibility == "Public");

       
        if (!string.IsNullOrEmpty(category))
        {
         
            query = query.Where(e => e.Category == category);
        }

       
        if (!string.IsNullOrEmpty(search))
        {
            query = query.Where(e => e.Title.Contains(search) || e.Venue.Contains(search));
        }

       
        if (!string.IsNullOrEmpty(date))
        {
            query = query.Where(e => e.Date == date);
        }

        
        int totalCount = await query.CountAsync();

        
        var paginatedItems = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new Tuple<IEnumerable<Event>, int>(paginatedItems, totalCount);
    }

   
    public async Task<IEnumerable<Event>> GetEventsByOrganizerAsync(Guid organizerId)
    {
        return await _context.Events
            .Where(e => e.OrganizerId == organizerId)
            .ToListAsync();
    }

    
    public async Task<bool> AddAsync(Event eventEntity)
    {
        await _context.Events.AddAsync(eventEntity);
        int rowsAffected = await _context.SaveChangesAsync(); 
        return rowsAffected > 0;
    }

    
    public async Task<bool> UpdateAsync(Event eventEntity)
    {
        _context.Events.Update(eventEntity);
        int rowsAffected = await _context.SaveChangesAsync(); 
        return rowsAffected > 0;
    }

   
    public async Task<bool> DeleteAsync(Guid eventId)
    {
        var eventEntity = await _context.Events.FirstOrDefaultAsync(e => e.EventId == eventId);

        if (eventEntity == null)
        {
            return false; 
        }

        _context.Events.Remove(eventEntity);
        int rowsAffected = await _context.SaveChangesAsync(); 
        return rowsAffected > 0;
    }
}