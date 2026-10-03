namespace TripSplit.Models;

public class Trip
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Destination { get; set; } = string.Empty;

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }
}