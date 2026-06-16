using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Gatherly.Domain.Entities;

namespace Gatherly.Domain.IRepositories
{
    public interface IVerificationRepository
    {
        void AddLog(VerificationLog log);
        IEnumerable<VerificationLog> GetLogsByEventId(Guid eventId);
        void RecordAttendeeStatus(Guid passId, string status); // Inside, Outside [cite: 43]
        string GetAttendeeStatus(Guid passId);
    }

}


