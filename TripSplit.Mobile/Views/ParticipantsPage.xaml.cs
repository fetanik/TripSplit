using TripSplit.Services;
using TripSplit.ViewModels;

namespace TripSplit.Views;

[QueryProperty(nameof(TripId), "tripId")]
public partial class ParticipantsPage : ContentPage
{
    private readonly ParticipantsViewModel _viewModel;

    public string TripId { get; set; } = string.Empty;

    public ParticipantsPage()
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
            new ParticipantsViewModel(service);

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