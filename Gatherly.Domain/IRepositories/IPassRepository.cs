using Gatherly.Domain.Entities;

public interface IPassRepository
{
    Pass? GetById(Guid passId);
    Pass? GetByRegistrationId(Guid registrationId);
    void Add(Pass pass);
    void Update(Pass pass);
    IEnumerable<Pass> SearchPasses(Guid eventId, string searchTerm);
}