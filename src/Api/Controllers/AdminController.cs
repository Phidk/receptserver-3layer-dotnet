using Microsoft.AspNetCore.Mvc;
using ReceptServer.Application;

namespace Api.Controllers;

[ApiController]
[Route("api/admin")]
public class AdminController : ControllerBase
{
    private readonly PrescriptionService _prescriptionService;
    private readonly ILogger<AdminController> _logger;

    public AdminController(PrescriptionService prescriptionService, ILogger<AdminController> logger)
    {
        _prescriptionService = prescriptionService;
        _logger = logger;
    }

    [HttpPost("close-expired-prescriptions")]
    [ProducesResponseType(typeof(int), 200)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> CloseExpiredPrescriptions()
    {
        try
        {
            var closedCount = await _prescriptionService.CloseExpiredPrescriptionsAsync();
            return Ok(new { message = $"Job udført. Lukkede {closedCount} udløbne recept(er)." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while closing expired prescriptions.");
            return StatusCode(500, "An unexpected error occurred while closing expired prescriptions.");
        }
    }
}
