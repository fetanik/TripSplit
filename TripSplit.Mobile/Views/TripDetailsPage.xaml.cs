using TripSplit.ViewModels;

namespace TripSplit.Views;

[QueryProperty(nameof(TripId), "tripId")]
public partial class TripDetailsPage : ContentPage
{
    private readonly TripDetailsViewModel _viewModel;

    public string TripId { get; set; } =
        string.Empty;

    public TripDetailsPage(
        TripDetailsViewModel viewModel)
    {
        InitializeComponent();

        _viewModel = viewModel;

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

        _viewModel.TripId =
            tripId;

        await _viewModel.LoadAsync();
    }
}