using ReceptServer.Domain;

namespace ReceptServer.Application;

// Abstraktion til persistence af recepter.
public interface IPrescriptionRepository
{
    Task AddAsync(Prescription prescription, CancellationToken cancellationToken = default);
    Task<Prescription?> GetByIdAsync(Guid prescriptionId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Prescription>> GetOpenByCprAsync(string cprNumber, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Prescription>> GetOpenByAssignedPharmacyAsync(Guid pharmacyId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Prescription>> GetExpiredPrescriptionsAsync(DateTime referenceDate, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
