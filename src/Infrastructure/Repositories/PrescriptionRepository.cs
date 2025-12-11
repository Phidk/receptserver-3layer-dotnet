using Microsoft.EntityFrameworkCore;
using ReceptServer.Application;
using ReceptServer.Domain;

namespace ReceptServer.Infrastructure.Repositories;

public sealed class PrescriptionRepository : IPrescriptionRepository
{
    private readonly ReceptDbContext _dbContext;

    public PrescriptionRepository(ReceptDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public Task AddAsync(Prescription prescription, CancellationToken cancellationToken = default)
    {
        // Recept tilføjes til kontekst, ændringer gemmes ved SaveChanges.
        return _dbContext.Prescriptions.AddAsync(prescription, cancellationToken).AsTask();
    }

    public async Task<Prescription?> GetByIdAsync(Guid prescriptionId, CancellationToken cancellationToken = default)
    {
        // Henter recept inkl. ordinationer for at kunne udlevere.
        return await _dbContext.Prescriptions
            .Include(p => p.MedicationOrders)
            .FirstOrDefaultAsync(p => p.PrescriptionId == prescriptionId, cancellationToken);
    }

    public async Task<IReadOnlyList<Prescription>> GetOpenByAssignedPharmacyAsync(Guid pharmacyId, CancellationToken cancellationToken = default)
    {
        // Filtrerer på apotek ID og kun åbne recepter.
        return await _dbContext.Prescriptions
            .Include(p => p.MedicationOrders)
            .Where(p => p.AssignedPharmacyId == pharmacyId && p.Status == PrescriptionStatus.Open)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Prescription>> GetExpiredPrescriptionsAsync(DateTime referenceDate, CancellationToken cancellationToken = default)
    {
        // Finder åbne recepter, der er over 2 år gamle.
        var expiryDate = referenceDate.AddYears(-2);
        return await _dbContext.Prescriptions
            .Where(p => p.Status == PrescriptionStatus.Open && p.CreatedAt < expiryDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Prescription>> GetOpenByCprAsync(string cprNumber, CancellationToken cancellationToken = default)
    {
        // Filtrerer på CPR og kun åbne recepter.
        return await _dbContext.Prescriptions
            .Include(p => p.MedicationOrders)
            .Where(p => p.Patient.CprNumber == cprNumber && p.Status == PrescriptionStatus.Open)
            .ToListAsync(cancellationToken);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
        _dbContext.SaveChangesAsync(cancellationToken);
}
