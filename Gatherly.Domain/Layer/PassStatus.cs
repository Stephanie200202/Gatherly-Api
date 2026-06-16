namespace Gatherly.Domain.layer
{
    /// <summary>
    /// Represents the operational state of a generated event access pass.
    /// </summary>
    public enum PassStatus
    {
        Active,
        Used,
        Expired,
        Invalidated,
        Flagged
    }
}