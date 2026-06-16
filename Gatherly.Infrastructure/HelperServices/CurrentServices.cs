using System.Security.Claims;
using Gatherly.Application.Abstractions;
using Microsoft.AspNetCore.Http;

namespace Gatherly.Infrastructure.HelperServices;

public class CurrentUserService : ICurrentUserAbstraction
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    // 1. Fully robust string ID parsing supporting multiple claim formats
    public string UserId
    {
        get
        {
            var user = _httpContextAccessor.HttpContext?.User;

            // 🚀 THE FIX: Checks for nameid or sub, matching your jwt.io token format exactly
            var userIdClaim = user?.FindFirstValue(ClaimTypes.NameIdentifier)
                              ?? user?.FindFirstValue("sub")
                              ?? user?.FindFirstValue("nameid");

            return userIdClaim ?? string.Empty;
        }
    }

    // 2. Grab the Role string to match your interface using dual-claim support
    public string Role
    {
        get
        {
            var user = _httpContextAccessor.HttpContext?.User;

            // Checks for full XML schema format or the simple custom shorthand string "role"
            var roleClaim = user?.FindFirstValue(ClaimTypes.Role)
                            ?? user?.FindFirstValue("role");

            return roleClaim ?? string.Empty;
        }
    }
}