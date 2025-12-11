using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;

namespace Client.Wpf;

public partial class MainWindow : Window
{
    private static readonly HttpClient HttpClient = new()
    {
        BaseAddress = new Uri("http://localhost:5000") // Tilpas til API base-url.
    };

    public MainWindow()
    {
        InitializeComponent();
    }

    private async void Submit_Click(object sender, RoutedEventArgs e)
    {
        StatusText.Text = string.Empty;

        if (!Guid.TryParse(ClinicIdText.Text, out var clinicId))
        {
            StatusText.Text = "ClinicId skal være et gyldigt GUID (se seeding i README).";
            return;
        }

        if (!int.TryParse(AllowedText.Text, out var allowed) || allowed <= 0)
        {
            StatusText.Text = "Antal udleveringer skal være et positivt tal.";
            return;
        }

        var payload = new
        {
            ClinicId = clinicId,
            ProviderNumber = ProviderNumberText.Text,
            PatientCprNumber = PatientCprText.Text,
            PatientFullName = PatientNameText.Text,
            MedicationOrders = new[]
            {
                new
                {
                    DrugName = DrugNameText.Text,
                    Dosage = DosageText.Text,
                    AllowedDispensations = allowed
                }
            }
        };

        try
        {
            // Kalder API’et for at oprette recept med enkel validering af input.
            var json = JsonSerializer.Serialize(payload);
            using var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await HttpClient.PostAsync("/api/prescriptions", content);

            if (response.IsSuccessStatusCode)
            {
                StatusText.Text = "Recept oprettet.";
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();
                StatusText.Text = $"Fejl: {response.StatusCode} - {error}";
            }
        }
        catch (Exception ex)
        {
            StatusText.Text = $"Fejl: {ex.Message}";
        }
    }
}
