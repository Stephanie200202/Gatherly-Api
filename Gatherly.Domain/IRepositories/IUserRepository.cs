using Gatherly.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Gatherly.Domain.IRepositories
{
    public interface IUserRepository
    {
        Task<ApplicationUser> GetByIdAsync(Guid id);
        Task<ApplicationUser> GetByIdentifierAsync(string identifier); // Looks up by Email or Phone
        Task<ApplicationUser> GetByRefreshTokenAsync(string refreshToken);
        Task<ApplicationUser> GetByResetTokenAsync(string resetToken);
        Task AddAsync(ApplicationUser user);
        Task UpdateAsync(ApplicationUser user);
    }
}