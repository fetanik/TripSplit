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
            BaseAddress = new Uri("http://localhost:5170/")
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
        if (!int.TryParse(TripId, out var tripId))
            return;

        try
        {
            var expenses =
                await _tripApiService.GetExpensesAsync(tripId);

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
}