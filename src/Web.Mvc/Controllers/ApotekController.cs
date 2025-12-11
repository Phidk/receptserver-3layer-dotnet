using Microsoft.AspNetCore.Mvc;
using ReceptServer.Application;
using Web.Mvc.Models;

namespace Web.Mvc.Controllers;

public sealed class ApotekController : Controller
{
    private readonly PrescriptionService _service;

    public ApotekController(PrescriptionService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> Index(string? cpr)
    {
        var model = new ApotekViewModel { Cpr = cpr };

        if (!string.IsNullOrWhiteSpace(cpr))
        {
            try
            {
                // Finder åbne recepter på CPR og gør klar til visning.
                var prescriptions = await _service.SearchOpenByCprAsync(cpr);
                model.Prescriptions = prescriptions.Select(p => new PrescriptionDisplayModel
                {
                    PrescriptionId = p.PrescriptionId,
                    PatientCpr = p.Patient.CprNumber,
                    PatientName = p.Patient.FullName,
                    MedicationOrders = p.MedicationOrders.Select(o => new MedicationOrderDisplayModel
                    {
                        MedicationOrderId = o.MedicationOrderId,
                        DrugName = o.DrugName,
                        Dosage = o.Dosage,
                        AllowedDispensations = o.AllowedDispensations,
                        DispensedCount = o.DispensedCount,
                        CanDispense = o.CanDispense()
                    }).ToList()
                }).ToList();
            }
            catch (Exception ex)
            {
                model.Error = ex.Message;
            }
        }

        model.Message = TempData["Message"] as string;
        model.Error ??= TempData["Error"] as string;
        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Assigned()
    {
        var model = new ApotekViewModel();
        // Hardcoded apotek ID for Apotek A fra SeedData.
        // I et rigtigt system ville dette komme fra brugers context.
        var pharmacyId = new Guid("45a1b2c3-d4e5-4f6a-8b9c-0d1e2f3a4b5c");
        model.PharmacyName = "Aarhus Løve Apotek";

        try
        {
            // Henter recepter der er tildelt det specifikke apotek.
            var prescriptions = await _service.GetAssignedToPharmacyAsync(pharmacyId);
            model.Prescriptions = prescriptions.Select(p => new PrescriptionDisplayModel
            {
                PrescriptionId = p.PrescriptionId,
                PatientCpr = p.Patient.CprNumber,
                PatientName = p.Patient.FullName,
                MedicationOrders = p.MedicationOrders.Select(o => new MedicationOrderDisplayModel
                {
                    MedicationOrderId = o.MedicationOrderId,
                    DrugName = o.DrugName,
                    Dosage = o.Dosage,
                    AllowedDispensations = o.AllowedDispensations,
                    DispensedCount = o.DispensedCount,
                    CanDispense = o.CanDispense()
                }).ToList()
            }).ToList();
        }
        catch (Exception ex)
        {
            model.Error = ex.Message;
        }

        return View("Assigned", model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Search(string cpr)
    {
        return RedirectToAction(nameof(Index), new { cpr });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Dispense(Guid prescriptionId, Guid medicationOrderId, string cpr)
    {
        try
        {
            // Forretningsregel: registrer udlevering og luk recept hvis alt er udleveret.
            await _service.DispenseAsync(prescriptionId, medicationOrderId);
            TempData["Message"] = "Udlevering registreret.";
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;
        }

        return RedirectToAction(nameof(Index), new { cpr });
    }
}
