namespace TripSplit.Api.Models;

public class ExpenseShare
{
    public int Id { get; set; }

    public int ExpenseId { get; set; }

    public int ParticipantId { get; set; }

    public decimal ShareAmount { get; set; }
}