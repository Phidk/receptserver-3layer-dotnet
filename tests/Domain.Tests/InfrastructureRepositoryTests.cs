using Microsoft.EntityFrameworkCore;
using ReceptServer.Application;
using ReceptServer.Domain;
using ReceptServer.Infrastructure;
using ReceptServer.Infrastructure.Repositories;

namespace Domain.Tests;

// Tester infrastrukturlaget (Repositories) op mod en InMemory EF Core database.
// Formålet er at sikre integrationen mellem repositories og databasen virker som forventet.
public class InfrastructureRepositoryTests
{
    [Fact]
    // Bekræfter at seeded data (data der oprettes ved opstart) kan læses korrekt op fra databasen via CPR.
    public async Task SeededData_CanBeReadByCpr()
    {
        using var context = CreateContext();
        var repository = new PrescriptionRepository(context);

        var results = await repository.GetOpenByCprAsync("1205851234");

        Assert.NotEmpty(results);
        Assert.Equal("Sofie Madsen", results.First().Patient.FullName);
    }

    [Fact]
    // Tester hele flowet fra oprettelse til hentning.
    // 1. Opretter en recept via PrescriptionService.
    // 2. Henter recepten igen via PrescriptionRepository.
    // 3. Verificerer at dataene stemmer overens (round-trip).
    public async Task CreateAndFetchPrescription_RoundTripsViaRepository()
    {
        using var context = CreateContext();
        var repository = new PrescriptionRepository(context);
        var clinicId = Guid.NewGuid();
        var clinic = new Clinic(clinicId, "12345", "Test Clinic", "Adresse 1");
        context.Clinics.Add(clinic);
        await context.SaveChangesAsync();
        var clinicRepository = new ClinicRepository(context);
        var service = new PrescriptionService(repository, clinicRepository);

        var created = await service.CreatePrescriptionAsync(new CreatePrescriptionRequest
        {
            ClinicId = clinicId,
            ProviderNumber = "12345",
            PatientCprNumber = "5555555555",
            PatientFullName = "Test Bruger",
            MedicationOrders = new[]
            {
                new MedicationOrderRequest
                {
                    DrugName = "Panodil",
                    Dosage = "1 tablet",
                    AllowedDispensations = 1
                }
            }
        });

        var fetched = await repository.GetByIdAsync(created.PrescriptionId);

        Assert.NotNull(fetched);
        Assert.Equal("5555555555", fetched!.Patient.CprNumber);
        Assert.Single(fetched.MedicationOrders);
    }

    // Hjælpemetode der opretter en InMemory database instans til hver test.
    // Det sikrer at hver test kører isoleret fra de andre.
    private static ReceptDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<ReceptDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        var context = new ReceptDbContext(options);
        context.Database.EnsureCreated();
        return context;
    }
}
