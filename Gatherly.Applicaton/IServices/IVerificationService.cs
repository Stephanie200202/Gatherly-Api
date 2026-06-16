using Gatherly.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Gatherly.Applicaton.IServices
{
    public interface IVerificationService
    {
        object ScanPass(Guid eventId, string qrPayload, string gateId, string deviceId);
        IEnumerable<object> ManualSearch(Guid eventId, string searchTerm, string gateId);
        IEnumerable<VerificationLog> GetLog(Guid eventId);
        object GetActiveAttendees(Guid eventId);
    }
}
