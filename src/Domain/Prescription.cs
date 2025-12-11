namespace ReceptServer.Domain;

public sealed class Prescription
{
    private readonly List<MedicationOrder> _medicationOrders;

    // Parameterløs ctor til EF Core.
    private Prescription()
    {
        _medicationOrders = new List<MedicationOrder>();
    }

    public Prescription(
        Guid prescriptionId,
        Guid clinicId,
        Patient patient,
        IEnumerable<MedicationOrder> medicationOrders,
        DateTime? createdAt = null,
        Guid? assignedPharmacyId = null)
    {
        PrescriptionId = prescriptionId != Guid.Empty
            ? prescriptionId
            : throw new ArgumentException("PrescriptionId must be provided.", nameof(prescriptionId));

        ClinicId = clinicId != Guid.Empty
            ? clinicId
            : throw new ArgumentException("ClinicId must be provided.", nameof(clinicId));

        Patient = patient ?? throw new ArgumentNullException(nameof(patient));
        AssignedPharmacyId = assignedPharmacyId;

        _medicationOrders = medicationOrders?.ToList() ?? throw new ArgumentNullException(nameof(medicationOrders));
        if (_medicationOrders.Count == 0)
        {
            throw new ArgumentException("A prescription must contain at least one medication order.", nameof(medicationOrders));
        }

        CreatedAt = createdAt?.ToUniversalTime() ?? DateTime.UtcNow;
        Status = PrescriptionStatus.Open;
    }

    public Guid PrescriptionId { get; private set; }
    public Guid ClinicId { get; private set; }
    public Patient Patient { get; private set; } = null!;
    public IReadOnlyCollection<MedicationOrder> MedicationOrders => _medicationOrders.AsReadOnly();
    public DateTime CreatedAt { get; private set; }
    public PrescriptionStatus Status { get; private set; }
    public Guid? AssignedPharmacyId { get; private set; }
    public DateTime? ClosedAt { get; private set; }
    public bool IsOpen => Status == PrescriptionStatus.Open;
    public bool IsClosed => Status == PrescriptionStatus.Closed;

    // Tillader at binde/afbinde et foretrukket apotek.
    public void AssignPharmacy(Guid? pharmacyId)
    {
        AssignedPharmacyId = pharmacyId;
    }

    // Lukker recepten, hvis alle ordinationer er helt udleverede.
    public void CloseIfFullyDispensed(DateTime? closedAt = null)
    {
        if (IsClosed)
        {
            return;
        }

        if (_medicationOrders.All(order => !order.CanDispense()))
        {
            Status = PrescriptionStatus.Closed;
            ClosedAt = closedAt ?? DateTime.UtcNow;
        }
    }

    // Lukker recepten, hvis den er ældre end 2 år på reference-datoen.
    public void CloseIfExpired(DateTime referenceDate)
    {
        if (IsClosed)
        {
            return;
        }

        if (referenceDate >= CreatedAt.AddYears(2))
        {
            Status = PrescriptionStatus.Closed;
            ClosedAt = referenceDate;
        }
    }
}
