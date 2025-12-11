using ReceptServer.Domain;

namespace ReceptServer.Infrastructure;

internal static class SeedData
{
    // Realistic GUIDs and Names for Clinics
    public static readonly Clinic[] Clinics =
    [
        new(new Guid("d83b4c1a-5e2f-4b6a-9c8d-1e2f3a4b5c6d"), "1001", "Lægehuset v/ Torvet", "Torvet 4, 8000 Aarhus C"),
        new(new Guid("a1b2c3d4-e5f6-4a5b-8c9d-0e1f2a3b4c5d"), "1002", "Citylægerne", "Park Allé 15, 8000 Aarhus C"),
        new(new Guid("f1e2d3c4-b5a6-4978-9012-345678901234"), "1003", "Lægeklinikken Åboulevarden", "Åboulevarden 30, 8000 Aarhus C")
    ];

    // Realistic GUIDs and Names for Pharmacies
    public static readonly Pharmacy[] Pharmacies =
    [
        new(new Guid("45a1b2c3-d4e5-4f6a-8b9c-0d1e2f3a4b5c"), "Aarhus Løve Apotek", "Store Torv 5, 8000 Aarhus C"),
        new(new Guid("67b8c9d0-e1f2-4a3b-5c6d-7e8f9a0b1c2d"), "Viby Apotek", "Viby Ringvej 10, 8260 Viby J"),
        new(new Guid("12c3d4e5-f6a7-4b8c-9d0e-1f2a3b4c5d6e"), "Risskov Apotek", "Nordre Strandvej 40, 8240 Risskov")
    ];

    // Prescriptions with linked GUIDs
    public static readonly object[] Prescriptions =
    [
        // Active Prescription for Sofie Madsen
        new
        {
            PrescriptionId = new Guid("b0a0a0a0-b0b0-c0c0-d0d0-e0e0e0e0e0e0"),
            ClinicId = Clinics[0].ClinicId, // Lægehuset v/ Torvet
            CreatedAt = new DateTime(2024, 1, 15, 10, 30, 0, DateTimeKind.Utc),
            Status = PrescriptionStatus.Open,
            AssignedPharmacyId = (Guid?)Pharmacies[0].PharmacyId, // Assigned to Løve Apotek
            ClosedAt = (DateTime?)null
        },
        // Active Prescription for Lars Nielsen
        new
        {
            PrescriptionId = new Guid("f0f0f0f0-1010-2020-3030-404040404040"),
            ClinicId = Clinics[1].ClinicId, // Citylægerne
            CreatedAt = new DateTime(2024, 2, 20, 14, 15, 0, DateTimeKind.Utc),
            Status = PrescriptionStatus.Open,
            AssignedPharmacyId = (Guid?)null,
            ClosedAt = (DateTime?)null
        },
        // Expired Prescription for Gerda Olesen
        new
        {
            PrescriptionId = new Guid("a0a0a0a0-1111-2222-3333-444444444444"),
            ClinicId = Clinics[2].ClinicId, // Åboulevarden
            CreatedAt = new DateTime(2021, 5, 10, 09, 00, 0, DateTimeKind.Utc), // Expired (> 2 years)
            Status = PrescriptionStatus.Open,
            AssignedPharmacyId = (Guid?)null,
            ClosedAt = (DateTime?)null
        },
        // Active Prescription for Henrik Poulsen
        new
        {
            PrescriptionId = new Guid("c0c0c0c0-5555-6666-7777-888888888888"),
            ClinicId = Clinics[0].ClinicId, // Lægehuset v/ Torvet
            CreatedAt = new DateTime(2024, 3, 5, 11, 45, 0, DateTimeKind.Utc),
            Status = PrescriptionStatus.Open,
            AssignedPharmacyId = (Guid?)Pharmacies[1].PharmacyId, // Assigned to Viby Apotek
            ClosedAt = (DateTime?)null
        }
    ];

    // Mapping Patients to Prescriptions
    public static readonly object[] PrescriptionPatients =
    [
        new { PrescriptionId = new Guid("b0a0a0a0-b0b0-c0c0-d0d0-e0e0e0e0e0e0"), CprNumber = "1205851234", FullName = "Sofie Madsen" },
        new { PrescriptionId = new Guid("f0f0f0f0-1010-2020-3030-404040404040"), CprNumber = "2408905678", FullName = "Lars Nielsen" },
        new { PrescriptionId = new Guid("a0a0a0a0-1111-2222-3333-444444444444"), CprNumber = "0101409999", FullName = "Gerda Olesen" },
        new { PrescriptionId = new Guid("c0c0c0c0-5555-6666-7777-888888888888"), CprNumber = "1506758888", FullName = "Henrik Poulsen" }
    ];

    // Medication Orders linked to Prescriptions
    public static readonly object[] MedicationOrders =
    [
        // Sofie Madsen: Panodil
        new { MedicationOrderId = new Guid("d1d1d1d1-1212-3434-5656-787878787878"), DrugName = "Panodil", Dosage = "1g x 4 dagligt", AllowedDispensations = 5, DispensedCount = 1, PrescriptionId = new Guid("b0a0a0a0-b0b0-c0c0-d0d0-e0e0e0e0e0e0") },
        
        // Lars Nielsen: Ibuprofen & Penicillin
        new { MedicationOrderId = new Guid("e2e2e2e2-2323-4545-6767-898989898989"), DrugName = "Ibuprofen", Dosage = "400mg ved smerter", AllowedDispensations = 3, DispensedCount = 0, PrescriptionId = new Guid("f0f0f0f0-1010-2020-3030-404040404040") },
        new { MedicationOrderId = new Guid("f3f3f3f3-3434-5656-7878-909090909090"), DrugName = "Penicillin", Dosage = "1 tablet x 3 i 7 dage", AllowedDispensations = 1, DispensedCount = 0, PrescriptionId = new Guid("f0f0f0f0-1010-2020-3030-404040404040") },
        
        // Gerda Olesen: Hjertemedicin (Expired)
        new { MedicationOrderId = new Guid("a4a4a4a4-4545-6767-8989-010101010101"), DrugName = "Metoprolol", Dosage = "50mg morgen", AllowedDispensations = 10, DispensedCount = 2, PrescriptionId = new Guid("a0a0a0a0-1111-2222-3333-444444444444") },
        
        // Henrik Poulsen: Insulin
        new { MedicationOrderId = new Guid("b5b5b5b5-5656-7878-9090-121212121212"), DrugName = "Insulin Novorapid", Dosage = "efter skema", AllowedDispensations = 12, DispensedCount = 3, PrescriptionId = new Guid("c0c0c0c0-5555-6666-7777-888888888888") }
    ];
}