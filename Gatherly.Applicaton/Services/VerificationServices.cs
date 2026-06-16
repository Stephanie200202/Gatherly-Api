using Gatherly.Applicaton.IServices;
using Gatherly.Domain.Entities;
using Gatherly.Domain.IRepositories;
using Gatherly.Domain.layer;


namespace Gatherly.Application.Services
{
    public class VerificationService : IVerificationService
    {
        private readonly IPassRepository _passRepository;
        private readonly IVerificationRepository _verificationRepository;

        // Domain Constants for Tracking Statuses
        private const string StatusInside = "Inside";
        private const string StatusOutside = "Outside";

        // 1. Constructor Injection added to fix missing dependency variables
        public VerificationService(
            IPassRepository passRepository,
            IVerificationRepository verificationRepository)
        {
            _passRepository = passRepository;
            _verificationRepository = verificationRepository;
        }

        public object ScanPass(Guid eventId, string qrPayload, string gateId, string deviceId)
        {
            // Parse and Fetch Pass securely
            var isGuid = Guid.TryParse(qrPayload, out var parsedPassId);
            var pass = isGuid ? _passRepository.GetById(parsedPassId) : null;

            // Establish fallback tracking values up front
            var result = ScanResult.InvalidPass;
            var message = "QR payload is invalid or tampered.";
            var attendeeName = "Unknown";
            var ticketType = "General";
            var passId = Guid.Empty;

            // 2. Main Validation Pipeline (Guard Clauses)
            if (pass != null)
            {
                passId = pass.PassId;
                attendeeName = pass.AttendeeName;
                ticketType = pass.TicketType;

                if (pass.EventId != eventId)
                {
                    result = ScanResult.NotAllowed;
                    message = "Pass belongs to a different event.";
                }
                else if (pass.Status == PassStatus.Invalidated.ToString() || pass.Status == PassStatus.Flagged.ToString())
                {
                    result = ScanResult.Flagged;
                    message = "Pass is flagged — alert security team.";
                }
                // Use domain tracking constant instead of raw magic string
                else if (_verificationRepository.GetAttendeeStatus(pass.PassId) == StatusInside)
                {
                    result = ScanResult.AlreadyUsed;
                    message = "Pass already scanned — duplicate access denied.";
                }
                else
                {
                    // Access Approved
                    result = ScanResult.Approved;
                    message = "Access granted. Welcome!";

                    pass.Status = PassStatus.Used.ToString();
                    _passRepository.Update(pass);
                    _verificationRepository.RecordAttendeeStatus(pass.PassId, StatusInside);
                }
            }

            // 3. Document and persist the scan log attempt
            var log = new VerificationLog
            {
                EventId = eventId,
                PassId = passId,
                AttendeeName = attendeeName,
                TicketType = ticketType,
                Result = result.ToString(),
                GateId = gateId,
                ScannedAt = DateTime.UtcNow
            };
            _verificationRepository.AddLog(log);

            return new
            {
                result = result.ToString(),
                attendeeName,
                ticketType,
                passId,
                message,
                checkedInAt = DateTime.UtcNow
            };
        }

        public IEnumerable<object> ManualSearch(Guid eventId, string searchTerm, string gateId)
        {
            var passes = _passRepository.SearchPasses(eventId, searchTerm);

            return passes.Select(p => new
            {
                passId = p.PassId,
                attendeeName = p.AttendeeName,
                ticketType = p.TicketType,
                passStatus = p.Status,
                checkedIn = _verificationRepository.GetAttendeeStatus(p.PassId) == StatusInside
            }).ToList(); // Materialize collection explicitly
        }

        public IEnumerable<VerificationLog> GetLog(Guid eventId) =>
            _verificationRepository.GetLogsByEventId(eventId);

        public object GetActiveAttendees(Guid eventId)
        {
            // Fetch historical approved entries
            var approvedLogs = _verificationRepository.GetLogsByEventId(eventId)
                .Where(l => l.Result == ScanResult.Approved.ToString())
                .ToList();

            // Cross-verify against their actual real-time location status 
            // to prevent inflating headcount if an attendee leaves the building
            var activeAttendees = approvedLogs
                .Where(l => _verificationRepository.GetAttendeeStatus(l.PassId) == StatusInside)
                .Select(l => new
                {
                    l.PassId,
                    l.AttendeeName,
                    l.TicketType,
                    checkedInAt = l.ScannedAt
                })
                .DistinctBy(l => l.PassId) // Prevent duplicate row counting if they re-scanned
                .ToList();

            return new
            {
                eventId,
                currentlyInside = activeAttendees.Count,
                attendees = activeAttendees
            };
        }
    }
}











        //public object ScanPass(Guid eventId, string qrPayload, string gateId, string deviceId)
        //{
        //    // 1. Parse and Fetch Pass
        //    var isGuid = Guid.TryParse(qrPayload, out var parsedPassId);
        //    var pass = isGuid ? _passRepository.GetById(parsedPassId) : null;

        //    // Base/Default values if pass isn't found
        //    var result = ScanResult.InvalidPass;
        //    var message = "QR payload is invalid or tampered.";
        //    var attendeeName = "Unknown";
        //    var ticketType = "General";
        //    var passId = Guid.Empty;

        //    // 2. Validate Pass Statuses (Guard Clauses)
        //    if (pass != null)
        //    {
        //        passId = pass.PassId;
        //        attendeeName = pass.AttendeeName;
        //        ticketType = pass.TicketType;

        //        if (pass.EventId != eventId)
        //        {
        //            result = ScanResult.NotAllowed;
        //            message = "Pass belongs to a different event.";
        //        }
        //        else if (pass.Status == PassStatus.Invalidated.ToString() || pass.Status == PassStatus.Flagged.ToString())
        //        {
        //            result = ScanResult.Flagged;
        //            message = "Pass is flagged — alert security team.";
        //        }
        //        else if (_verificationRepository.GetAttendeeStatus(pass.PassId) == "Inside")
        //        {
        //            result = ScanResult.AlreadyUsed;
        //            message = "Pass already scanned — duplicate access denied.";
        //        }
        //        else
        //        {
        //            // Access Approved
        //            result = ScanResult.Approved;
        //            message = "Access granted. Welcome!";

        //            pass.Status = PassStatus.Used.ToString();
        //            _passRepository.Update(pass);
        //            _verificationRepository.RecordAttendeeStatus(pass.PassId, "Inside");
        //        }
        //    }

            // 3. Log the attempt
