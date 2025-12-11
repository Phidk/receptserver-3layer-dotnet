namespace ReceptServer.Api.Contracts;

public sealed class PrescriptionDto
{
    public Guid PrescriptionId { get; init; }
    public Guid ClinicId { get; init; }
    public string PatientCprNumber { get; init; } = string.Empty;
    public string PatientFullName { get; init; } = string.Empty;
    public string? AssignedPharmacyId { get; init; }
    public DateTime CreatedAt { get; init; }
    public string Status { get; init; } = string.Empty;
    public IReadOnlyCollection<MedicationOrderDto> MedicationOrders { get; init; } = Array.Empty<MedicationOrderDto>();
}

public sealed class MedicationOrderDto
{
    public Guid MedicationOrderId { get; init; }
    public string DrugName { get; init; } = string.Empty;
    public string Dosage { get; init; } = string.Empty;
    public int AllowedDispensations { get; init; }
    public int DispensedCount { get; init; }
    public bool CanDispense { get; init; }
}
