namespace TripSplit.Api.DTOs;

public class BalanceResponse
{
    public decimal TotalExpenses { get; set; }

    public List<ParticipantBalanceResponse> Balances { get; set; } = new();

    public List<SettlementResponse> Settlements { get; set; } = new();
}

public class ParticipantBalanceResponse
{
    public int ParticipantId { get; set; }

    public string ParticipantName { get; set; } = string.Empty;

    public decimal Paid { get; set; }

    public decimal Owed { get; set; }

    public decimal Balance { get; set; }
}

public class SettlementResponse
{
    public int FromParticipantId { get; set; }

    public string FromParticipantName { get; set; } = string.Empty;

    public int ToParticipantId { get; set; }

    public string ToParticipantName { get; set; } = string.Empty;

    public decimal Amount { get; set; }
}