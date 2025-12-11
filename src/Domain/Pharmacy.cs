namespace ReceptServer.Domain;

public sealed class Pharmacy
{
    public Pharmacy(Guid pharmacyId, string name, string address)
    {
        // Validerer at id, navn og adresse er udfyldt.
        PharmacyId = pharmacyId != Guid.Empty
            ? pharmacyId
            : throw new ArgumentException("PharmacyId must be provided.", nameof(pharmacyId));

        Name = !string.IsNullOrWhiteSpace(name)
            ? name.Trim()
            : throw new ArgumentException("Name is required.", nameof(name));

        Address = !string.IsNullOrWhiteSpace(address)
            ? address.Trim()
            : throw new ArgumentException("Address is required.", nameof(address));
    }

    public Guid PharmacyId { get; }
    public string Name { get; }
    public string Address { get; }
}
