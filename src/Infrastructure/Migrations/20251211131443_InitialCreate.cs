using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Clinics",
                columns: table => new
                {
                    ClinicId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ProviderNumber = table.Column<string>(type: "TEXT", maxLength: 32, nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Address = table.Column<string>(type: "TEXT", maxLength: 300, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clinics", x => x.ClinicId);
                });

            migrationBuilder.CreateTable(
                name: "Pharmacies",
                columns: table => new
                {
                    PharmacyId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Address = table.Column<string>(type: "TEXT", maxLength: 300, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pharmacies", x => x.PharmacyId);
                });

            migrationBuilder.CreateTable(
                name: "Prescriptions",
                columns: table => new
                {
                    PrescriptionId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ClinicId = table.Column<Guid>(type: "TEXT", nullable: false),
                    PatientCprNumber = table.Column<string>(type: "TEXT", maxLength: 10, nullable: false),
                    PatientFullName = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    AssignedPharmacyId = table.Column<Guid>(type: "TEXT", nullable: true),
                    ClosedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Prescriptions", x => x.PrescriptionId);
                });

            migrationBuilder.CreateTable(
                name: "MedicationOrders",
                columns: table => new
                {
                    MedicationOrderId = table.Column<Guid>(type: "TEXT", nullable: false),
                    DrugName = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Dosage = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    AllowedDispensations = table.Column<int>(type: "INTEGER", nullable: false),
                    DispensedCount = table.Column<int>(type: "INTEGER", nullable: false),
                    PrescriptionId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MedicationOrders", x => x.MedicationOrderId);
                    table.ForeignKey(
                        name: "FK_MedicationOrders_Prescriptions_PrescriptionId",
                        column: x => x.PrescriptionId,
                        principalTable: "Prescriptions",
                        principalColumn: "PrescriptionId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Clinics",
                columns: new[] { "ClinicId", "Address", "Name", "ProviderNumber" },
                values: new object[,]
                {
                    { new Guid("a1b2c3d4-e5f6-4a5b-8c9d-0e1f2a3b4c5d"), "Park Allé 15, 8000 Aarhus C", "Citylægerne", "1002" },
                    { new Guid("d83b4c1a-5e2f-4b6a-9c8d-1e2f3a4b5c6d"), "Torvet 4, 8000 Aarhus C", "Lægehuset v/ Torvet", "1001" },
                    { new Guid("f1e2d3c4-b5a6-4978-9012-345678901234"), "Åboulevarden 30, 8000 Aarhus C", "Lægeklinikken Åboulevarden", "1003" }
                });

            migrationBuilder.InsertData(
                table: "Pharmacies",
                columns: new[] { "PharmacyId", "Address", "Name" },
                values: new object[,]
                {
                    { new Guid("12c3d4e5-f6a7-4b8c-9d0e-1f2a3b4c5d6e"), "Nordre Strandvej 40, 8240 Risskov", "Risskov Apotek" },
                    { new Guid("45a1b2c3-d4e5-4f6a-8b9c-0d1e2f3a4b5c"), "Store Torv 5, 8000 Aarhus C", "Aarhus Løve Apotek" },
                    { new Guid("67b8c9d0-e1f2-4a3b-5c6d-7e8f9a0b1c2d"), "Viby Ringvej 10, 8260 Viby J", "Viby Apotek" }
                });

            migrationBuilder.InsertData(
                table: "Prescriptions",
                columns: new[] { "PrescriptionId", "PatientCprNumber", "PatientFullName", "AssignedPharmacyId", "ClinicId", "ClosedAt", "CreatedAt", "Status" },
                values: new object[,]
                {
                    { new Guid("a0a0a0a0-1111-2222-3333-444444444444"), "0101409999", "Gerda Olesen", null, new Guid("f1e2d3c4-b5a6-4978-9012-345678901234"), null, new DateTime(2021, 5, 10, 9, 0, 0, 0, DateTimeKind.Utc), 0 },
                    { new Guid("b0a0a0a0-b0b0-c0c0-d0d0-e0e0e0e0e0e0"), "1205851234", "Sofie Madsen", new Guid("45a1b2c3-d4e5-4f6a-8b9c-0d1e2f3a4b5c"), new Guid("d83b4c1a-5e2f-4b6a-9c8d-1e2f3a4b5c6d"), null, new DateTime(2024, 1, 15, 10, 30, 0, 0, DateTimeKind.Utc), 0 },
                    { new Guid("c0c0c0c0-5555-6666-7777-888888888888"), "1506758888", "Henrik Poulsen", new Guid("67b8c9d0-e1f2-4a3b-5c6d-7e8f9a0b1c2d"), new Guid("d83b4c1a-5e2f-4b6a-9c8d-1e2f3a4b5c6d"), null, new DateTime(2024, 3, 5, 11, 45, 0, 0, DateTimeKind.Utc), 0 },
                    { new Guid("f0f0f0f0-1010-2020-3030-404040404040"), "2408905678", "Lars Nielsen", null, new Guid("a1b2c3d4-e5f6-4a5b-8c9d-0e1f2a3b4c5d"), null, new DateTime(2024, 2, 20, 14, 15, 0, 0, DateTimeKind.Utc), 0 }
                });

            migrationBuilder.InsertData(
                table: "MedicationOrders",
                columns: new[] { "MedicationOrderId", "AllowedDispensations", "DispensedCount", "Dosage", "DrugName", "PrescriptionId" },
                values: new object[,]
                {
                    { new Guid("a4a4a4a4-4545-6767-8989-010101010101"), 10, 2, "50mg morgen", "Metoprolol", new Guid("a0a0a0a0-1111-2222-3333-444444444444") },
                    { new Guid("b5b5b5b5-5656-7878-9090-121212121212"), 12, 3, "efter skema", "Insulin Novorapid", new Guid("c0c0c0c0-5555-6666-7777-888888888888") },
                    { new Guid("d1d1d1d1-1212-3434-5656-787878787878"), 5, 1, "1g x 4 dagligt", "Panodil", new Guid("b0a0a0a0-b0b0-c0c0-d0d0-e0e0e0e0e0e0") },
                    { new Guid("e2e2e2e2-2323-4545-6767-898989898989"), 3, 0, "400mg ved smerter", "Ibuprofen", new Guid("f0f0f0f0-1010-2020-3030-404040404040") },
                    { new Guid("f3f3f3f3-3434-5656-7878-909090909090"), 1, 0, "1 tablet x 3 i 7 dage", "Penicillin", new Guid("f0f0f0f0-1010-2020-3030-404040404040") }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Clinics_ProviderNumber",
                table: "Clinics",
                column: "ProviderNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MedicationOrders_PrescriptionId",
                table: "MedicationOrders",
                column: "PrescriptionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Clinics");

            migrationBuilder.DropTable(
                name: "MedicationOrders");

            migrationBuilder.DropTable(
                name: "Pharmacies");

            migrationBuilder.DropTable(
                name: "Prescriptions");
        }
    }
}
