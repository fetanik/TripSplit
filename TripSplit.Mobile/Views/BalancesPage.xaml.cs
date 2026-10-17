using TripSplit.Services;

namespace TripSplit.Views;

[QueryProperty(nameof(TripId), "tripId")]
public partial class BalancesPage : ContentPage
{
    private readonly TripApiService _tripApiService;

    public string TripId { get; set; } = string.Empty;

    public BalancesPage()
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

        await LoadBalancesAsync();
    }

    private async Task LoadBalancesAsync()
    {
        if (!int.TryParse(TripId, out var tripId))
            return;

        try
        {
            var result =
                await _tripApiService.GetBalancesAsync(tripId);

            if (result is null)
            {
                await DisplayAlertAsync(
                    "Помилка",
                    "Не вдалося отримати дані про баланси.",
                    "OK");

                return;
            }

            TotalExpensesLabel.Text =
                result.TotalExpensesText;

            BindableLayout.SetItemsSource(
                BalancesLayout,
                result.Balances);

            BindableLayout.SetItemsSource(
                SettlementsLayout,
                result.Settlements);

            NoSettlementsLabel.IsVisible =
                result.Settlements.Count == 0;

            SettlementsLayout.IsVisible =
                result.Settlements.Count > 0;
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync(
                "Помилка",
                $"Не вдалося завантажити баланси: {ex.Message}",
                "OK");
        }
    }
}