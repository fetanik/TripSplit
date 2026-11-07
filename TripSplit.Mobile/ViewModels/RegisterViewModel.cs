using System.Windows.Input;
using TripSplit.Services;

namespace TripSplit.ViewModels;

public class RegisterViewModel : BaseViewModel
{
    private readonly AuthService _authService;

    private readonly AppNavigationService
        _navigationService;

    private string _displayName = string.Empty;
    private string _email = string.Empty;
    private string _password = string.Empty;
    private string _confirmPassword = string.Empty;

    public string DisplayName
    {
        get => _displayName;

        set => SetProperty(
            ref _displayName,
            value);
    }

    public string Email
    {
        get => _email;

        set => SetProperty(
            ref _email,
            value);
    }

    public string Password
    {
        get => _password;

        set => SetProperty(
            ref _password,
            value);
    }

    public string ConfirmPassword
    {
        get => _confirmPassword;

        set => SetProperty(
            ref _confirmPassword,
            value);
    }

    public ICommand RegisterCommand { get; }

    public ICommand BackToLoginCommand { get; }

    public RegisterViewModel(
        AuthService authService,
        AppNavigationService navigationService)
    {
        _authService =
            authService;

        _navigationService =
            navigationService;

        RegisterCommand =
            new Command(
                async () =>
                    await RegisterAsync());

        BackToLoginCommand =
            new Command(
                async () =>
                    await _navigationService
                        .GoBackAsync());
    }

    private async Task RegisterAsync()
    {
        if (IsBusy)
            return;

        var displayName =
            DisplayName?.Trim()
            ?? string.Empty;

        var email =
            Email?.Trim()
            ?? string.Empty;

        if (string.IsNullOrWhiteSpace(displayName))
        {
            await ShowErrorAsync(
                "Введіть ім'я.");

            return;
        }

        if (string.IsNullOrWhiteSpace(email))
        {
            await ShowErrorAsync(
                "Введіть електронну адресу.");

            return;
        }

        if (string.IsNullOrWhiteSpace(Password))
        {
            await ShowErrorAsync(
                "Введіть пароль.");

            return;
        }

        if (Password.Length < 6)
        {
            await ShowErrorAsync(
                "Пароль повинен містити щонайменше 6 символів.");

            return;
        }

        if (Password != ConfirmPassword)
        {
            await ShowErrorAsync(
                "Паролі не збігаються.");

            return;
        }

        try
        {
            IsBusy = true;

            var result =
                await _authService.RegisterAsync(
                    displayName,
                    email,
                    Password);

            await _authService
                .SaveSessionAsync(result);

            Password =
                string.Empty;

            ConfirmPassword =
                string.Empty;

            await _navigationService
                .ShowAuthenticatedAppAsync();
        }
        catch (Exception ex)
        {
            await ShowErrorAsync(
                ex.Message,
                "Не вдалося зареєструватися");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private static async Task ShowErrorAsync(
        string message,
        string title = "Помилка")
    {
        await Application.Current!
            .Windows[0]
            .Page!
            .DisplayAlertAsync(
                title,
                message,
                "OK");
    }
}