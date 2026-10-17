namespace TripSplit.Models;

public class ParticipantSelectionItem
{
    public int ParticipantId { get; set; }

    public string Name { get; set; } = string.Empty;

    public bool IsSelected { get; set; }
}