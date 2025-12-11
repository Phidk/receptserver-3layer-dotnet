using ReceptServer.Domain;

namespace Domain.Tests;

// Tester forretningsregler direkte på domæneobjekterne (Unit Tests)
// Fokus er på logik indkapslet i Prescription og MedicationOrder klasserne.
public class DomainRuleTests
{
    [Fact]
    // Bekræfter at man ikke kan udlevere mere end det tilladte antal gange.
    // Hvis man prøver at udlevere ud over grænsen, skal systemet throw en error.
    public void MedicationOrder_RegisterDispense_IncrementsUntilLimitAndThenThrows()
    {
        var order = new MedicationOrder(Guid.NewGuid(), "Panodil", "1 tablet", 2);

        order.RegisterDispense();
        order.RegisterDispense();

        Assert.Equal(2, order.DispensedCount);
        Assert.False(order.CanDispense());
        Assert.Throws<InvalidOperationException>(() => order.RegisterDispense());
    }

    [Fact]
    // Bekræfter at en recept lukkes automatisk, når ALLE dens ordinationer er fuldt udleveret.
    public void CloseIfFullyDispensed_ClosesWhenAllOrdersAreFulfilled()
    {
        var patient = new Patient("1234567890", "Test Patient");
        var orders = new List<MedicationOrder>
        {
            new(Guid.NewGuid(), "Panodil", "1 tablet", 1),
            new(Guid.NewGuid(), "Ibuprofen", "1 tablet", 1)
        };

        var prescription = new Prescription(Guid.NewGuid(), Guid.NewGuid(), patient, orders, new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Utc));
        orders.ForEach(o => o.RegisterDispense());

        var closedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        prescription.CloseIfFullyDispensed(closedAt);

        Assert.Equal(PrescriptionStatus.Closed, prescription.Status);
        Assert.Equal(closedAt, prescription.ClosedAt);
    }

    [Fact]
    // Bekræfter at en recept forbliver åben, hvis der stadig er ordinationer der mangler udleveringer.
    public void CloseIfFullyDispensed_StaysOpenWhenDispensationsRemain()
    {
        var patient = new Patient("1234567890", "Test Patient");
        var orders = new List<MedicationOrder>
        {
            new(Guid.NewGuid(), "Panodil", "1 tablet", 2, dispensedCount: 1),
            new(Guid.NewGuid(), "Ibuprofen", "1 tablet", 1)
        };

        var prescription = new Prescription(Guid.NewGuid(), Guid.NewGuid(), patient, orders, new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Utc));
        prescription.CloseIfFullyDispensed();

        Assert.Equal(PrescriptionStatus.Open, prescription.Status);
        Assert.Null(prescription.ClosedAt);
    }

    [Fact]
    // Bekræfter at recepter ældre end 2 år lukkes, når man kører udløbslogikken.
    public void CloseIfExpired_ClosesWhenCreatedMoreThanTwoYearsAgo()
    {
        var patient = new Patient("1234567890", "Test Patient");
        var order = new MedicationOrder(Guid.NewGuid(), "Panodil", "1 tablet", 1);

        var createdAt = new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var prescription = new Prescription(Guid.NewGuid(), Guid.NewGuid(), patient, new[] { order }, createdAt);
        var referenceDate = createdAt.AddYears(2).AddDays(1);

        prescription.CloseIfExpired(referenceDate);

        Assert.Equal(PrescriptionStatus.Closed, prescription.Status);
        Assert.Equal(referenceDate, prescription.ClosedAt);
    }

    [Fact]
    // Bekræfter at recepter nyere end 2 år forbliver åbne, når man kører udløbslogikken.
    public void CloseIfExpired_LeavesRecentPrescriptionOpen()
    {
        var patient = new Patient("1234567890", "Test Patient");
        var order = new MedicationOrder(Guid.NewGuid(), "Panodil", "1 tablet", 1);

        var createdAt = new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var prescription = new Prescription(Guid.NewGuid(), Guid.NewGuid(), patient, new[] { order }, createdAt);
        var referenceDate = createdAt.AddYears(2).AddDays(-1);

        prescription.CloseIfExpired(referenceDate);

        Assert.Equal(PrescriptionStatus.Open, prescription.Status);
        Assert.Null(prescription.ClosedAt);
    }

    [Theory]
    [InlineData("")]
    [InlineData("1234")]
    [InlineData("abcdefg123")]
    // Tester at domænet beskytter mod ugyldige CPR-numre ved oprettelse af patienter.
    public void Patient_InvalidCpr_Throws(string invalidCpr)
    {
        Assert.Throws<ArgumentException>(() => new Patient(invalidCpr, "Test Patient"));
    }
}
