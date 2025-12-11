using System.ComponentModel.DataAnnotations;

namespace ReceptServer.Api.Contracts;

public sealed class DispenseRequest
{
    [Required]
    public Guid MedicationOrderId { get; init; }
}
