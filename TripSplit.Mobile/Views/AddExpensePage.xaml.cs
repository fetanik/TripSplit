using System.Globalization;
using TripSplit.Models;
using TripSplit.Services;

namespace TripSplit.Views;

[QueryProperty(nameof(TripId), "tripId")]
public partial class AddExpensePage : ContentPage
{
    private readonly TripApiService _tripApiService;

    private List<ParticipantSelectionItem> _participants = new();

    public string TripId { get; set; } = string.Empty;

    public AddExpensePage()
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

            if (participants.Count == 0)
            {
                await DisplayAlertAsync(
                    "Немає учасників",
                    "Спочатку додайте хоча б одного учасника подорожі.",
                    "OK");

                await Shell.Current.GoToAsync("..");
                return;
            }

            PayerPicker.ItemsSource = participants;
            PayerPicker.SelectedIndex = 0;

            _participants = participants
                .Select(participant =>
                    new ParticipantSelectionItem
                    {
                        ParticipantId = participant.Id,
                        Name = participant.Name,
                        IsSelected = true
                    })
                .ToList();

            ParticipantsCollectionView.ItemsSource =
                _participants;
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync(
                "Помилка",
                $"Не вдалося завантажити учасників: {ex.Message}",
                "OK");
        }
    }

    private async void OnSaveClicked(
        object? sender,
        EventArgs e)
    {
        if (!int.TryParse(TripId, out var tripId))
            return;

        var title = TitleEntry.Text?.Trim();

        if (string.IsNullOrWhiteSpace(title))
        {
            await DisplayAlertAsync(
                "Помилка",
                "Введіть назву витрати.",
                "OK");

            return;
        }

        if (!TryParseAmount(AmountEntry.Text, out var amount) ||
            amount <= 0)
        {
            await DisplayAlertAsync(
                "Помилка",
                "Введіть коректну суму.",
                "OK");

            return;
        }

        if (PayerPicker.SelectedItem is not Participant payer)
        {
            await DisplayAlertAsync(
                "Помилка",
                "Оберіть, хто заплатив.",
                "OK");

            return;
        }

        var selectedParticipantIds = _participants
            .Where(participant => participant.IsSelected)
            .Select(participant => participant.ParticipantId)
            .ToList();

        if (selectedParticipantIds.Count == 0)
        {
            await DisplayAlertAsync(
                "Помилка",
                "Оберіть хоча б одного учасника для розподілу витрати.",
                "OK");

            return;
        }

        var dateTime =
            ExpenseDatePicker.Date ?? DateTime.Today;

        var request = new CreateExpenseRequest
        {
            Title = title,
            Amount = amount,
            PaidByParticipantId = payer.Id,
            Date = DateOnly.FromDateTime(dateTime),
            Note = NoteEditor.Text?.Trim() ?? string.Empty,
            ParticipantIds = selectedParticipantIds
        };

        try
        {
            await _tripApiService.CreateExpenseAsync(
                tripId,
                request);

            await Shell.Current.GoToAsync("..");
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync(
                "Помилка",
                $"Не вдалося створити витрату: {ex.Message}",
                "OK");
        }
    }

    private static bool TryParseAmount(
        string? text,
        out decimal amount)
    {
        amount = 0;

        if (string.IsNullOrWhiteSpace(text))
            return false;

        if (decimal.TryParse(
                text,
                NumberStyles.Number,
                CultureInfo.CurrentCulture,
                out amount))
        {
            return true;
        }

        var normalized = text.Replace(',', '.');

        return decimal.TryParse(
            normalized,
            NumberStyles.Number,
            CultureInfo.InvariantCulture,
            out amount);
    }
}