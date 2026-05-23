using DrunkenBestManEFSM.Domain.Enums;

namespace DrunkenBestManEFSM.Domain.Models;

/// <summary>
/// Represents the cost of traveling between two locations using a specific travel mode.
/// </summary>
public sealed class RouteCost
{
    public Location From { get; set; }

    public Location To { get; set; }

    public TravelMode TravelMode { get; set; }

    public int TimeCost { get; set; }

    public int FuelCost { get; set; }
}
