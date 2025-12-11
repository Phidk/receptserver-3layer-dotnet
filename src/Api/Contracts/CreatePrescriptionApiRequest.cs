using System.ComponentModel.DataAnnotations;
using ReceptServer.Application;

namespace ReceptServer.Api.Contracts;

public sealed class CreatePrescriptionApiRequest
{
    [Required]
    public Guid ClinicId { get; init; }

    [Required]
    public string ProviderNumber { get; init; } = string.Empty;

    [Required]
    public string PatientCprNumber { get; init; } = string.Empty;

    [Required]
    public string PatientFullName { get; init; } = string.Empty;

    [MinLength(1)]
    public IReadOnlyCollection<MedicationOrderRequest> MedicationOrders { get; init; } = Array.Empty<MedicationOrderRequest>();
}
