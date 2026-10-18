using TripSplit.Views;

namespace TripSplit;

public partial class AppShell : Shell
{
    public AppShell(MainPage mainPage)
    {
        InitializeComponent();

        Items.Add(
            new ShellContent
            {
                Title = "Мої подорожі",
                Route = "MainPage",
                Content = mainPage
            });

        Routing.RegisterRoute(
            nameof(AddTripPage),
            typeof(AddTripPage));

        Routing.RegisterRoute(
            nameof(TripDetailsPage),
            typeof(TripDetailsPage));

        Routing.RegisterRoute(
            nameof(ParticipantsPage),
            typeof(ParticipantsPage));

        Routing.RegisterRoute(
            nameof(ExpensesPage),
            typeof(ExpensesPage));

        Routing.RegisterRoute(
            "expense-form",
            typeof(ExpenseFormPage));

        Routing.RegisterRoute(
            "balances",
            typeof(BalancesPage));
    }
}