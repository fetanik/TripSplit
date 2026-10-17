using TripSplit.Services;
using TripSplit.ViewModels;

namespace TripSplit;

public partial class MainPage : ContentPage
{
    private readonly MainViewModel _viewModel;

    public MainPage()
    {
        InitializeComponent();

        var httpClient = new HttpClient
        {
            BaseAddress = new Uri(
                "http://localhost:5170/")
        };

        var tripApiService =
            new TripApiService(httpClient);

        _viewModel =
            new MainViewModel(tripApiService);

        BindingContext =
            _viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        _viewModel.LoadTripsCommand.Execute(null);
    }
}