using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gatherly.Domain.Entities;
using System.Security.Claims;

namespace Gatherly.Application.Abstractions
{
    public interface IJwtProvider
    {
        TokenResult GenerateToken(ApplicationUser applicationUser);
        ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
    }
}