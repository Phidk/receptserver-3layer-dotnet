using Microsoft.EntityFrameworkCore;
using ReceptServer.Application;
using ReceptServer.Domain;

namespace ReceptServer.Infrastructure.Repositories;

public sealed class ClinicRepository : IClinicRepository
{
    private readonly ReceptDbContext _dbContext;

    public ClinicRepository(ReceptDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public Task<Clinic?> GetByIdAsync(Guid clinicId, CancellationToken cancellationToken = default) =>
        _dbContext.Clinics.FirstOrDefaultAsync(c => c.ClinicId == clinicId, cancellationToken);

    public Task<Clinic?> GetByProviderNumberAsync(string providerNumber, CancellationToken cancellationToken = default) =>
        _dbContext.Clinics.FirstOrDefaultAsync(c => c.ProviderNumber == providerNumber, cancellationToken);
}
