using Microsoft.EntityFrameworkCore;
using ReceptServer.Domain;

namespace ReceptServer.Infrastructure;

public sealed class ReceptDbContext : DbContext
{
    public ReceptDbContext(DbContextOptions<ReceptDbContext> options) : base(options) { }

    public DbSet<Clinic> Clinics => Set<Clinic>();
    public DbSet<Pharmacy> Pharmacies => Set<Pharmacy>();
    public DbSet<Prescription> Prescriptions => Set<Prescription>();
    public DbSet<MedicationOrder> MedicationOrders => Set<MedicationOrder>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ReceptDbContext).Assembly);
    }
}
