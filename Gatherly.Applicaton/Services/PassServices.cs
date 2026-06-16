using System;
using Gatherly.Application.IServices;
using Gatherly.Applicaton.IServices;
using Gatherly.Domain.Entities;
using Gatherly.Domain.IRepositories;
using Gatherly.Domain.layer; // For your PassStatus enum

namespace Gatherly.Application.Services;

public class PassService : IPassService
{
    private readonly IPassRepository _passRepository;

    public PassService(IPassRepository passRepository)
    {
        _passRepository = passRepository;
    }

    public Pass? GetPassDetails(Guid passId) =>
        _passRepository.GetById(passId);

    public Pass? GetPassByRegistration(Guid registrationId) =>
        _passRepository.GetByRegistrationId(registrationId);

    public bool InvalidatePass(Guid passId, string reason)
    {
        var pass = _passRepository.GetById(passId);
        if (pass == null)
        {
            return false;
        }

        // Updated from string to Domain Enum string conversion
        pass.Status = PassStatus.Invalidated.ToString();

        _passRepository.Update(pass);
        return true;
    }

    public Pass? RegeneratePass(Guid passId, string reason)
    {
        var oldPass = _passRepository.GetById(passId);
        if (oldPass == null)
        {
            return null;
        }

        // Invalidate the compromised/old pass
        oldPass.Status = PassStatus.Invalidated.ToString();
        _passRepository.Update(oldPass);

        // Issue a completely fresh pass entity referencing the original registration
        var newPass = new Pass
        {
            EventId = oldPass.EventId,
            EventTitle = oldPass.EventTitle,
            EventDate = oldPass.EventDate,
            EventVenue = oldPass.EventVenue,
            AttendeeName = oldPass.AttendeeName,
            TicketType = oldPass.TicketType,
            Status = PassStatus.Active.ToString(),
            QrPayload = "regenerated-" + Guid.NewGuid().ToString()[..8],
            TokenExpiresAt = oldPass.TokenExpiresAt,
            RegistrationId = oldPass.RegistrationId
        };

        _passRepository.Add(newPass);
        return newPass;
    }
}