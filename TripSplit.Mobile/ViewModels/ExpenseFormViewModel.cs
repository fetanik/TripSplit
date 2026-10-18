using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows.Input;
using TripSplit.Models;
using TripSplit.Services;

namespace TripSplit.ViewModels;

public class ExpenseFormViewModel : BaseViewModel
{
    private readonly TripApiService _tripApiService;

    private string _pageTitle = "Нова витрата";
    private string _saveButtonText = "Зберегти витрату";
    private string _title = string.Empty;
    private string _amountText = string.Empty;
    private Participant? _selectedPayer;
    private DateTime? _expenseDate = DateTime.Today;
    private string _note = string.Empty;

    public int TripId { get; set; }

    public int ExpenseId { get; set; }

    public bool IsEditMode =>
        ExpenseId > 0;

    public string PageTitle
    {
        get => _pageTitle;

        set => SetProperty(
            ref _pageTitle,
            value);
    }

    public string SaveButtonText
    {
        get => _saveButtonText;

        set => SetProperty(
            ref _saveButtonText,
            value);
    }

    public string Title
    {
        get => _title;

        set => SetProperty(
            ref _title,
            value);
    }

    public string AmountText
    {
        get => _amountText;

        set => SetProperty(
            ref _amountText,
            value);
    }

    public Participant? SelectedPayer
    {
        get => _selectedPayer;

        set => SetProperty(
            ref _selectedPayer,
            value);
    }

    public DateTime? ExpenseDate
    {
        get => _expenseDate;

        set => SetProperty(
            ref _expenseDate,
            value);
    }

    public string Note
    {
        get => _note;

        set => SetProperty(
            ref _note,
            value);
    }

    public ObservableCollection<Participant> Payers { get; }
        = new();

    public ObservableCollection<ParticipantSelectionItem>
        ParticipantSelections { get; }
        = new();

    public ICommand SaveCommand { get; }

    public ExpenseFormViewModel(
        TripApiService tripApiService)
    {
        _tripApiService = tripApiService;

        SaveCommand =
            new Command(
                async () =>
                    await SaveAsync());
    }

