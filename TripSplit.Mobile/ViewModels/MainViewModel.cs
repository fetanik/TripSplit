using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using TripSplit.Models;
using TripSplit.Services;
using TripSplit.Views;

namespace TripSplit.ViewModels;

public class MainViewModel : INotifyPropertyChanged
{
    private readonly TripApiService _tripApiService;

    private bool _isBusy;
    private Trip? _selectedTrip;

    public ObservableCollection<Trip> Trips { get; } = new();

    public bool IsBusy
    {
        get => _isBusy;

        set
        {
            if (_isBusy == value)
                return;

            _isBusy = value;
            OnPropertyChanged();
        }
    }

    public Trip? SelectedTrip
    {
        get => _selectedTrip;

        set
        {
            if (_selectedTrip == value)
                return;

            _selectedTrip = value;
            OnPropertyChanged();

            if (value is not null)
                OpenTripCommand.Execute(value);
        }
    }

    public ICommand LoadTripsCommand { get; }

    public ICommand AddTripCommand { get; }

    public ICommand OpenTripCommand { get; }

    public MainViewModel(TripApiService tripApiService)
    {
        _tripApiService = tripApiService;

        LoadTripsCommand =
            new Command(
                async () => await LoadTripsAsync());

        AddTripCommand =
            new Command(
                async () => await AddTripAsync());

        OpenTripCommand =
            new Command<Trip>(
                async trip => await OpenTripAsync(trip));
    }

    private async Task LoadTripsAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            var trips =
                await _tripApiService.GetTripsAsync();

            Trips.Clear();

            foreach (var trip in trips)
                Trips.Add(trip);
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
        try
        {
            await Shell.Current.GoToAsync(
                nameof(AddTripPage));
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlertAsync(
                "Помилка",
                $"Не вдалося відкрити форму: {ex.Message}",
                "OK");
        }
    }

    private async Task OpenTripAsync(Trip trip)
    {
        try
        {
            // Знімаємо вибір, щоб після повернення
            // картку можна було натиснути ще раз.
            SelectedTrip = null;

            await Shell.Current.GoToAsync(
                $"{nameof(TripDetailsPage)}?tripId={trip.Id}");
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlertAsync(
                "Помилка",
                $"Не вдалося відкрити подорож: {ex.Message}",
                "OK");
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged(
        [CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(
            this,
            new PropertyChangedEventArgs(propertyName));
    }
}