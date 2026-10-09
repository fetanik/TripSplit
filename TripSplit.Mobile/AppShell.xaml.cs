using TripSplit.Views;

namespace TripSplit;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        Routing.RegisterRoute(
            nameof(AddTripPage),
            typeof(AddTripPage));

        Routing.RegisterRoute(
            nameof(TripDetailsPage),
            typeof(TripDetailsPage));
    }
}