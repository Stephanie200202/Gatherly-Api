using Gatherly.Domain.layer;

namespace Gatherly.Domain.layer
{
    /// <summary>
    /// Defines platform-wide access levels and permissions.
    /// </summary>
    public enum UserRole
    {
        Organizer,
        Attendee,
        GateManager,
        Admin
    }
}
