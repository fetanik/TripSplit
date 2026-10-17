using System.Globalization;
using TripSplit.Models;
using TripSplit.Services;

namespace TripSplit.Views;

[QueryProperty(nameof(TripId), "tripId")]
[QueryProperty(nameof(ExpenseId), "expenseId")]
public partial class ExpenseFormPage : ContentPage
{
    private readonly TripApiService _tripApiService;

    private List<Participant> _participants = new();

    private List<ParticipantSelectionItem>
        _participantSelections = new();

    private bool _loaded;

    public string TripId { get; set; } = string.Empty;

    public string ExpenseId { get; set; } = string.Empty;

    private bool IsEditMode =>
        int.TryParse(
            ExpenseId,
            out var expenseId) &&
        expenseId > 0;

    public ExpenseFormPage()
    {
        InitializeComponent();

        var httpClient = new HttpClient
        {
            BaseAddress =
                new Uri("http://localhost:5170/")
        };

        _tripApiService =
            new TripApiService(httpClient);
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (_loaded)
            return;

        _loaded = true;

        await LoadPageAsync();
    }

    private async Task LoadPageAsync()
    {
        if (!int.TryParse(
                TripId,
                out var tripId))
        {
            return;
        }

        try
        {
            _participants =
                await _tripApiService
                    .GetParticipantsAsync(tripId);

            if (_participants.Count == 0)
            {
                await DisplayAlertAsync(
                    "Немає учасників",
                    "Спочатку додайте хоча б одного " +
                    "учасника подорожі.",
                    "OK");

                await Shell.Current
                    .GoToAsync("..");

                return;
            }

            PayerPicker.ItemsSource =
                _participants;

            if (IsEditMode)
            {
                await LoadExpenseForEditingAsync(
                    tripId);
            }
            else
            {
                PrepareNewExpense();
            }
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync(
                "Помилка",
                $"Не вдалося завантажити форму: {ex.Message}",
                "OK");
        }
    }

    private void PrepareNewExpense()
    {
        PageTitleLabel.Text =
            "Нова витрата";

        SaveButton.Text =
            "Зберегти витрату";

        PayerPicker.SelectedIndex = 0;

        _participantSelections =
            _participants
                .Select(
                    participant =>
                        new ParticipantSelectionItem
                        {
                            ParticipantId =
                                participant.Id,

                            Name =
                                participant.Name,

                            IsSelected = true
                        })
                .ToList();

        ParticipantsCollectionView.ItemsSource =
            _participantSelections;
    }

    private async Task LoadExpenseForEditingAsync(
        int tripId)
    {
        if (!int.TryParse(
                ExpenseId,
                out var expenseId))
        {
            return;
        }

        var expense =
            await _tripApiService
                .GetExpenseAsync(
                    tripId,
                    expenseId);

        if (expense is null)
        {
            await DisplayAlertAsync(
                "Помилка",
                "Витрату не знайдено.",
                "OK");

            await Shell.Current
                .GoToAsync("..");

            return;
        }

        PageTitleLabel.Text =
            "Редагування витрати";

        SaveButton.Text =
            "Зберегти зміни";

        TitleEntry.Text =
            expense.Title;

        AmountEntry.Text =
            expense.Amount.ToString(
                "0.##",
                CultureInfo.CurrentCulture);

        NoteEditor.Text =
            expense.Note;

        ExpenseDatePicker.Date =
            expense.Date.ToDateTime(
                TimeOnly.MinValue);

        var payerIndex =
            _participants.FindIndex(
                participant =>
                    participant.Id ==
                    expense.PaidByParticipantId);

        PayerPicker.SelectedIndex =
            payerIndex >= 0
                ? payerIndex
                : 0;

        var selectedIds =
            expense.Shares
                .Select(
                    share =>
                        share.ParticipantId)
                .ToHashSet();

        _participantSelections =
            _participants
                .Select(
                    participant =>
                        new ParticipantSelectionItem
                        {
                            ParticipantId =
                                participant.Id,

                            Name =
                                participant.Name,

                            IsSelected =
                                selectedIds.Contains(
                                    participant.Id)
                        })
                .ToList();

        ParticipantsCollectionView.ItemsSource =
            _participantSelections;
    }

    private async void OnSaveClicked(
        object? sender,
        EventArgs e)
    {
        if (!int.TryParse(
                TripId,
                out var tripId))
        {
            return;
        }

        var title =
            TitleEntry.Text?.Trim();

        if (string.IsNullOrWhiteSpace(title))
        {
            await DisplayAlertAsync(
                "Помилка",
                "Введіть назву витрати.",
                "OK");

            return;
        }

        if (!TryParseAmount(
                AmountEntry.Text,
                out var amount) ||
            amount <= 0)
        {
            await DisplayAlertAsync(
                "Помилка",
                "Введіть коректну суму.",
                "OK");

            return;
        }

        if (PayerPicker.SelectedItem
            is not Participant payer)
        {
            await DisplayAlertAsync(
                "Помилка",
                "Оберіть, хто заплатив.",
                "OK");

            return;
        }

        var selectedParticipantIds =
            _participantSelections
                .Where(
                    participant =>
                        participant.IsSelected)
                .Select(
                    participant =>
                        participant.ParticipantId)
                .ToList();

        if (selectedParticipantIds.Count == 0)
        {
            await DisplayAlertAsync(
                "Помилка",
                "Оберіть хоча б одного учасника " +
                "для розподілу витрати.",
                "OK");

            return;
        }

        var dateTime =
            ExpenseDatePicker.Date ??
            DateTime.Today;

        var request =
            new CreateExpenseRequest
            {
                Title = title,
                Amount = amount,

                PaidByParticipantId =
                    payer.Id,

                Date =
                    DateOnly.FromDateTime(
                        dateTime),

                Note =
                    NoteEditor.Text?.Trim() ??
                    string.Empty,

                ParticipantIds =
                    selectedParticipantIds
            };

        try
        {
            if (IsEditMode)
            {
                if (!int.TryParse(
                        ExpenseId,
                        out var expenseId))
                {
                    return;
                }

                await _tripApiService
                    .UpdateExpenseAsync(
                        tripId,
                        expenseId,
                        request);
            }
            else
            {
                await _tripApiService
                    .CreateExpenseAsync(
                        tripId,
                        request);
            }

            await Shell.Current
                .GoToAsync("..");
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync(
                "Помилка",
                IsEditMode
                    ? $"Не вдалося оновити витрату: {ex.Message}"
                    : $"Не вдалося створити витрату: {ex.Message}",
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

        var normalized =
            text.Replace(',', '.');

        return decimal.TryParse(
            normalized,
            NumberStyles.Number,
            CultureInfo.InvariantCulture,
            out amount);
    }
}