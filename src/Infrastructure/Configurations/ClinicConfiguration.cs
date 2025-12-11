using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ReceptServer.Domain;

namespace ReceptServer.Infrastructure.Configurations;

public sealed class ClinicConfiguration : IEntityTypeConfiguration<Clinic>
{
    public void Configure(EntityTypeBuilder<Clinic> builder)
    {
        builder.ToTable("Clinics");

        builder.HasKey(c => c.ClinicId);
        builder.Property(c => c.ProviderNumber).IsRequired().HasMaxLength(32);
        builder.Property(c => c.Name).IsRequired().HasMaxLength(200);
        builder.Property(c => c.Address).IsRequired().HasMaxLength(300);
        builder.HasIndex(c => c.ProviderNumber).IsUnique();

        builder.HasData(SeedData.Clinics);
    }
}
