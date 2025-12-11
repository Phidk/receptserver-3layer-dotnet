namespace ReceptServer.Application;

public sealed class MedicationOrderRequest
{
    public string DrugName { get; init; } = string.Empty;
    public string Dosage { get; init; } = string.Empty;
    public int AllowedDispensations { get; init; }
}
