namespace ReceptServer.Application;

public sealed class CreatePrescriptionRequest
{
    public Guid ClinicId { get; init; }
    public string ProviderNumber { get; init; } = string.Empty;
    public string PatientCprNumber { get; init; } = string.Empty;
    public string PatientFullName { get; init; } = string.Empty;
    public IReadOnlyCollection<MedicationOrderRequest> MedicationOrders { get; init; } = Array.Empty<MedicationOrderRequest>();
}