    public async Task LoadAsync()
    {
        if (TripId <= 0 || IsBusy)
            return;

        try
        {
            IsBusy = true;

            var participants =
                await _tripApiService
                    .GetParticipantsAsync(TripId);

            if (participants.Count == 0)
            {
                await Shell.Current.DisplayAlertAsync(
                    "Немає учасників",
                    "Спочатку додайте хоча б одного учасника подорожі.",
                    "OK");

                await Shell.Current.GoToAsync("..");

                return;
            }

            Payers.Clear();

            foreach (var participant in participants)
            {
                Payers.Add(participant);
            }

            if (IsEditMode)
            {
                await LoadExpenseAsync();
            }
            else
            {
                PrepareNewExpense();
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlertAsync(
                "Помилка",
                $"Не вдалося завантажити форму: {ex.Message}",
                "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private void PrepareNewExpense()
    {
        PageTitle =
            "Нова витрата";

        SaveButtonText =
            "Зберегти витрату";

        Title = string.Empty;
        AmountText = string.Empty;
        Note = string.Empty;
        ExpenseDate = DateTime.Today;

        SelectedPayer =
            Payers.FirstOrDefault();

        ParticipantSelections.Clear();

        foreach (var participant in Payers)
        {
            ParticipantSelections.Add(
                new ParticipantSelectionItem
                {
                    ParticipantId =
                        participant.Id,

                    Name =
                        participant.Name,

                    IsSelected = true
                });
        }
    }

    private async Task LoadExpenseAsync()
    {
        var expense =
            await _tripApiService
                .GetExpenseAsync(
                    TripId,
                    ExpenseId);

        if (expense is null)
        {
            await Shell.Current.DisplayAlertAsync(
                "Помилка",
                "Витрату не знайдено.",
                "OK");

            await Shell.Current.GoToAsync("..");

            return;
        }

        PageTitle =
            "Редагування витрати";

        SaveButtonText =
            "Зберегти зміни";

        Title =
            expense.Title;

        AmountText =
            expense.Amount.ToString(
                "0.##",
                CultureInfo.CurrentCulture);

        ExpenseDate =
            expense.Date.ToDateTime(
                TimeOnly.MinValue);

        Note =
            expense.Note;

        SelectedPayer =
            Payers.FirstOrDefault(
                participant =>
                    participant.Id ==
                    expense.PaidByParticipantId);

        var selectedIds =
            expense.Shares
                .Select(
                    share =>
                        share.ParticipantId)
                .ToHashSet();

        ParticipantSelections.Clear();

        foreach (var participant in Payers)
        {
            ParticipantSelections.Add(
                new ParticipantSelectionItem
                {
                    ParticipantId =
                        participant.Id,

                    Name =
                        participant.Name,

                    IsSelected =
                        selectedIds.Contains(
                            participant.Id)
                });
        }
    }

    private async Task SaveAsync()
    {
        if (IsBusy)
            return;

        var title =
            Title?.Trim();

        if (string.IsNullOrWhiteSpace(title))
        {
            await Shell.Current.DisplayAlertAsync(
                "Помилка",
                "Введіть назву витрати.",
                "OK");

            return;
        }

        if (!TryParseAmount(
                AmountText,
                out var amount) ||
            amount <= 0)
        {
            await Shell.Current.DisplayAlertAsync(
                "Помилка",
                "Введіть коректну суму.",
                "OK");

            return;
        }

        if (SelectedPayer is null)
        {
            await Shell.Current.DisplayAlertAsync(
                "Помилка",
                "Оберіть, хто заплатив.",
                "OK");

            return;
        }

        var selectedParticipantIds =
            ParticipantSelections
                .Where(
                    participant =>
                        participant.IsSelected)
                .Select(
                    participant =>
                        participant.ParticipantId)
                .ToList();

        if (selectedParticipantIds.Count == 0)
        {
            await Shell.Current.DisplayAlertAsync(
                "Помилка",
                "Оберіть хоча б одного учасника для розподілу витрати.",
                "OK");

            return;
        }

        var expenseDate =
            DateOnly.FromDateTime(
                ExpenseDate ??
                DateTime.Today);

        try
        {
            IsBusy = true;

            var trip =
                await _tripApiService
                    .GetTripAsync(TripId);

            if (trip is null)
            {
                await Shell.Current.DisplayAlertAsync(
                    "Помилка",
                    "Не вдалося знайти подорож.",
                    "OK");

                return;
            }

            if (expenseDate < trip.StartDate ||
                expenseDate > trip.EndDate)
            {
                var continueSaving =
                    await Shell.Current.DisplayAlertAsync(
                        "Дата поза межами подорожі",
                        $"Подорож триває з " +
                        $"{trip.StartDate:dd.MM.yyyy} до " +
                        $"{trip.EndDate:dd.MM.yyyy}.\n\n" +
                        $"Дата витрати: " +
                        $"{expenseDate:dd.MM.yyyy}.\n\n" +
                        "Це може бути попередня оплата " +
                        "або витрата після завершення подорожі. " +
                        "Зберегти витрату все одно?",
                        "Зберегти",
                        "Скасувати");

                if (!continueSaving)
                    return;
            }

            var request =
                new CreateExpenseRequest
                {
                    Title = title,

                    Amount = amount,

                    PaidByParticipantId =
                        SelectedPayer.Id,

                    Date = expenseDate,

                    Note =
                        Note?.Trim() ??
                        string.Empty,

                    ParticipantIds =
                        selectedParticipantIds
                };

            if (IsEditMode)
            {
                await _tripApiService
                    .UpdateExpenseAsync(
                        TripId,
                        ExpenseId,
                        request);
            }
            else
            {
                await _tripApiService
                    .CreateExpenseAsync(
                        TripId,
                        request);
            }

            await Shell.Current
                .GoToAsync("..");
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlertAsync(
                "Помилка",
                IsEditMode
                    ? $"Не вдалося оновити витрату: {ex.Message}"
                    : $"Не вдалося створити витрату: {ex.Message}",
                "OK");
        }
        finally
        {
            IsBusy = false;
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