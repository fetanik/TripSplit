using TripSplit.ViewModels;

namespace TripSplit.Views;

public partial class RegisterPage : ContentPage
{
    public RegisterPage(
        RegisterViewModel viewModel)
    {
        InitializeComponent();

        BindingContext =
            viewModel;
    }
}