namespace TripSplit.Api.Models;

public class Participant
{
    public int Id { get; set; }

    public int TripId { get; set; }

    public string Name { get; set; } = string.Empty;
}