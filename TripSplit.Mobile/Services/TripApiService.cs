using System.Net.Http.Headers;
using System.Net.Http.Json;
using TripSplit.Models;

namespace TripSplit.Services;

public class TripApiService
{
    private readonly HttpClient _httpClient;
    private readonly AuthService _authService;

    public TripApiService(
        HttpClient httpClient,
        AuthService authService)
    {
        _httpClient = httpClient;
        _authService = authService;
    }

    private async Task PrepareAuthorizationAsync()
    {
        var token =
            await _authService.GetTokenAsync();

        if (string.IsNullOrWhiteSpace(token))
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                null;

            return;
        }

        _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(
                "Bearer",
                token);
    }

    // --------------------
    // Trips
    // --------------------

    public async Task<List<Trip>> GetTripsAsync()
    {
        await PrepareAuthorizationAsync();

        var trips =
            await _httpClient
                .GetFromJsonAsync<List<Trip>>(
                    "api/trips");

        return trips ?? new List<Trip>();
    }

    public async Task<Trip?> GetTripAsync(
        int id)
    {
        await PrepareAuthorizationAsync();

        return await _httpClient
            .GetFromJsonAsync<Trip>(
                $"api/trips/{id}");
    }

    public async Task<Trip?> CreateTripAsync(
        Trip trip)
    {
        await PrepareAuthorizationAsync();

        var response =
            await _httpClient.PostAsJsonAsync(
                "api/trips",
                trip);

        response.EnsureSuccessStatusCode();

        return await response.Content
            .ReadFromJsonAsync<Trip>();
    }

    // --------------------
    // Participants
    // --------------------

    public async Task<List<Participant>>
        GetParticipantsAsync(
            int tripId)
    {
        await PrepareAuthorizationAsync();

        var participants =
            await _httpClient
                .GetFromJsonAsync<List<Participant>>(
                    $"api/trips/{tripId}/participants");

        return participants ??
               new List<Participant>();
    }

    public async Task<Participant?>
        CreateParticipantAsync(
            int tripId,
            Participant participant)
    {
        await PrepareAuthorizationAsync();

        var response =
            await _httpClient.PostAsJsonAsync(
                $"api/trips/{tripId}/participants",
                participant);

        response.EnsureSuccessStatusCode();

        return await response.Content
            .ReadFromJsonAsync<Participant>();
    }

    public async Task DeleteParticipantAsync(
        int tripId,
        int participantId)
    {
        await PrepareAuthorizationAsync();

        var response =
            await _httpClient.DeleteAsync(
                $"api/trips/{tripId}/participants/{participantId}");

        response.EnsureSuccessStatusCode();
    }

    // --------------------
    // Expenses
    // --------------------

    public async Task<List<Expense>>
        GetExpensesAsync(
            int tripId)
    {
        await PrepareAuthorizationAsync();

        var expenses =
            await _httpClient
                .GetFromJsonAsync<List<Expense>>(
                    $"api/trips/{tripId}/expenses");

        return expenses ??
               new List<Expense>();
    }

    public async Task<Expense?>
        GetExpenseAsync(
            int tripId,
            int expenseId)
    {
        await PrepareAuthorizationAsync();

        return await _httpClient
            .GetFromJsonAsync<Expense>(
                $"api/trips/{tripId}/expenses/{expenseId}");
    }

    public async Task<Expense?>
        CreateExpenseAsync(
            int tripId,
            CreateExpenseRequest request)
    {
        await PrepareAuthorizationAsync();

        var response =
            await _httpClient.PostAsJsonAsync(
                $"api/trips/{tripId}/expenses",
                request);

        response.EnsureSuccessStatusCode();

        return await response.Content
            .ReadFromJsonAsync<Expense>();
    }

    public async Task<Expense?>
        UpdateExpenseAsync(
            int tripId,
            int expenseId,
            CreateExpenseRequest request)
    {
        await PrepareAuthorizationAsync();

        var response =
            await _httpClient.PutAsJsonAsync(
                $"api/trips/{tripId}/expenses/{expenseId}",
                request);

        response.EnsureSuccessStatusCode();

        return await response.Content
            .ReadFromJsonAsync<Expense>();
    }

    public async Task DeleteExpenseAsync(
        int tripId,
        int expenseId)
    {
        await PrepareAuthorizationAsync();

        var response =
            await _httpClient.DeleteAsync(
                $"api/trips/{tripId}/expenses/{expenseId}");

        response.EnsureSuccessStatusCode();
    }

    // --------------------
    // Balances
    // --------------------

    public async Task<BalanceResult?>
        GetBalancesAsync(
            int tripId)
    {
        await PrepareAuthorizationAsync();

        return await _httpClient
            .GetFromJsonAsync<BalanceResult>(
                $"api/trips/{tripId}/balances");
    }
}