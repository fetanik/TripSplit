namespace TripSplit.Api.DTOs;

public class CreateExpenseRequest
{
    public string Title { get; set; } = string.Empty;

    public decimal Amount { get; set; }

    public int PaidByParticipantId { get; set; }

    public DateOnly Date { get; set; }

    public string Note { get; set; } = string.Empty;

    public List<int> ParticipantIds { get; set; } = new();
}