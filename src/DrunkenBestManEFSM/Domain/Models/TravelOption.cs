using DrunkenBestManEFSM.Domain.Enums;

namespace DrunkenBestManEFSM.Domain.Models;

/// <summary>
/// Represents a travel choice available to the player.
/// </summary>
public sealed class TravelOption
{
    public Location Destination { get; set; }

    public TravelMode TravelMode { get; set; }

    public RouteCost Cost { get; set; } = new();

    public bool IsAvailable { get; set; }

    public string? UnavailableReasonKey { get; set; }
}
