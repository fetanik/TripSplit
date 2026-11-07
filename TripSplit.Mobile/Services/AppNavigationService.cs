using Microsoft.Extensions.DependencyInjection;
using TripSplit.Views;

namespace TripSplit.Services;

public class AppNavigationService
{
    private readonly IServiceProvider _serviceProvider;

    public AppNavigationService(
        IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public Task ShowAuthenticatedAppAsync()
    {
        var window =
            Application.Current?
                .Windows
                .FirstOrDefault();

        if (window is null)
            return Task.CompletedTask;

        var shell =
            _serviceProvider
                .GetRequiredService<AppShell>();

        window.Page = shell;

        return Task.CompletedTask;
    }

    public void ShowLogin()
    {
        var window =
            Application.Current?
                .Windows
                .FirstOrDefault();

        if (window is null)
            return;

        var loginPage =
            _serviceProvider
                .GetRequiredService<LoginPage>();

        window.Page =
            new NavigationPage(loginPage);
    }

    public async Task ShowRegisterAsync()
    {
        var window =
            Application.Current?
                .Windows
                .FirstOrDefault();

        if (window?.Page is not NavigationPage navigationPage)
            return;

        var registerPage =
            _serviceProvider
                .GetRequiredService<RegisterPage>();

        await navigationPage.PushAsync(
            registerPage);
    }

    public async Task GoBackAsync()
    {
        var window =
            Application.Current?
                .Windows
                .FirstOrDefault();

        if (window?.Page is not NavigationPage navigationPage)
            return;

        if (navigationPage.Navigation.NavigationStack.Count > 1)
        {
            await navigationPage.PopAsync();
        }
    }
}