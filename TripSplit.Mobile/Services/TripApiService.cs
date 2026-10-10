using System.Net.Http.Json;
using TripSplit.Models;

namespace TripSplit.Services;

public class TripApiService
{
    private readonly HttpClient _httpClient;

    public TripApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    // --------------------
    // Trips
    // --------------------

    public async Task<List<Trip>> GetTripsAsync()
    {
        var trips =
            await _httpClient.GetFromJsonAsync<List<Trip>>("api/trips");

        return trips ?? new List<Trip>();
    }

    public async Task<Trip?> GetTripAsync(int id)
    {
        return await _httpClient
            .GetFromJsonAsync<Trip>($"api/trips/{id}");
    }

    public async Task<Trip?> CreateTripAsync(Trip trip)
    {
        var response =
            await _httpClient.PostAsJsonAsync("api/trips", trip);

        response.EnsureSuccessStatusCode();

        return await response.Content
            .ReadFromJsonAsync<Trip>();
    }

    // --------------------
    // Participants
    // --------------------

    public async Task<List<Participant>> GetParticipantsAsync(int tripId)
    {
        var participants =
            await _httpClient.GetFromJsonAsync<List<Participant>>(
                $"api/trips/{tripId}/participants");

        return participants ?? new List<Participant>();
    }

    public async Task<Participant?> CreateParticipantAsync(
        int tripId,
        Participant participant)
    {
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
        var response =
            await _httpClient.DeleteAsync(
                $"api/trips/{tripId}/participants/{participantId}");

        response.EnsureSuccessStatusCode();
    }
}