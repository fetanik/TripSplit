using System.Collections.ObjectModel;
using TripSplit.Models;
using TripSplit.Services;

namespace TripSplit.ViewModels;

public class BalancesViewModel : BaseViewModel
{
    private readonly TripApiService _tripApiService;

    private string _totalExpensesText =
        "0,00 грн";

    private bool _hasSettlements;

    public int TripId { get; set; }

    public string TotalExpensesText
    {
        get => _totalExpensesText;

        set => SetProperty(
            ref _totalExpensesText,
            value);
    }

    public bool HasSettlements
    {
        get => _hasSettlements;

        set
        {
            if (SetProperty(
                    ref _hasSettlements,
                    value))
            {
                OnPropertyChanged(
                    nameof(HasNoSettlements));
            }
        }
    }

    public bool HasNoSettlements =>
        !HasSettlements;

    public ObservableCollection<ParticipantBalance> Balances { get; }
        = new();

    public ObservableCollection<Settlement> Settlements { get; }
        = new();

    public BalancesViewModel(
        TripApiService tripApiService)
    {
        _tripApiService = tripApiService;
    }

    public async Task LoadAsync()
    {
        if (TripId <= 0 || IsBusy)
            return;

        try
        {
            IsBusy = true;

            var result =
                await _tripApiService
                    .GetBalancesAsync(TripId);

            if (result is null)
            {
                await Shell.Current.DisplayAlertAsync(
                    "Помилка",
                    "Не вдалося отримати баланси.",
                    "OK");

                return;
            }

            TotalExpensesText =
                result.TotalExpensesText;

            Balances.Clear();

            foreach (var balance in result.Balances)
            {
                Balances.Add(balance);
            }

            Settlements.Clear();

            foreach (var settlement in result.Settlements)
            {
                Settlements.Add(settlement);
            }

            HasSettlements =
                Settlements.Count > 0;
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlertAsync(
                "Помилка",
                $"Не вдалося завантажити баланси: {ex.Message}",
                "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }
}