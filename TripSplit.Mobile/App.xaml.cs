using TripSplit.Views;

namespace TripSplit;

public partial class App : Application
{
    private readonly StartupPage _startupPage;

    public App(
        StartupPage startupPage)
    {
        InitializeComponent();

        _startupPage =
            startupPage;
    }

    protected override Window CreateWindow(
        IActivationState? activationState)
    {
        return new Window(
            _startupPage);
    }
}