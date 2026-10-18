using TripSplit.ViewModels;

namespace TripSplit.Views;

public partial class AddTripPage : ContentPage
{
    public AddTripPage(
        AddTripViewModel viewModel)
    {
        InitializeComponent();

        BindingContext = viewModel;
    }
}