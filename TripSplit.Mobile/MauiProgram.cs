using Microsoft.Extensions.Logging;
using TripSplit.Services;
using TripSplit.ViewModels;
using TripSplit.Views;

namespace TripSplit;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();

        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
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

        builder.Services.AddSingleton(
            new HttpClient
            {
                BaseAddress =
                    new Uri("http://localhost:5170/")
            });

        builder.Services.AddSingleton<TripApiService>();

        // --------------------
        // ViewModels
        // --------------------

        builder.Services.AddTransient<MainViewModel>();

        builder.Services.AddTransient<AddTripViewModel>();

        builder.Services.AddTransient<TripDetailsViewModel>();

        builder.Services.AddTransient<ParticipantsViewModel>();

        builder.Services.AddTransient<ExpensesViewModel>();

        builder.Services.AddTransient<ExpenseFormViewModel>();

        builder.Services.AddTransient<BalancesViewModel>();

        // --------------------
        // Pages
        // --------------------

        builder.Services.AddTransient<MainPage>();

        builder.Services.AddTransient<AddTripPage>();

        builder.Services.AddTransient<TripDetailsPage>();

        builder.Services.AddTransient<ParticipantsPage>();

        builder.Services.AddTransient<ExpensesPage>();

        builder.Services.AddTransient<ExpenseFormPage>();

        builder.Services.AddTransient<BalancesPage>();

        // Shell створюємо один раз
        builder.Services.AddSingleton<AppShell>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}