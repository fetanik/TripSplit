using Microsoft.Extensions.Logging;
using TripSplit.Services;
using TripSplit.ViewModels;
using TripSplit.Views;

namespace TripSplit;

public static class MauiProgram
{
    private const string ApiBaseAddress =
        "http://localhost:5170/";

    public static MauiApp CreateMauiApp()
    {
        var builder =
            MauiApp.CreateBuilder();

        builder
            .UseMauiApp<App>()
            .ConfigureFonts(
                fonts =>
                {
                    fonts.AddFont(
                        "OpenSans-Regular.ttf",
                        "OpenSansRegular");

                    fonts.AddFont(
                        "OpenSans-Semibold.ttf",
                        "OpenSansSemibold");
                });

        // --------------------
        // Services
        // --------------------

        builder.Services
            .AddSingleton<AuthService>(
                _ =>
                    new AuthService(
                        new HttpClient
                        {
                            BaseAddress =
                                new Uri(
                                    ApiBaseAddress)
                        }));

        builder.Services
            .AddSingleton<TripApiService>(
                serviceProvider =>
                    new TripApiService(
                        new HttpClient
                        {
                            BaseAddress =
                                new Uri(
                                    ApiBaseAddress)
                        },
                        serviceProvider
                            .GetRequiredService<AuthService>()));

        builder.Services
            .AddSingleton<AppNavigationService>();

        // --------------------
        // ViewModels
        // --------------------

        builder.Services
            .AddTransient<LoginViewModel>();

        builder.Services
            .AddTransient<RegisterViewModel>();

        builder.Services
            .AddTransient<MainViewModel>();

        builder.Services
            .AddTransient<AddTripViewModel>();

        builder.Services
            .AddTransient<TripDetailsViewModel>();

        builder.Services
            .AddTransient<ParticipantsViewModel>();

        builder.Services
            .AddTransient<ExpensesViewModel>();

        builder.Services
            .AddTransient<ExpenseFormViewModel>();

        builder.Services
            .AddTransient<BalancesViewModel>();

        // --------------------
        // Pages
        // --------------------

        builder.Services
            .AddTransient<StartupPage>();

        builder.Services
            .AddTransient<LoginPage>();

        builder.Services
            .AddTransient<RegisterPage>();

        builder.Services
            .AddTransient<MainPage>();

        builder.Services
            .AddTransient<AddTripPage>();

        builder.Services
            .AddTransient<TripDetailsPage>();

        builder.Services
            .AddTransient<ParticipantsPage>();

        builder.Services
            .AddTransient<ExpensesPage>();

        builder.Services
            .AddTransient<ExpenseFormPage>();

        builder.Services
            .AddTransient<BalancesPage>();

        builder.Services
            .AddSingleton<AppShell>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}