using System.Windows.Input;
using TripSplit.Services;

namespace TripSplit.ViewModels;

public class TripDetailsViewModel : BaseViewModel
{
    private readonly TripApiService _tripApiService;

    private string _name = string.Empty;
    private string _destination = string.Empty;
    private string _dateRange = string.Empty;

    public int TripId { get; set; }

    public string Name
    {
        get => _name;
        set => SetProperty(
            ref _name,
            value);
    }

    public string Destination
    {
        get => _destination;
        set => SetProperty(
            ref _destination,
            value);
    }

    public string DateRange
    {
        get => _dateRange;
        set => SetProperty(
            ref _dateRange,
            value);
    }

    public ICommand OpenParticipantsCommand { get; }

    public ICommand OpenExpensesCommand { get; }

    public ICommand OpenBalancesCommand { get; }

    public TripDetailsViewModel(
        TripApiService tripApiService)
    {
        _tripApiService = tripApiService;

        OpenParticipantsCommand =
            new Command(
                async () =>
                    await OpenParticipantsAsync());

        OpenExpensesCommand =
            new Command(
                async () =>
                    await OpenExpensesAsync());

        OpenBalancesCommand =
            new Command(
                async () =>
                    await OpenBalancesAsync());
    }

    public async Task LoadAsync()
    {
        if (TripId <= 0 || IsBusy)
            return;

        try
        {
            IsBusy = true;

            var trip =
                await _tripApiService
                    .GetTripAsync(TripId);

            if (trip is null)
            {
                await Shell.Current.DisplayAlertAsync(
                    "Помилка",
                    "Подорож не знайдено.",
                    "OK");

                return;
            }

            Name =
                trip.Name;

            Destination =
                trip.Destination;

            DateRange =
                $"{trip.StartDate:dd.MM.yyyy} — " +
                $"{trip.EndDate:dd.MM.yyyy}";
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlertAsync(
                "Помилка",
                $"Не вдалося завантажити подорож: {ex.Message}",
                "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task OpenParticipantsAsync()
    {
        if (TripId <= 0)
            return;

        await Shell.Current.GoToAsync(
            $"{nameof(Views.ParticipantsPage)}?tripId={TripId}");
    }

    private async Task OpenExpensesAsync()
    {
        if (TripId <= 0)
            return;

        await Shell.Current.GoToAsync(
            $"{nameof(Views.ExpensesPage)}?tripId={TripId}");
    }

    private async Task OpenBalancesAsync()
    {
        if (TripId <= 0)
            return;

        await Shell.Current.GoToAsync(
            $"balances?tripId={TripId}");
    }
}