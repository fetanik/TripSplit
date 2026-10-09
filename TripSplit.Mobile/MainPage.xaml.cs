using TripSplit.Models;
using TripSplit.Services;
using TripSplit.Views;

namespace TripSplit;

public partial class MainPage : ContentPage
{
    private readonly TripApiService _tripApiService;

    public MainPage()
    {
        InitializeComponent();

        var httpClient = new HttpClient
        {
            BaseAddress = new Uri("http://localhost:5170/")
        };

        _tripApiService = new TripApiService(httpClient);
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        try
        {
            var trips = await _tripApiService.GetTripsAsync();
            TripsCollectionView.ItemsSource = trips;
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync(
                "Помилка",
                $"Не вдалося завантажити подорожі: {ex.Message}",
                "OK");
        }
    }

    private async void OnAddTripClicked(
        object? sender,
        EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(AddTripPage));
    }

    private async void OnTripSelectionChanged(
        object? sender,
        SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is not Trip trip)
            return;

        TripsCollectionView.SelectedItem = null;

        await Shell.Current.GoToAsync(
            $"{nameof(TripDetailsPage)}?tripId={trip.Id}");
    }
}