using System.Collections.ObjectModel;
using System.Windows.Input;
using TripSplit.Models;
using TripSplit.Services;
using TripSplit.Views;

namespace TripSplit.ViewModels;

public class MainViewModel : BaseViewModel
{
    private readonly TripApiService _tripApiService;

    private readonly AuthService _authService;

    private readonly AppNavigationService
        _navigationService;

    private Trip? _selectedTrip;

    public ObservableCollection<Trip> Trips { get; }
        = new();

    public Trip? SelectedTrip
    {
        get => _selectedTrip;

        set
        {
            if (!SetProperty(
                    ref _selectedTrip,
                    value))
            {
                return;
            }

            if (value is not null)
            {
                OpenTripCommand.Execute(value);
            }
        }
    }

    public ICommand LoadTripsCommand { get; }

    public ICommand AddTripCommand { get; }

    public ICommand OpenTripCommand { get; }

    public ICommand LogoutCommand { get; }

    public MainViewModel(
        TripApiService tripApiService,
        AuthService authService,
        AppNavigationService navigationService)
    {
        _tripApiService =
            tripApiService;

        _authService =
            authService;

        _navigationService =
            navigationService;

        LoadTripsCommand =
            new Command(
                async () =>
                    await LoadTripsAsync());

        AddTripCommand =
            new Command(
                async () =>
                    await AddTripAsync());

        OpenTripCommand =
            new Command<Trip>(
                async trip =>
                    await OpenTripAsync(trip));

        LogoutCommand =
            new Command(
                async () =>
                    await LogoutAsync());
    }

    private async Task LoadTripsAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            var trips =
                await _tripApiService
                    .GetTripsAsync();

            Trips.Clear();

            foreach (var trip in trips)
            {
                Trips.Add(trip);
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlertAsync(
                "Помилка",
                $"Не вдалося завантажити подорожі: {ex.Message}",
                "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task AddTripAsync()
    {
        await Shell.Current.GoToAsync(
            nameof(AddTripPage));
    }

    private async Task OpenTripAsync(
        Trip? trip)
    {
        if (trip is null)
            return;

        SelectedTrip = null;

        await Shell.Current.GoToAsync(
            $"{nameof(TripDetailsPage)}?tripId={trip.Id}");
    }

    private async Task LogoutAsync()
    {
        var confirmed =
            await Shell.Current.DisplayAlertAsync(
                "Вийти з акаунта?",
                "Ви дійсно хочете вийти?",
                "Вийти",
                "Скасувати");

        if (!confirmed)
            return;

        await _authService.LogoutAsync();

        Trips.Clear();

        _navigationService.ShowLogin();
    }
}