using TripSplit.Models;
using TripSplit.Services;

namespace TripSplit.Views;

public partial class AddTripPage : ContentPage
{
    private readonly TripApiService _tripApiService;

    public AddTripPage()
    {
        InitializeComponent();

        var httpClient = new HttpClient
        {
            BaseAddress = new Uri("http://localhost:5170/")
        };

        _tripApiService = new TripApiService(httpClient);

        StartDatePicker.Date = DateTime.Today;
        EndDatePicker.Date = DateTime.Today.AddDays(1);
    }

    private async void OnCreateTripClicked(
        object? sender,
        EventArgs e)
    {
        var name = NameEntry.Text?.Trim();
        var destination = DestinationEntry.Text?.Trim();

        if (string.IsNullOrWhiteSpace(name))
        {
            await DisplayAlertAsync(
                "Помилка",
                "Введіть назву подорожі.",
                "OK");

            return;
        }

        if (string.IsNullOrWhiteSpace(destination))
        {
            await DisplayAlertAsync(
                "Помилка",
                "Введіть місце призначення.",
                "OK");

            return;
        }

        var startDateTime = StartDatePicker.Date ?? DateTime.Today;
        var endDateTime = EndDatePicker.Date ?? DateTime.Today.AddDays(1);

        var startDate = DateOnly.FromDateTime(startDateTime);
        var endDate = DateOnly.FromDateTime(endDateTime);

        if (endDate < startDate)
        {
            await DisplayAlertAsync(
                "Помилка",
                "Дата завершення не може бути раніше дати початку.",
                "OK");

            return;
        }

        var trip = new Trip
        {
            Name = name,
            Destination = destination,
            StartDate = startDate,
            EndDate = endDate
        };

        try
        {
            await _tripApiService.CreateTripAsync(trip);

            await Shell.Current.GoToAsync("..");
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync(
                "Помилка",
                $"Не вдалося створити подорож: {ex.Message}",
                "OK");
        }
    }
}