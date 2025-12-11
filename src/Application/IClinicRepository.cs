using ReceptServer.Domain;

namespace ReceptServer.Application;

public interface IClinicRepository
{
    Task<Clinic?> GetByIdAsync(Guid clinicId, CancellationToken cancellationToken = default);
    Task<Clinic?> GetByProviderNumberAsync(string providerNumber, CancellationToken cancellationToken = default);
}
