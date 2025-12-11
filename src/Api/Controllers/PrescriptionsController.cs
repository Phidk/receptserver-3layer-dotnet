using Microsoft.AspNetCore.Mvc;
using ReceptServer.Api.Contracts;
using ReceptServer.Application;
using ReceptServer.Domain;

namespace ReceptServer.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class PrescriptionsController : ControllerBase
{
    private readonly PrescriptionService _service;

    public PrescriptionsController(PrescriptionService service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<ActionResult<PrescriptionDto>> Create([FromBody] CreatePrescriptionApiRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        try
        {
            // Opretter recept via application service med validering af CPR/ydernummer.
            var created = await _service.CreatePrescriptionAsync(new CreatePrescriptionRequest
            {
                ClinicId = request.ClinicId,
                ProviderNumber = request.ProviderNumber,
                PatientCprNumber = request.PatientCprNumber,
                PatientFullName = request.PatientFullName,
                MedicationOrders = request.MedicationOrders
            }, cancellationToken);

            return CreatedAtAction(nameof(GetByCpr), new { cpr = created.Patient.CprNumber }, ToDto(created));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<PrescriptionDto>>> GetByCpr([FromQuery] string cpr, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(cpr))
        {
            return BadRequest("cpr is required");
        }

        // Finder åbne recepter på CPR.
        var results = await _service.SearchOpenByCprAsync(cpr, cancellationToken);
        return Ok(results.Select(ToDto).ToList());
    }

    [HttpPost("{prescriptionId:guid}/dispense")]
    public async Task<ActionResult> Dispense(Guid prescriptionId, [FromBody] DispenseRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        try
        {
            // Forretningsregel: registrer udlevering og luk hvis alt er udleveret.
            await _service.DispenseAsync(prescriptionId, request.MedicationOrderId, cancellationToken);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    private static PrescriptionDto ToDto(Prescription prescription) => new()
    {
        PrescriptionId = prescription.PrescriptionId,
        ClinicId = prescription.ClinicId,
        PatientCprNumber = prescription.Patient.CprNumber,
        PatientFullName = prescription.Patient.FullName,
        AssignedPharmacyId = prescription.AssignedPharmacyId?.ToString(),
        CreatedAt = prescription.CreatedAt,
        Status = prescription.Status.ToString(),
        MedicationOrders = prescription.MedicationOrders.Select(o => new MedicationOrderDto
        {
            MedicationOrderId = o.MedicationOrderId,
            DrugName = o.DrugName,
            Dosage = o.Dosage,
            AllowedDispensations = o.AllowedDispensations,
            DispensedCount = o.DispensedCount,
            CanDispense = o.CanDispense()
        }).ToList()
    };
}
