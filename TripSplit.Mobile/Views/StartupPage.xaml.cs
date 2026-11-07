using TripSplit.Services;

namespace TripSplit.Views;

public partial class StartupPage : ContentPage
{
    private readonly AuthService _authService;

    private readonly AppNavigationService
        _navigationService;

    private bool _initialized;

    public StartupPage(
        AuthService authService,
        AppNavigationService navigationService)
    {
        InitializeComponent();

        _authService =
            authService;

        _navigationService =
            navigationService;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        if (_initialized)
            return;

        _initialized = true;

        Dispatcher.Dispatch(async () =>
        {
            var authenticated =
                await _authService
                    .IsAuthenticatedAsync();

            if (authenticated)
            {
                await _navigationService
                    .ShowAuthenticatedAppAsync();
            }
            else
            {
                _navigationService.ShowLogin();
            }
        });
    }
}