using TripSplit.Services;
using TripSplit.ViewModels;

namespace TripSplit.Views;

[QueryProperty(nameof(TripId), "tripId")]
public partial class ExpensesPage : ContentPage
{
    private readonly ExpensesViewModel _viewModel;

    public string TripId { get; set; } = string.Empty;

    public ExpensesPage()
    {
        InitializeComponent();

        var httpClient = new HttpClient
        {
            BaseAddress =
                new Uri("http://localhost:5170/")
        };

        var service =
            new TripApiService(httpClient);

        _viewModel =
            new ExpensesViewModel(service);

        BindingContext = _viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (!int.TryParse(
                TripId,
                out var tripId))
        {
            return;
        }

        _viewModel.TripId = tripId;

        await _viewModel.LoadAsync();
    }
}