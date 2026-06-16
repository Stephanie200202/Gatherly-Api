using Gatherly.Domain.layer;

namespace Gatherly.Domain.layer
{
    /// <summary>
    /// Tracks the lifecycle of a created event.
    /// </summary>
    public enum EventStatus
    {
        Draft,
        Published,
        RegistrationClosed,
        Cancelled
    }
}
