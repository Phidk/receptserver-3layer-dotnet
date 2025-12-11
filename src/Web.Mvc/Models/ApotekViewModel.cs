namespace Web.Mvc.Models;

public sealed class ApotekViewModel
{
    public string PharmacyName { get; set; } = string.Empty;
    public string? Cpr { get; set; }
    public string? Message { get; set; }
    public string? Error { get; set; }
    public IReadOnlyCollection<PrescriptionDisplayModel> Prescriptions { get; set; } = Array.Empty<PrescriptionDisplayModel>();
}

public sealed class PrescriptionDisplayModel
{
    public Guid PrescriptionId { get; set; }
    public string PatientName { get; set; } = string.Empty;
    public string PatientCpr { get; set; } = string.Empty;
    public IReadOnlyCollection<MedicationOrderDisplayModel> MedicationOrders { get; set; } = Array.Empty<MedicationOrderDisplayModel>();
}

public sealed class MedicationOrderDisplayModel
{
    public Guid MedicationOrderId { get; set; }
    public string DrugName { get; set; } = string.Empty;
    public string Dosage { get; set; } = string.Empty;
    public int AllowedDispensations { get; set; }
    public int DispensedCount { get; set; }
    public bool CanDispense { get; set; }
}
