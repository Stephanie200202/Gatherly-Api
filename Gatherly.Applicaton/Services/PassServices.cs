using System;
using System.Collections.Generic;
using Gatherly.Application.IServices;
using Gatherly.Applicaton.IServices;
using Gatherly.Domain.Entities;
using Gatherly.Domain.IRepositories;
using Gatherly.Domain.layer; // For PassStatus enum

namespace Gatherly.Application.Services
{
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

        public IEnumerable<Pass> SearchPasses(Guid eventId, string searchTerm) =>
            _passRepository.SearchPasses(eventId, searchTerm);

        public bool InvalidatePass(Guid passId, string reason)
        {
            var pass = _passRepository.GetById(passId);
            if (pass == null) return false;

            pass.Status = PassStatus.Invalidated.ToString();
            _passRepository.Update(pass);
            return true;
        }

        public Pass? RegeneratePass(Guid passId, string reason)
        {
            var oldPass = _passRepository.GetById(passId);
            if (oldPass == null) return null;

            // 1. Kill the old ticket
            oldPass.Status = PassStatus.Invalidated.ToString();
            _passRepository.Update(oldPass);

            // 2. Mint the new ticket with synchronized Guid keys
            var newPassId = Guid.NewGuid();

            var newPass = new Pass
            {
                PassId = newPassId,
                EventId = oldPass.EventId,
                EventTitle = oldPass.EventTitle,
                EventDate = oldPass.EventDate,
                EventVenue = oldPass.EventVenue,
                AttendeeName = oldPass.AttendeeName,
                TicketType = oldPass.TicketType,
                Status = PassStatus.Active.ToString(),
                // Making payload parseable into a clean Guid for your verification system
                QrPayload = newPassId.ToString(),
                TokenExpiresAt = oldPass.TokenExpiresAt,
                RegistrationId = oldPass.RegistrationId
            };

            _passRepository.Add(newPass);
            return newPass;
        }
    }
}