//            var log = new VerificationLog
//            {
//                EventId = eventId,
//                PassId = passId,
//                AttendeeName = attendeeName,
//                TicketType = ticketType,
//                Result = result.ToString(),
//                GateId = gateId,
//                ScannedAt = DateTime.UtcNow // Added to fix the missing property in GetActiveAttendees
//            };
//            _verificationRepository.AddLog(log);

//            return new { result = result.ToString(), attendeeName, ticketType, passId, message, checkedInAt = DateTime.UtcNow };
//        }

//        // Simplified to actually utilize the repository (Assuming your repository supports searching)
//        public IEnumerable<object> ManualSearch(Guid eventId, string searchTerm, string gateId)
//        {
//            var passes = _passRepository.SearchPasses(eventId, searchTerm); // Adjust method name to match your repo

//            return passes.Select(p => new
//            {
//                passId = p.PassId,
//                attendeeName = p.AttendeeName,
//                ticketType = p.TicketType,
//                passStatus = p.Status,
//                checkedIn = _verificationRepository.GetAttendeeStatus(p.PassId) == "Inside"
//            });
//        }
//        public IEnumerable<VerificationLog> GetLog(Guid eventId) => _verificationRepository.GetLogsByEventId(eventId);

//        public object GetActiveAttendees(Guid eventId)
//        {
//            // Fixed missing () on ToString()
//            var approvedLogs = _verificationRepository.GetLogsByEventId(eventId)
//                .Where(l => l.Result == ScanResult.Approved.ToString())
//                .ToList();

//            return new
//            {
//                eventId,
//                currentlyInside = approvedLogs.Count,
//                attendees = approvedLogs.Select(l => new { l.PassId, l.AttendeeName, l.TicketType, checkedInAt = l.ScannedAt })
//            };
//        }
//    }
//}






//        public object ScanPass(Guid eventId, string qrPayload, string gateId, string deviceId)
//        {
//            var pass = _passRepository.GetById(Guid.TryParse(qrPayload, out var id) ? id : Guid.Empty);

//            ScanResult result = ScanResult.InvalidPass;
//            string message = "QR payload is invalid or tampered.";
//            string attendeeName = "Unknown";
//            string ticketType = "General";
//            Guid passId = Guid.Empty;

//            if (pass != null)
//            {
//                passId = pass.PassId;
//                attendeeName = pass.AttendeeName;
//                ticketType = pass.TicketType;

//                if (pass.EventId != eventId)
//                {
//                    result = ScanResult.NotAllowed;
//                    message = "Pass belongs to a different event.";
//                }
//                else if (pass.Status == pass.Status.Invalidated.ToString() || PassStatus.Flagged.ToString()) ;
//                {
//                    result = ScanResult.Flagged;
//                    message = "Pass is flagged — alert security team.";
//                }
//                        else if (_verificationRepository.GetAttendeeStatus(pass.PassId) == "Inside")
//                {
//                    result = ScanResult.AlreadyUsed;
//                    message = "Pass already scanned — duplicate access denied.";
//                }
//                else
//                {
//                    result = ScanResult.Approved;
//                    message = "Access granted. Welcome!";
//                    pass.Status = PassStatus.Used.ToString();
//                    _passRepository.Update(pass);
//                    _verificationRepository.RecordAttendeeStatus(pass.PassId, "Inside");
//                }
//            }

//            var log = new VerificationLog
//            {
//                EventId = eventId,
//                PassId = passId,
//                AttendeeName = attendeeName,
//                TicketType = ticketType,
//                Result = result.ToString(),
//                GateId = gateId
//            };
//            _verificationRepository.AddLog(log);

//            return new { result = result.ToString(), attendeeName, ticketType, passId, message, checkedInAt = DateTime.UtcNow };
//        }

//        public IEnumerable<object> ManualSearch(Guid eventId, string searchTerm, string gateId)
//        {
//            return new List<object>
//                    {
//                        new { passId = Guid.NewGuid(), attendeeName = searchTerm, ticketType = "General", passStatus = PassStatus.Active.ToString(), checkedIn = false }
//                    };
//        }

//        public IEnumerable<VerificationLog> GetLog(Guid eventId) => _verificationRepository.GetLogsByEventId(eventId);

//        public object GetActiveAttendees(Guid eventId)
//        {
//            var logs = _verificationRepository.GetLogsByEventId(eventId).Where(l => l.Result == ScanResult.Approved.ToString);
//            return new
//            {
//                eventId,
//                currentlyInside = logs.Count(),
//                attendees = logs.Select(l => new { l.PassId, l.AttendeeName, l.TicketType, checkedInAt = l.ScannedAt })
//            };
//        }
//    }
//}