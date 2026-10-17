namespace TripSplit.Models;

public class BalanceResult
{
    public decimal TotalExpenses { get; set; }

    public List<ParticipantBalance> Balances { get; set; } = new();

    public List<Settlement> Settlements { get; set; } = new();

    public string TotalExpensesText =>
        $"{TotalExpenses:F2} грн";
}

public class ParticipantBalance
{
    public int ParticipantId { get; set; }

    public string ParticipantName { get; set; } = string.Empty;

    public decimal Paid { get; set; }

    public decimal Owed { get; set; }

    public decimal Balance { get; set; }

    public string PaidText =>
        $"Сплачено: {Paid:F2} грн";

    public string OwedText =>
        $"Частка витрат: {Owed:F2} грн";

    public string BalanceText
    {
        get
        {
            if (Balance > 0)
                return $"+{Balance:F2} грн";

            return $"{Balance:F2} грн";
        }
    }

    public string BalanceDescription
    {
        get
        {
            if (Balance > 0)
                return "Має отримати";

            if (Balance < 0)
                return "Має сплатити";

            return "Розраховано";
        }
    }
}

public class Settlement
{
    public int FromParticipantId { get; set; }

    public string FromParticipantName { get; set; } = string.Empty;

    public int ToParticipantId { get; set; }

    public string ToParticipantName { get; set; } = string.Empty;

    public decimal Amount { get; set; }

    public string TransferText =>
        $"{FromParticipantName} → {ToParticipantName}";

    public string AmountText =>
        $"{Amount:F2} грн";
}