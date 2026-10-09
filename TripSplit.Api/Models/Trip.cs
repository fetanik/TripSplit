namespace TripSplit.Api.Models;

public class Trip
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Destination { get; set; } = string.Empty;

    public DateOnly StartDate { get; set; }

    public DateOnly EndDate { get; set; }
}