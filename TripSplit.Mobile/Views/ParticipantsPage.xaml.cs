using TripSplit.Models;
using TripSplit.Services;

namespace TripSplit.Views;

[QueryProperty(nameof(TripId), "tripId")]
public partial class ParticipantsPage : ContentPage
{
    private readonly TripApiService _tripApiService;

    public string TripId { get; set; } = string.Empty;

    public ParticipantsPage()
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

        await LoadParticipantsAsync();
    }

    private async Task LoadParticipantsAsync()
    {
        if (!int.TryParse(TripId, out var tripId))
            return;

        try
        {
            var participants =
                await _tripApiService.GetParticipantsAsync(tripId);

            ParticipantsCollectionView.ItemsSource = participants;
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync(
                "Помилка",
                $"Не вдалося завантажити учасників: {ex.Message}",
                "OK");
        }
    }

    private async void OnAddParticipantClicked(
        object? sender,
        EventArgs e)
    {
        if (!int.TryParse(TripId, out var tripId))
            return;

        var name = NameEntry.Text?.Trim();

        if (string.IsNullOrWhiteSpace(name))
        {
            await DisplayAlertAsync(
                "Помилка",
                "Введіть ім'я учасника.",
                "OK");

            return;
        }

        var participant = new Participant
        {
            Name = name
        };

        try
        {
            await _tripApiService.CreateParticipantAsync(
                tripId,
                participant);

            NameEntry.Text = string.Empty;

            await LoadParticipantsAsync();
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync(
                "Помилка",
                $"Не вдалося додати учасника: {ex.Message}",
                "OK");
        }
    }

    private async void OnDeleteParticipantClicked(
        object? sender,
        EventArgs e)
    {
        if (!int.TryParse(TripId, out var tripId))
            return;

        if (sender is not Button button)
            return;

        if (button.CommandParameter is not Participant participant)
            return;

        try
        {
            await _tripApiService.DeleteParticipantAsync(
                tripId,
                participant.Id);

            await LoadParticipantsAsync();
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync(
                "Помилка",
                $"Не вдалося видалити учасника: {ex.Message}",
                "OK");
        }
    }
}