namespace TripSplit.Api.DTOs;

public class ExpenseResponse
{
    public int Id { get; set; }

    public int TripId { get; set; }

    public string Title { get; set; } = string.Empty;

    public decimal Amount { get; set; }

    public int PaidByParticipantId { get; set; }

    public string PaidByName { get; set; } = string.Empty;

    public DateOnly Date { get; set; }

    public string Note { get; set; } = string.Empty;

    public List<ExpenseShareResponse> Shares { get; set; } = new();
}

public class ExpenseShareResponse
{
    public int ParticipantId { get; set; }

    public string ParticipantName { get; set; } = string.Empty;

    public decimal ShareAmount { get; set; }
}