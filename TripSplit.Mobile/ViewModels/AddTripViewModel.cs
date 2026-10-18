using System.Windows.Input;
using TripSplit.Models;
using TripSplit.Services;

namespace TripSplit.ViewModels;

public class AddTripViewModel : BaseViewModel
{
    private readonly TripApiService _tripApiService;

    private string _name = string.Empty;
    private string _destination = string.Empty;
    private DateTime? _startDate = DateTime.Today;
    private DateTime? _endDate = DateTime.Today.AddDays(1);

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

    public DateTime? StartDate
    {
        get => _startDate;
        set => SetProperty(
            ref _startDate,
            value);
    }

    public DateTime? EndDate
    {
        get => _endDate;
        set => SetProperty(
            ref _endDate,
            value);
    }

    public ICommand SaveCommand { get; }

    public AddTripViewModel(
        TripApiService tripApiService)
    {
        _tripApiService = tripApiService;

        SaveCommand =
            new Command(
                async () =>
                    await SaveAsync());
    }

    private async Task SaveAsync()
    {
        if (IsBusy)
            return;

        var name =
            Name?.Trim();

        var destination =
            Destination?.Trim();

        if (string.IsNullOrWhiteSpace(name))
        {
            await Shell.Current.DisplayAlertAsync(
                "Помилка",
                "Введіть назву подорожі.",
                "OK");

            return;
        }

        if (string.IsNullOrWhiteSpace(destination))
        {
            await Shell.Current.DisplayAlertAsync(
                "Помилка",
                "Введіть місце призначення.",
                "OK");

            return;
        }

        var startDate =
            StartDate ?? DateTime.Today;

        var endDate =
            EndDate ?? startDate.AddDays(1);

        if (endDate < startDate)
        {
            await Shell.Current.DisplayAlertAsync(
                "Помилка",
                "Дата завершення не може бути раніше дати початку.",
                "OK");

            return;
        }

        try
        {
            IsBusy = true;

            var trip =
                new Trip
                {
                    Name = name,

                    Destination =
                        destination,

                    StartDate =
                        DateOnly.FromDateTime(
                            startDate),

                    EndDate =
                        DateOnly.FromDateTime(
                            endDate)
                };

            await _tripApiService
                .CreateTripAsync(trip);

            await Shell.Current
                .GoToAsync("..");
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlertAsync(
                "Помилка",
                $"Не вдалося створити подорож: {ex.Message}",
                "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }
}