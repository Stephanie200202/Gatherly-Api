//using System;
//using Gatherly.Domain.Entities;
//using Gatherly.Domain.IRepositories;
//using Gatherly.Domain.layer;

//namespace Gatherly.Application.Services
//{
//    public class PassService
//    {
//        private readonly IPassRepository _passRepository;

//        public PassService(IPassRepository passRepository)
//        {
//            _passRepository = passRepository;
//        }

//        public Pass? GetPassDetails(Guid passId) => _passRepository.GetById(passId);
//        public Pass? GetPassByRegistration(Guid registrationId) => _passRepository.GetByRegistrationId(registrationId);

//        public bool InvalidatePass(Guid passId, string reason)
//        {
//            var pass = _passRepository.GetById(passId);
//            if (pass == null) return false;

//            pass.Status = PassStatus.Invalidated;
//            _passRepository.Update(pass);
//            return true;
//        }

//        public Pass? RegeneratePass(Guid passId, string reason)
//        {
//            var oldPass = _passRepository.GetById(passId);
//            if (oldPass == null) return null;

//            oldPass.Status = PassStatus.Invalidated;
//            _passRepository.Update(oldPass);

//            var newPass = new Pass
//            {
//                EventId = oldPass.EventId,
//                EventTitle = oldPass.EventTitle,
//                EventDate = oldPass.EventDate,
//                EventVenue = oldPass.EventVenue,
//                AttendeeName = oldPass.AttendeeName,
//                TicketType = oldPass.TicketType,
//                Status = PassStatus.Active,
//                QrPayload = Guid.NewGuid().ToString("N"),
//                TokenExpiresAt = DateTime.UtcNow.AddDays(1),
//                RegistrationId = oldPass.RegistrationId
//            };
//            _passRepository.Add(newPass);
//            return newPass;
//        }
//    }
//}