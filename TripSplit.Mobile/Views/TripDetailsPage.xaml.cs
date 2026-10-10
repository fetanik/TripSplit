using TripSplit.Services;

namespace TripSplit.Views;

[QueryProperty(nameof(TripId), "tripId")]
public partial class TripDetailsPage : ContentPage
{
    private readonly TripApiService _tripApiService;

    public string TripId { get; set; } = string.Empty;

    public TripDetailsPage()
    {
        InitializeComponent();

        var httpClient = new HttpClient
        {
            BaseAddress = new Uri("http://localhost:5170/")
        };

        _tripApiService = new TripApiService(httpClient);
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (!int.TryParse(TripId, out var tripId))
            return;

        try
        {
            var trip =
                await _tripApiService.GetTripAsync(tripId);

            if (trip is null)
            {
                await DisplayAlertAsync(
                    "Помилка",
                    "Подорож не знайдено.",
                    "OK");

                return;
            }

            NameLabel.Text = trip.Name;
            DestinationLabel.Text = trip.Destination;

            StartDateLabel.Text =
                trip.StartDate.ToString("dd.MM.yyyy");

            EndDateLabel.Text =
                trip.EndDate.ToString("dd.MM.yyyy");
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync(
                "Помилка",
                $"Не вдалося завантажити подорож: {ex.Message}",
                "OK");
        }
    }

    private async void OnParticipantsClicked(
        object? sender,
        EventArgs e)
    {
        if (!int.TryParse(TripId, out var tripId))
            return;

        await Shell.Current.GoToAsync(
            $"{nameof(ParticipantsPage)}?tripId={tripId}");
    }
}