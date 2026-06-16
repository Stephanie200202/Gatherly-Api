using System;
using System.Collections.Generic;
using System.Linq;
using Gatherly.Domain.Entities;
using Gatherly.Domain.Interfaces; 
using Gatherly.Domain.IRepositories;
using Gatherly.Infrastructure.Data; 

namespace Gatherly.Infrastructure.Repositories;

public class VerificationRepository : IVerificationRepository
{
    private readonly ApplicationDbContext _context;

    public VerificationRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public void AddLog(VerificationLog log)
    {
        _context.VerificationLogs.Add(log);
        _context.SaveChanges();
    }

    public IEnumerable<VerificationLog> GetLogsByEventId(Guid eventId)
    {
        return _context.VerificationLogs
            .Where(l => l.EventId == eventId)
            .ToList();
    }

    public void RecordAttendeeStatus(Guid passId, string status)
    {
      
        var pass = _context.Passes.FirstOrDefault(p => p.PassId == passId);

  
        if (pass == null)
        {
            return;
        }

  
        pass.Status = status;

        _context.SaveChanges();
    }

    public string GetAttendeeStatus(Guid passId)
    {
        var pass = _context.Passes.FirstOrDefault(p => p.PassId == passId);
        return pass?.Status ?? "Unknown";
    }
}