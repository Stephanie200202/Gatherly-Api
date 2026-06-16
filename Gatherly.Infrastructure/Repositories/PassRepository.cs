using System;
using System.Collections.Generic;
using System.Linq;
using Gatherly.Domain.Entities;
using Gatherly.Domain.IRepositories;
using Gatherly.Infrastructure.Data; 

namespace Gatherly.Infrastructure.Repositories;

public class PassRepository : IPassRepository
{
    private readonly ApplicationDbContext _context;


    public PassRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public Pass? GetById(Guid passId)
    {
        return _context.Passes
            .FirstOrDefault(p => p.PassId == passId);
    }

    public Pass? GetByRegistrationId(Guid registrationId)
    {
        return _context.Passes
            .FirstOrDefault(p => p.RegistrationId == registrationId);
    }

    public void Add(Pass pass)
    {
        _context.Passes.Add(pass);
        _context.SaveChanges(); 
    }

    public void Update(Pass pass)
    {
        _context.Passes.Update(pass);
        _context.SaveChanges(); 
    }

    public IEnumerable<Pass> SearchPasses(Guid eventId, string searchTerm)
    {
     
        return _context.Passes
            .Where(p => p.EventId == eventId &&
                        (p.AttendeeName.Contains(searchTerm) ||
                         p.TicketType.Contains(searchTerm)))
            .ToList();
    }
}