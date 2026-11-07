using System.Net.Http.Json;
using TripSplit.Models;

namespace TripSplit.Services;

public class AuthService
{
    private const string TokenKey = "auth_token";
    private const string UserIdKey = "auth_user_id";
    private const string DisplayNameKey = "auth_display_name";
    private const string EmailKey = "auth_email";

    private readonly HttpClient _httpClient;

    public AuthService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<AuthResponse> RegisterAsync(
        string displayName,
        string email,
        string password)
    {
        var request = new RegisterRequest
        {
            DisplayName = displayName,
            Email = email,
            Password = password
        };

        var response =
            await _httpClient.PostAsJsonAsync(
                "api/auth/register",
                request);

        if (!response.IsSuccessStatusCode)
        {
            var message =
                await response.Content.ReadAsStringAsync();

            throw new InvalidOperationException(
                string.IsNullOrWhiteSpace(message)
                    ? "Не вдалося зареєструвати користувача."
                    : message.Trim('"'));
        }

        var result =
            await response.Content
                .ReadFromJsonAsync<AuthResponse>();

        if (result is null ||
            string.IsNullOrWhiteSpace(result.Token))
        {
            throw new InvalidOperationException(
                "Сервер не повернув дані авторизації.");
        }

        return result;
    }

    public async Task<AuthResponse> LoginAsync(
        string email,
        string password)
    {
        var request = new LoginRequest
        {
            Email = email,
            Password = password
        };

        var response =
            await _httpClient.PostAsJsonAsync(
                "api/auth/login",
                request);

        if (!response.IsSuccessStatusCode)
        {
            var message =
                await response.Content.ReadAsStringAsync();

            throw new InvalidOperationException(
                string.IsNullOrWhiteSpace(message)
                    ? "Неправильна електронна адреса або пароль."
                    : message.Trim('"'));
        }

        var result =
            await response.Content
                .ReadFromJsonAsync<AuthResponse>();

        if (result is null ||
            string.IsNullOrWhiteSpace(result.Token))
        {
            throw new InvalidOperationException(
                "Сервер не повернув дані авторизації.");
        }

        return result;
    }

    public async Task SaveSessionAsync(
        AuthResponse response)
    {
        await SecureStorage.Default.SetAsync(
            TokenKey,
            response.Token);

        await SecureStorage.Default.SetAsync(
            UserIdKey,
            response.UserId);

        await SecureStorage.Default.SetAsync(
            DisplayNameKey,
            response.DisplayName);

        await SecureStorage.Default.SetAsync(
            EmailKey,
            response.Email);
    }

    public async Task<string?> GetTokenAsync()
    {
        try
        {
            return await SecureStorage.Default
                .GetAsync(TokenKey);
        }
        catch
        {
            return null;
        }
    }

    public async Task<bool> IsAuthenticatedAsync()
    {
        var token =
            await GetTokenAsync();

        return !string.IsNullOrWhiteSpace(token);
    }

    public Task LogoutAsync()
    {
        SecureStorage.Default.Remove(TokenKey);
        SecureStorage.Default.Remove(UserIdKey);
        SecureStorage.Default.Remove(DisplayNameKey);
        SecureStorage.Default.Remove(EmailKey);

        return Task.CompletedTask;
    }
}