using ReceptServer.Application;
using ReceptServer.Domain;

namespace Domain.Tests;

// Tester forretningslogik i PrescriptionService.
// Bruger InMemory repositories for at undgå afhængighed af en rigtig database.
public class PrescriptionServiceTests
{
    private readonly InMemoryPrescriptionRepository _repository = new();
    private readonly InMemoryClinicRepository _clinicRepository = new();
    private readonly PrescriptionService _service;

    public PrescriptionServiceTests()
    {
        _service = new PrescriptionService(_repository, _clinicRepository);
    }

    [Fact]
    // Bekræfter at en recept kan oprettes med gyldige data, og at status sættes korrekt til 'Open'.
    public async Task CreatePrescription_SavesAndValidates()
    {
        var request = BuildRequest("1234567890", providerNumber: "123456", drugName: "Panodil");

        var prescription = await _service.CreatePrescriptionAsync(request);

        Assert.Single(_repository.Items);
        Assert.Equal(PrescriptionStatus.Open, prescription.Status);
        Assert.Equal("1234567890", prescription.Patient.CprNumber);
    }

    [Fact]
    // Bekræfter at søgning på CPR kun returnerer åbne recepter, og filtrerer lukkede/udløbne fra.
    public async Task SearchOpenByCprAsync_ReturnsOnlyOpenPrescriptions()
    {
        var cpr = "2222222222";
        var open = await _service.CreatePrescriptionAsync(BuildRequest(cpr));
        var closed = await _service.CreatePrescriptionAsync(BuildRequest(cpr));
        // Simulerer at den lukkede recept manuelt lukkes eller udløber (f.eks. ved admin job).
        closed.CloseIfExpired(DateTime.UtcNow.AddYears(3));

        var results = await _service.SearchOpenByCprAsync(cpr);

        Assert.Contains(open, results);
        Assert.DoesNotContain(closed, results);
    }

    [Fact]
    // Bekræfter at en udlevering opdaterer tælleren på ordinationen, og at recepten lukkes automatisk
    // når alle ordinationer er færdigudleveret.
    public async Task DispenseAsync_RegistersDispenseAndClosesWhenDone()
    {
        var prescription = await _service.CreatePrescriptionAsync(BuildRequest("3333333333"));
        var order = prescription.MedicationOrders.Single();

        await _service.DispenseAsync(prescription.PrescriptionId, order.MedicationOrderId);

        Assert.Equal(1, order.DispensedCount);
        Assert.Equal(PrescriptionStatus.Closed, prescription.Status);
    }

    [Fact]
    // Tester validering: Et ugyldigt ydernummer (f.eks. bogstaver) skal kaste en fejl.
    public async Task CreatePrescription_InvalidProviderNumberThrows()
    {
        var request = new CreatePrescriptionRequest
        {
            ClinicId = Guid.NewGuid(),
            ProviderNumber = "abc",
            PatientCprNumber = "4444444444",
            PatientFullName = "Test Patient",
            MedicationOrders = new[]
            {
                new MedicationOrderRequest
                {
                    DrugName = "Ibuprofen",
                    Dosage = "1 tablet",
                    AllowedDispensations = 1
                }
            }
        };
        await Assert.ThrowsAsync<ArgumentException>(() => _service.CreatePrescriptionAsync(request));
    }

    [Fact]
    // Tester sikkerhed: Hvis ydernummeret i requestet ikke matcher det ydernummer, der er registreret på klinikken (ClinicId), 
    // skal systemet afvise oprettelsen.
    public async Task CreatePrescription_UnknownClinicThrows()
    {
        var request = new CreatePrescriptionRequest
        {
            ClinicId = Guid.NewGuid(),
            ProviderNumber = "9999",
            PatientCprNumber = "5555555555",
            PatientFullName = "Test Patient",
            MedicationOrders = new[]
            {
                new MedicationOrderRequest
                {
                    DrugName = "Ibuprofen",
                    Dosage = "1 tablet",
                    AllowedDispensations = 1
                }
            }
        };

        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CreatePrescriptionAsync(request));
    }

    // Hjælpemetode til at bygge et gyldigt request objekt hurtigt.
    private CreatePrescriptionRequest BuildRequest(string cpr, string providerNumber = "987654", string drugName = "Ibuprofen")
    {
        var clinicId = Guid.NewGuid();
        // Sørger for at klinikken eksisterer i vores in-memory repository før testen kører.
        _clinicRepository.Add(new Clinic(clinicId, providerNumber, "Test Clinic", "Testvej 1"));

        return new CreatePrescriptionRequest
        {
            ClinicId = clinicId,
            ProviderNumber = providerNumber,
            PatientCprNumber = cpr,
            PatientFullName = "Test Patient",
            MedicationOrders = new[]
            {
                new MedicationOrderRequest
                {
                    DrugName = drugName,
                    Dosage = "1 tablet",
                    AllowedDispensations = 1
                }
            }
        };
    }

    // Simpel in-memory implementering af IPrescriptionRepository til testformål.
    // Gemmer recepter i en liste i stedet for en database.
    private sealed class InMemoryPrescriptionRepository : IPrescriptionRepository
    {
        public List<Prescription> Items { get; } = new();

        public Task AddAsync(Prescription prescription, CancellationToken cancellationToken = default)
        {
            Items.Add(prescription);
            return Task.CompletedTask;
        }

        public Task<Prescription?> GetByIdAsync(Guid prescriptionId, CancellationToken cancellationToken = default)
        {
            var match = Items.FirstOrDefault(p => p.PrescriptionId == prescriptionId);
            return Task.FromResult(match);
        }

        public Task<IReadOnlyList<Prescription>> GetOpenByCprAsync(string cprNumber, CancellationToken cancellationToken = default)
        {
            var matches = Items.Where(p => p.Patient.CprNumber == cprNumber && p.IsOpen).ToList();
            return Task.FromResult<IReadOnlyList<Prescription>>(matches);
        }

        public Task<IReadOnlyList<Prescription>> GetOpenByAssignedPharmacyAsync(Guid pharmacyId, CancellationToken cancellationToken = default)
        {
            var matches = Items.Where(p => p.IsOpen && p.AssignedPharmacyId == pharmacyId).ToList();
            return Task.FromResult<IReadOnlyList<Prescription>>(matches);
        }

        public Task<IReadOnlyList<Prescription>> GetExpiredPrescriptionsAsync(DateTime referenceDate, CancellationToken cancellationToken = default)
        {
            // Recepten er udløbet hvis den er mere end 2 år gammel i forhold til referenceDate
            var matches = Items.Where(p => p.IsOpen && referenceDate >= p.CreatedAt.AddYears(2)).ToList();
            return Task.FromResult<IReadOnlyList<Prescription>>(matches);
        }

        public Task SaveChangesAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
    }

    // Simpel in-memory implementering af IClinicRepository.
    private sealed class InMemoryClinicRepository : IClinicRepository
    {
        private readonly List<Clinic> _clinics = new();

        public void Add(Clinic clinic) => _clinics.Add(clinic);

        public Task<Clinic?> GetByIdAsync(Guid clinicId, CancellationToken cancellationToken = default)
        {
            var match = _clinics.FirstOrDefault(c => c.ClinicId == clinicId);
            return Task.FromResult(match);
        }

        public Task<Clinic?> GetByProviderNumberAsync(string providerNumber, CancellationToken cancellationToken = default)
        {
            var match = _clinics.FirstOrDefault(c => c.ProviderNumber == providerNumber);
            return Task.FromResult(match);
        }
    }
}
