using Gatherly.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Gatherly.Applicaton.IServices
{
    public interface IPassService
    {
        Pass? GetPassDetails(Guid passId);
        Pass? GetPassByRegistration(Guid registrationId);
        bool InvalidatePass(Guid passId, string reason);
        Pass? RegeneratePass(Guid passId, string reason);
    }
}