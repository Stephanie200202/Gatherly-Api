using Gatherly.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Reflection.Emit;

namespace Gatherly.Infrastructure.Data
{
    // Inherit from IdentityDbContext to unlock all built-in identity tables with Guid keys
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        // Custom system domain tables
        public DbSet<Event> Events { get; set; } = null!;
        public DbSet<Registration> Registrations { get; set; } = null!;
        public DbSet<Pass> Passes { get; set; } = null!;
        public DbSet<VerificationLog> VerificationLogs { get; set; } = null!;
        public DbSet<Notification> Notifications { get; set; } = null!;
        public DbSet<Announcement> Announcements { get; set; } = null!;
        public DbSet<Feedback> Feedbacks { get; set; }

       

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // CRITICAL: Always call base.OnModelCreating first when using Identity. 
            // This maps the identity schema tables correctly before configuring your custom relationships.
            base.OnModelCreating(modelBuilder);

            // Unique constraints for authentication identifiers
            modelBuilder.Entity<ApplicationUser>().HasIndex(u => u.Email).IsUnique();
            modelBuilder.Entity<ApplicationUser>().HasIndex(u => u.PhoneNumber).IsUnique();

            // Event Relationships
            modelBuilder.Entity<Event>()
                .HasOne(e => e.Organizer)
                .WithMany()
                .HasForeignKey(e => e.OrganizerId)
                .OnDelete(DeleteBehavior.Restrict);

            // Registration Relationships
            modelBuilder.Entity<Registration>()
                .HasOne<Event>()
                .WithMany()
                .HasForeignKey(r => r.EventId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Registration>()
                .HasOne<ApplicationUser>()
                .WithMany()
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Pass Relationships
            modelBuilder.Entity<Pass>()
                .HasOne<Registration>()
                .WithMany()
                .HasForeignKey(p => p.RegistrationId)
                .OnDelete(DeleteBehavior.Cascade);

            // Notification & Announcement Relationships
            modelBuilder.Entity<Notification>()
                .HasOne<ApplicationUser>()
                .WithMany()
                .HasForeignKey(n => n.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Announcement>()
                .HasOne<Event>()
                .WithMany()
                .HasForeignKey(a => a.EventId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
