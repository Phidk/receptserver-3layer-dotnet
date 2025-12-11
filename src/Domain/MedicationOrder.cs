namespace ReceptServer.Domain;

public sealed class MedicationOrder
{
    public MedicationOrder(Guid medicationOrderId, string drugName, string dosage, int allowedDispensations, int dispensedCount = 0)
    {
        MedicationOrderId = medicationOrderId != Guid.Empty
            ? medicationOrderId
            : throw new ArgumentException("MedicationOrderId must be provided.", nameof(medicationOrderId));

        DrugName = !string.IsNullOrWhiteSpace(drugName)
            ? drugName.Trim()
            : throw new ArgumentException("DrugName is required.", nameof(drugName));

        Dosage = !string.IsNullOrWhiteSpace(dosage)
            ? dosage.Trim()
            : throw new ArgumentException("Dosage is required.", nameof(dosage));

        if (allowedDispensations <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(allowedDispensations), "AllowedDispensations must be positive.");
        }

        if (dispensedCount < 0 || dispensedCount > allowedDispensations)
        {
            throw new ArgumentOutOfRangeException(nameof(dispensedCount), "DispensedCount must be between 0 and the allowed dispensations.");
        }

        AllowedDispensations = allowedDispensations;
        DispensedCount = dispensedCount;
    }

    public Guid MedicationOrderId { get; }
    public string DrugName { get; }
    public string Dosage { get; }
    public int AllowedDispensations { get; }
    public int DispensedCount { get; private set; }

    // Kan kun udleveres hvis dispensationer er under tilladt antal.
    public bool CanDispense() => DispensedCount < AllowedDispensations;

    // Registrerer en udlevering og stopper hvis kvoten er brugt.
    public void RegisterDispense()
    {
        if (!CanDispense())
        {
            throw new InvalidOperationException("Medication order has already been fully dispensed.");
        }

        DispensedCount += 1;
    }
}
