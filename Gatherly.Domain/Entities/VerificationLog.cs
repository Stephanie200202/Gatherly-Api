//using System;

//namespace Gatherly.Domain.Entities
//{
//    public class VerificationLog
//    {
//        public Guid LogId { get; set; } = Guid.NewGuid();
//        public Guid EventId { get; set; }
//        public Guid PassId { get; set; }
//        public string AttendeeName { get; set; } = string.Empty;
//        public string TicketType { get; set; } = string.Empty;
//        public string Result { get; set; } = string.Empty; // Approved, AlreadyUsed, InvalidPass, Expired, Flagged, etc. [cite: 38]
//        public string GateId { get; set; } = string.Empty;
//        public DateTime ScannedAt { get; set; } = DateTime.UtcNow;
//    }
//}




using System;

namespace Gatherly.Domain.Entities
{
    public class VerificationLog
    {
        // Renaming this to 'Id' fixes it automatically
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid EventId { get; set; }
        public Guid PassId { get; set; }
        public string AttendeeName { get; set; } = string.Empty;
        public string TicketType { get; set; } = string.Empty;
        public string Result { get; set; } = string.Empty;
        public string GateId { get; set; } = string.Empty;
        public DateTime ScannedAt { get; set; } = DateTime.UtcNow;
    }
}