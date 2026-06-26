using System;
using Microsoft.AspNetCore.Identity;
using System;

namespace Gatherly.Domain.Entities
{
    //public class ApplicationUser
    //{
    //    public Guid UserId { get; set; } = Guid.NewGuid();
    //    public string FullName { get; set; }
    //    public string Email { get; set; }
    //    public string Phone { get; set; }
    //    public string PasswordHash { get; set; }
    //    public string Role { get; set; } // Attendee, Organizer, GateManager, Admin
    //    public string ProfilePhoto { get; set; }
    //    public string RefreshToken { get; set; }
    //    public DateTime? RefreshTokenExpiryTime { get; set; }
    //    public string ResetToken { get; set; }
    //    public DateTime? ResetTokenExpiryTime { get; set; }
    //    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    //    public DateTime? UpdatedAt { get; set; }
    //}



    public class ApplicationUser : IdentityUser<Guid>
    {

        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string FullName => $"{FirstName} {LastName}";
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string RefreshToken { get; set; }
        public string Role { get; set; }
        public DateTime RefreshTokenExpiryTime { get; set; }
        public string? ResetToken { get; set; }
        public DateTime? ResetTokenExpiryTime { get; set; }
        public string ProfilePhoto { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}