using System.Text.RegularExpressions;

namespace ReceptServer.Domain;

public sealed class Patient : IEquatable<Patient>
{
    private static readonly Regex CprPattern = new(@"^\d{10}$", RegexOptions.Compiled);

    public Patient(string cprNumber, string fullName)
    {
        // Simpel CPR-validering: præcis 10 cifre og ikke tomt.
        if (string.IsNullOrWhiteSpace(cprNumber))
        {
            throw new ArgumentException("CprNumber is required.", nameof(cprNumber));
        }

        var normalized = cprNumber.Trim();
        if (!CprPattern.IsMatch(normalized))
        {
            throw new ArgumentException("CprNumber must be exactly 10 digits.", nameof(cprNumber));
        }

        if (string.IsNullOrWhiteSpace(fullName))
        {
            throw new ArgumentException("FullName is required.", nameof(fullName));
        }

        CprNumber = normalized;
        FullName = fullName.Trim();
    }

    public string CprNumber { get; }
    public string FullName { get; }

    public bool Equals(Patient? other)
    {
        if (other is null)
        {
            return false;
        }

        return string.Equals(CprNumber, other.CprNumber, StringComparison.Ordinal);
    }

    public override bool Equals(object? obj) => Equals(obj as Patient);

    public override int GetHashCode() => CprNumber.GetHashCode(StringComparison.Ordinal);
}
