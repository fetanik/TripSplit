using System.Windows.Input;
using TripSplit.Services;

namespace TripSplit.ViewModels;

public class LoginViewModel : BaseViewModel
{
    private readonly AuthService _authService;

    private readonly AppNavigationService
        _navigationService;

    private string _email = string.Empty;
    private string _password = string.Empty;

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

    public ICommand LoginCommand { get; }

    public ICommand OpenRegisterCommand { get; }

    public LoginViewModel(
        AuthService authService,
        AppNavigationService navigationService)
    {
        _authService = authService;

        _navigationService =
            navigationService;

        LoginCommand =
            new Command(
                async () =>
                    await LoginAsync());

        OpenRegisterCommand =
            new Command(
                async () =>
                    await _navigationService
                        .ShowRegisterAsync());
    }

    private async Task LoginAsync()
    {
        if (IsBusy)
            return;

        var email =
            Email?.Trim()
            ?? string.Empty;

        if (string.IsNullOrWhiteSpace(email))
        {
            await Application.Current!
                .Windows[0]
                .Page!
                .DisplayAlertAsync(
                    "Помилка",
                    "Введіть електронну адресу.",
                    "OK");

            return;
        }

        if (string.IsNullOrWhiteSpace(Password))
        {
            await Application.Current!
                .Windows[0]
                .Page!
                .DisplayAlertAsync(
                    "Помилка",
                    "Введіть пароль.",
                    "OK");

            return;
        }

        try
        {
            IsBusy = true;

            var result =
                await _authService.LoginAsync(
                    email,
                    Password);

            await _authService
                .SaveSessionAsync(result);

            Password = string.Empty;

            await _navigationService
                .ShowAuthenticatedAppAsync();
        }
        catch (Exception ex)
        {
            await Application.Current!
                .Windows[0]
                .Page!
                .DisplayAlertAsync(
                    "Не вдалося увійти",
                    ex.Message,
                    "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }
}