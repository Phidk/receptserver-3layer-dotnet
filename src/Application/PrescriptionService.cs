using System.Text.RegularExpressions;
using ReceptServer.Domain;

namespace ReceptServer.Application;

public sealed class PrescriptionService
{
    private readonly IPrescriptionRepository _repository;
    private readonly IClinicRepository _clinicRepository;
    private static readonly Regex ProviderNumberPattern = new(@"^\d{1,10}$", RegexOptions.Compiled);
    private static readonly Regex CprPattern = new(@"^\d{10}$", RegexOptions.Compiled);

    public PrescriptionService(IPrescriptionRepository repository, IClinicRepository clinicRepository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _clinicRepository = clinicRepository ?? throw new ArgumentNullException(nameof(clinicRepository));
    }

    public async Task<Prescription> CreatePrescriptionAsync(CreatePrescriptionRequest request, CancellationToken cancellationToken = default)
    {
        if (request is null) throw new ArgumentNullException(nameof(request));

        // Validerer input: ydernummer og CPR skal overholde formater.
        ValidateProviderNumber(request.ProviderNumber);
        ValidateCpr(request.PatientCprNumber);
        if (request.ClinicId == Guid.Empty) throw new ArgumentException("ClinicId is required.", nameof(request));
        if (string.IsNullOrWhiteSpace(request.PatientFullName)) throw new ArgumentException("Patient name is required.", nameof(request));

        // Bekræft at klinikken findes og at ydernummer matcher det registrerede.
        var clinic = await _clinicRepository.GetByIdAsync(request.ClinicId, cancellationToken)
            ?? throw new InvalidOperationException("Clinic not found.");
        if (!string.Equals(clinic.ProviderNumber, request.ProviderNumber.Trim(), StringComparison.Ordinal))
        {
            throw new InvalidOperationException("Clinic provider number does not match.");
        }

        // Bygger domæneobjekter for ordinationer og patient.
        var medicationOrders = BuildOrders(request.MedicationOrders);
        var patient = new Patient(request.PatientCprNumber, request.PatientFullName);

        // Opretter recepten og gemmer den i databasen.
        var prescription = new Prescription(Guid.NewGuid(), request.ClinicId, patient, medicationOrders);
        await _repository.AddAsync(prescription, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);
        return prescription;
    }

    public async Task<IReadOnlyList<Prescription>> GetAssignedToPharmacyAsync(Guid pharmacyId, CancellationToken cancellationToken = default)
    {
        if (pharmacyId == Guid.Empty) throw new ArgumentException("PharmacyId is required.", nameof(pharmacyId));
        return await _repository.GetOpenByAssignedPharmacyAsync(pharmacyId, cancellationToken);
    }

    public async Task<IReadOnlyList<Prescription>> SearchOpenByCprAsync(string cprNumber, CancellationToken cancellationToken = default)
    {
        ValidateCpr(cprNumber);
        return await _repository.GetOpenByCprAsync(cprNumber.Trim(), cancellationToken);
    }

    public async Task DispenseAsync(Guid prescriptionId, Guid medicationOrderId, CancellationToken cancellationToken = default)
    {
        if (prescriptionId == Guid.Empty) throw new ArgumentException("PrescriptionId is required.", nameof(prescriptionId));
        if (medicationOrderId == Guid.Empty) throw new ArgumentException("MedicationOrderId is required.", nameof(medicationOrderId));

        // Henter recepten fra databasen inklusiv ordinationer.
        var prescription = await _repository.GetByIdAsync(prescriptionId, cancellationToken)
            ?? throw new InvalidOperationException("Prescription not found.");

        if (prescription.IsClosed)
        {
            throw new InvalidOperationException("Prescription is closed.");
        }

        // Finder den specifikke ordination på recepten.
        var order = prescription.MedicationOrders.FirstOrDefault(o => o.MedicationOrderId == medicationOrderId)
            ?? throw new InvalidOperationException("Medication order not found on prescription.");

        // Forretningsregel: kan kun udlevere på en åben recept og hvis der er udleveringer tilbage.
        order.RegisterDispense();
        // Luk hvis alle ordinationer er færdigudleveret.
        prescription.CloseIfFullyDispensed();

        await _repository.SaveChangesAsync(cancellationToken);
    }

    public async Task<int> CloseExpiredPrescriptionsAsync(CancellationToken cancellationToken = default)
    {
        var referenceDate = DateTime.UtcNow;
        // Finder alle recepter der er udløbet (ældre end 2 år).
        var expiredPrescriptions = await _repository.GetExpiredPrescriptionsAsync(referenceDate, cancellationToken);

        if (expiredPrescriptions == null || !expiredPrescriptions.Any())
        {
            return 0;
        }

        // Lukker recepterne.
        foreach (var prescription in expiredPrescriptions)
        {
            prescription.CloseIfExpired(referenceDate);
        }

        await _repository.SaveChangesAsync(cancellationToken);
        return expiredPrescriptions.Count;
    }

    private static IReadOnlyCollection<MedicationOrder> BuildOrders(IReadOnlyCollection<MedicationOrderRequest> requests)
    {
        if (requests == null || requests.Count == 0)
        {
            throw new ArgumentException("At least one medication order is required.", nameof(requests));
        }

        var orders = new List<MedicationOrder>();
        foreach (var request in requests)
        {
            if (string.IsNullOrWhiteSpace(request.DrugName))
            {
                throw new ArgumentException("DrugName is required on medication order.");
            }

            if (string.IsNullOrWhiteSpace(request.Dosage))
            {
                throw new ArgumentException("Dosage is required on medication order.");
            }

            if (request.AllowedDispensations <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(request.AllowedDispensations), "AllowedDispensations must be positive.");
            }

            orders.Add(new MedicationOrder(Guid.NewGuid(), request.DrugName, request.Dosage, request.AllowedDispensations));
        }

        return orders;
    }

    private static void ValidateProviderNumber(string providerNumber)
    {
        if (string.IsNullOrWhiteSpace(providerNumber))
        {
            throw new ArgumentException("ProviderNumber is required.", nameof(providerNumber));
        }

        var normalized = providerNumber.Trim();
        if (!ProviderNumberPattern.IsMatch(normalized))
        {
            throw new ArgumentException("ProviderNumber skal være 1-10 cifre.", nameof(providerNumber));
        }
    }

    private static void ValidateCpr(string cprNumber)
    {
        if (string.IsNullOrWhiteSpace(cprNumber))
        {
            throw new ArgumentException("CprNumber is required.", nameof(cprNumber));
        }

        var normalized = cprNumber.Trim();
        if (!CprPattern.IsMatch(normalized))
        {
            throw new ArgumentException("CprNumber skal være præcis 10 cifre.", nameof(cprNumber));
        }
    }
}
