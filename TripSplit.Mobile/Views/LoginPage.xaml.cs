using TripSplit.ViewModels;

namespace TripSplit.Views;

public partial class LoginPage : ContentPage
{
    public LoginPage(
        LoginViewModel viewModel)
    {
        InitializeComponent();

        BindingContext =
            viewModel;
    }
}