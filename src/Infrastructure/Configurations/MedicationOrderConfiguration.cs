using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ReceptServer.Domain;

namespace ReceptServer.Infrastructure.Configurations;

public sealed class MedicationOrderConfiguration : IEntityTypeConfiguration<MedicationOrder>
{
    public void Configure(EntityTypeBuilder<MedicationOrder> builder)
    {
        builder.ToTable("MedicationOrders");

        builder.HasKey(o => o.MedicationOrderId);
        builder.Property<Guid>("PrescriptionId");
        builder.Property(o => o.DrugName).IsRequired().HasMaxLength(200);
        builder.Property(o => o.Dosage).IsRequired().HasMaxLength(200);
        builder.Property(o => o.AllowedDispensations).IsRequired();
        builder.Property(o => o.DispensedCount).IsRequired();

        builder.HasData(SeedData.MedicationOrders);
    }
}
