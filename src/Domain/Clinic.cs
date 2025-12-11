namespace ReceptServer.Domain;

public sealed class Clinic
{
    public Clinic(Guid clinicId, string providerNumber, string name, string address)
    {
        // Validerer at id, ydernummer, navn og adresse er udfyldt.
        ClinicId = clinicId != Guid.Empty
            ? clinicId
            : throw new ArgumentException("ClinicId must be provided.", nameof(clinicId));

        ProviderNumber = !string.IsNullOrWhiteSpace(providerNumber)
            ? providerNumber.Trim()
            : throw new ArgumentException("ProviderNumber is required.", nameof(providerNumber));

        Name = !string.IsNullOrWhiteSpace(name)
            ? name.Trim()
            : throw new ArgumentException("Name is required.", nameof(name));

        Address = !string.IsNullOrWhiteSpace(address)
            ? address.Trim()
            : throw new ArgumentException("Address is required.", nameof(address));
    }

    public Guid ClinicId { get; }
    public string ProviderNumber { get; }
    public string Name { get; }
    public string Address { get; }
}
