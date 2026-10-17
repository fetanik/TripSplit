using TripSplit.Models;
using TripSplit.Services;

namespace TripSplit.Views;

[QueryProperty(nameof(TripId), "tripId")]
public partial class ExpensesPage : ContentPage
{
    private readonly TripApiService _tripApiService;

    public string TripId { get; set; } = string.Empty;

    public ExpensesPage()
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

        await LoadExpensesAsync();
    }

    private async Task LoadExpensesAsync()
    {
        if (!int.TryParse(
                TripId,
                out var tripId))
        {
            return;
        }

        try
        {
            var expenses =
                await _tripApiService
                    .GetExpensesAsync(tripId);

            ExpensesCollectionView.ItemsSource =
                expenses;
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync(
                "Помилка",
                $"Не вдалося завантажити витрати: {ex.Message}",
                "OK");
        }
    }

    private async void OnAddExpenseClicked(
        object? sender,
        EventArgs e)
    {
        if (!int.TryParse(
                TripId,
                out var tripId))
        {
            return;
        }

        await Shell.Current.GoToAsync(
            $"expense-form?tripId={tripId}");
    }

    private async void OnEditExpenseClicked(
        object? sender,
        EventArgs e)
    {
        if (!int.TryParse(
                TripId,
                out var tripId))
        {
            return;
        }

        if (sender is not Button button)
            return;

        if (button.CommandParameter
            is not Expense expense)
        {
            return;
        }

        await Shell.Current.GoToAsync(
            $"expense-form?tripId={tripId}" +
            $"&expenseId={expense.Id}");
    }

    private async void OnDeleteExpenseClicked(
        object? sender,
        EventArgs e)
    {
        if (!int.TryParse(
                TripId,
                out var tripId))
        {
            return;
        }

        if (sender is not Button button)
            return;

        if (button.CommandParameter
            is not Expense expense)
        {
            return;
        }

        var confirmed =
            await DisplayAlertAsync(
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
                    tripId,
                    expense.Id);

            await LoadExpensesAsync();
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync(
                "Помилка",
                $"Не вдалося видалити витрату: {ex.Message}",
                "OK");
        }
    }
}