using System.Collections.ObjectModel;
using System.Windows.Input;
using TripSplit.Models;
using TripSplit.Services;

namespace TripSplit.ViewModels;

public class ParticipantsViewModel : BaseViewModel
{
    private readonly TripApiService _tripApiService;

    private string _newParticipantName = string.Empty;

    public int TripId { get; set; }

    public ObservableCollection<Participant> Participants { get; }
        = new();

    public string NewParticipantName
    {
        get => _newParticipantName;
        set => SetProperty(
            ref _newParticipantName,
            value);
    }

    public ICommand AddParticipantCommand { get; }

    public ICommand DeleteParticipantCommand { get; }

    public ParticipantsViewModel(
        TripApiService tripApiService)
    {
        _tripApiService = tripApiService;

        AddParticipantCommand =
            new Command(
                async () =>
                    await AddParticipantAsync());

        DeleteParticipantCommand =
            new Command<Participant>(
                async participant =>
                    await DeleteParticipantAsync(
                        participant));
    }

    public async Task LoadAsync()
    {
        if (TripId <= 0 || IsBusy)
            return;

        try
        {
            IsBusy = true;

            var participants =
                await _tripApiService
                    .GetParticipantsAsync(TripId);

            Participants.Clear();

            foreach (var participant in participants)
            {
                Participants.Add(participant);
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlertAsync(
                "Помилка",
                $"Не вдалося завантажити учасників: {ex.Message}",
                "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task AddParticipantAsync()
    {
        var name =
            NewParticipantName?.Trim();

        if (string.IsNullOrWhiteSpace(name))
        {
            await Shell.Current.DisplayAlertAsync(
                "Помилка",
                "Введіть ім'я учасника.",
                "OK");

            return;
        }

        try
        {
            var participant =
                new Participant
                {
                    Name = name
                };

            await _tripApiService
                .CreateParticipantAsync(
                    TripId,
                    participant);

            NewParticipantName =
                string.Empty;

            await LoadAsync();
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlertAsync(
                "Помилка",
                $"Не вдалося додати учасника: {ex.Message}",
                "OK");
        }
    }

    private async Task DeleteParticipantAsync(
        Participant? participant)
    {
        if (participant is null)
            return;

        var confirmed =
            await Shell.Current.DisplayAlertAsync(
                "Видалити учасника?",
                $"Ви дійсно хочете видалити «{participant.Name}»?",
                "Видалити",
                "Скасувати");

        if (!confirmed)
            return;

        try
        {
            await _tripApiService
                .DeleteParticipantAsync(
                    TripId,
                    participant.Id);

            await LoadAsync();
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlertAsync(
                "Помилка",
                $"Не вдалося видалити учасника: {ex.Message}",
                "OK");
        }
    }
}