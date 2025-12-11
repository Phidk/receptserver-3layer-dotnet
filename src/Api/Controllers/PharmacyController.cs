using Microsoft.AspNetCore.Mvc;
using ReceptServer.Application;
using ReceptServer.Domain;

namespace Api.Controllers;

[ApiController]
[Route("api/pharmacies")]
public class PharmacyController : ControllerBase
{
    private readonly PrescriptionService _prescriptionService;
    private readonly ILogger<PharmacyController> _logger;

    public PharmacyController(PrescriptionService prescriptionService, ILogger<PharmacyController> logger)
    {
        _prescriptionService = prescriptionService;
        _logger = logger;
    }

    [HttpGet("{pharmacyId:guid}/assigned-prescriptions")]
    [ProducesResponseType(typeof(IReadOnlyCollection<Prescription>), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetAssignedPrescriptions(Guid pharmacyId)
    {
        if (pharmacyId == Guid.Empty)
        {
            return BadRequest("A valid pharmacy ID must be provided.");
        }

        try
        {
            var prescriptions = await _prescriptionService.GetAssignedToPharmacyAsync(pharmacyId);
            if (prescriptions == null || !prescriptions.Any())
            {
                return NotFound("No open prescriptions found for the specified pharmacy.");
            }
            return Ok(prescriptions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving assigned prescriptions for pharmacy {PharmacyId}", pharmacyId);
            return StatusCode(500, "An unexpected error occurred.");
        }
    }
}
