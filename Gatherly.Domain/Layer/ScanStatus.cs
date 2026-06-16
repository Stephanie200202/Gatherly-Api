namespace Gatherly.Domain.layer
{
    /// <summary>
    /// Evaluates the UI feedback state during access pass verification.
    /// </summary>
    public enum ScanResult
    {
        Approved,
        AlreadyUsed,
        InvalidPass,
        Expired,
        Flagged,
        EventNotStarted,
        NotAllowed
    }
}
