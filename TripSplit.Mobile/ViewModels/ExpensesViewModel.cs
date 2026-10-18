using System.Collections.ObjectModel;
using System.Windows.Input;
using TripSplit.Models;
using TripSplit.Services;

namespace TripSplit.ViewModels;

public class ExpensesViewModel : BaseViewModel
{
    private readonly TripApiService _tripApiService;

    public int TripId { get; set; }

    public ObservableCollection<Expense> Expenses { get; }
        = new();

    public ICommand AddExpenseCommand { get; }

    public ICommand EditExpenseCommand { get; }

    public ICommand DeleteExpenseCommand { get; }

    public ExpensesViewModel(
        TripApiService tripApiService)
    {
        _tripApiService = tripApiService;

        AddExpenseCommand =
            new Command(
                async () =>
                    await AddExpenseAsync());

        EditExpenseCommand =
            new Command<Expense>(
                async expense =>
                    await EditExpenseAsync(expense));

        DeleteExpenseCommand =
            new Command<Expense>(
                async expense =>
                    await DeleteExpenseAsync(expense));
    }

    public async Task LoadAsync()
    {
        if (TripId <= 0 || IsBusy)
            return;

        try
        {
            IsBusy = true;

            var expenses =
                await _tripApiService
                    .GetExpensesAsync(TripId);

            Expenses.Clear();

            foreach (var expense in expenses)
            {
                Expenses.Add(expense);
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlertAsync(
                "Помилка",
                $"Не вдалося завантажити витрати: {ex.Message}",
                "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task AddExpenseAsync()
    {
        await Shell.Current.GoToAsync(
            $"expense-form?tripId={TripId}");
    }

    private async Task EditExpenseAsync(
        Expense? expense)
    {
        if (expense is null)
            return;

        await Shell.Current.GoToAsync(
            $"expense-form?tripId={TripId}" +
            $"&expenseId={expense.Id}");
    }

    private async Task DeleteExpenseAsync(
        Expense? expense)
    {
        if (expense is null)
            return;

        var confirmed =
            await Shell.Current.DisplayAlertAsync(
                "Видалити витрату?",
                $"Ви дійсно хочете видалити " +
                $"«{expense.Title}» на суму " +
                $"{expense.Amount:F2} грн?",
                "Видалити",
                "Скасувати");

        if (!confirmed)
            return;

        try
        {
            await _tripApiService
                .DeleteExpenseAsync(
                    TripId,
                    expense.Id);

            await LoadAsync();
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlertAsync(
                "Помилка",
                $"Не вдалося видалити витрату: {ex.Message}",
                "OK");
        }
    }
}