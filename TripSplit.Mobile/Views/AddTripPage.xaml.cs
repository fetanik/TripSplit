using TripSplit.Services;
using TripSplit.ViewModels;

namespace TripSplit.Views;

public partial class AddTripPage : ContentPage
{
    public AddTripPage()
    {
        InitializeComponent();

        var httpClient =
            new HttpClient
            {
                BaseAddress =
                    new Uri(
                        "http://localhost:5170/")
            };

        var service =
            new TripApiService(httpClient);

        BindingContext =
            new AddTripViewModel(service);
    }
}