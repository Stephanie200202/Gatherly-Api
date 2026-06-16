using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gatherly.Domain.Entities;
using Gatherly.Domain.IRepositories;
using Gatherly.Infrastructure.Data; // Adjust to your actual ApplicationDbContext namespace
using Microsoft.EntityFrameworkCore;

namespace Gatherly.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _context;

    // Inject the real database context here
    public UserRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ApplicationUser?> GetByIdAsync(Guid id)
    {
        // Finds a user matching the primary key in SQL Server asynchronously
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<ApplicationUser?> GetByIdentifierAsync(string identifier)
    {
        // Checks both columns in the database table
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Email == identifier || u.PhoneNumber == identifier);
    }

    public async Task<ApplicationUser?> GetByRefreshTokenAsync(string refreshToken)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);
    }

    public async Task<ApplicationUser?> GetByResetTokenAsync(string resetToken)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.ResetToken == resetToken);
    }

    public async Task AddAsync(ApplicationUser user)
    {
        // 1. Stage the user into EF Core tracking memory
        await _context.Users.AddAsync(user);

        // 2. Persist the record directly to your database table!
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(ApplicationUser user)
    {
        // Tell EF Core to update the modified properties
        _context.Users.Update(user);

        // Push changes to SQL Server
        await _context.SaveChangesAsync();
    }
}