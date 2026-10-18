using TripSplit.ViewModels;

namespace TripSplit.Views;

[QueryProperty(nameof(TripId), "tripId")]
[QueryProperty(nameof(ExpenseId), "expenseId")]
public partial class ExpenseFormPage : ContentPage
{
    private readonly ExpenseFormViewModel _viewModel;

    public string TripId { get; set; } =
        string.Empty;

    public string ExpenseId { get; set; } =
        string.Empty;

    public ExpenseFormPage(
        ExpenseFormViewModel viewModel)
    {
        InitializeComponent();

        _viewModel = viewModel;

        BindingContext = _viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (!int.TryParse(
                TripId,
                out var tripId))
        {
            return;
        }

        _viewModel.TripId =
            tripId;

        if (int.TryParse(
                ExpenseId,
                out var expenseId))
        {
            _viewModel.ExpenseId =
                expenseId;
        }
        else
        {
            _viewModel.ExpenseId = 0;
        }

        await _viewModel.LoadAsync();
    }
}