namespace TripSplit.Models;

public class ExpenseShare
{
    public int ParticipantId { get; set; }

    public string ParticipantName { get; set; } = string.Empty;

    public decimal ShareAmount { get; set; }
}