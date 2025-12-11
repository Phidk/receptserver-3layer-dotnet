using Microsoft.AspNetCore.Mvc;

namespace Web.Mvc.Controllers;

public class AdminController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;

    public AdminController(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RunCloseExpiredJob()
    {
        try
        {
            var client = _httpClientFactory.CreateClient();
            // Base address bør konfigureres i appsettings.json og injectes.
            // For det eksempel er det hardcoded.
            client.BaseAddress = new Uri("http://localhost:5000"); // Antager at API'et kører på port 5000
            var response = await client.PostAsync("api/admin/close-expired-prescriptions", null);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<CloseExpiredResult>();
                TempData["Message"] = result?.Message ?? "Job completed successfully.";
            }
            else
            {
                TempData["Error"] = "Error running the job. Status code: " + response.StatusCode;
            }
        }
        catch (Exception ex)
        {
            TempData["Error"] = "An exception occurred: " + ex.Message;
        }

        return RedirectToAction("Index");
    }
}

public class CloseExpiredResult
{
    public string? Message { get; set; }
}
