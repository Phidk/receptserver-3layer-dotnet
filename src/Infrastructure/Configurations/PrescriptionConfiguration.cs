using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ReceptServer.Domain;

namespace ReceptServer.Infrastructure.Configurations;

public sealed class PrescriptionConfiguration : IEntityTypeConfiguration<Prescription>
{
    public void Configure(EntityTypeBuilder<Prescription> builder)
    {
        builder.ToTable("Prescriptions");

        builder.HasKey(p => p.PrescriptionId);
        builder.Property(p => p.CreatedAt).IsRequired();
        builder.Property(p => p.Status).IsRequired();
        builder.Property(p => p.AssignedPharmacyId).IsRequired(false);
        builder.Property(p => p.ClosedAt).IsRequired(false);

        // Patient modelleres som ejet type, lagres på samme tabel.
        builder.OwnsOne(p => p.Patient, owned =>
        {
            owned.Property(x => x.CprNumber).HasColumnName("PatientCprNumber").IsRequired().HasMaxLength(10);
            owned.Property(x => x.FullName).HasColumnName("PatientFullName").IsRequired().HasMaxLength(200);
            owned.HasData(SeedData.PrescriptionPatients);
        });

        // Støtter private backing field for ordinationer.
        builder.Navigation(nameof(Prescription.MedicationOrders)).UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.HasMany(p => p.MedicationOrders)
            .WithOne()
            .HasForeignKey("PrescriptionId");

        builder.HasData(SeedData.Prescriptions);
    }
}
