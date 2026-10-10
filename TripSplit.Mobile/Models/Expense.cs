namespace TripSplit.Models;

public class Expense
{
    public int Id { get; set; }

    public int TripId { get; set; }

    public string Title { get; set; } = string.Empty;

    public decimal Amount { get; set; }

    public int PaidByParticipantId { get; set; }

    public string PaidByName { get; set; } = string.Empty;

    public DateOnly Date { get; set; }

    public string Note { get; set; } = string.Empty;

    public List<ExpenseShare> Shares { get; set; } = new();

    public string AmountText => $"{Amount:F2} грн";

    public string DateText => Date.ToString("dd.MM.yyyy");

    public string PaidByText => $"Сплатив(ла): {PaidByName}";
}