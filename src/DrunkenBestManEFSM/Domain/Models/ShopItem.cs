using DrunkenBestManEFSM.Domain.Enums;

namespace DrunkenBestManEFSM.Domain.Models;

/// <summary>
/// Represents a purchasable item at a location.
/// </summary>
public sealed class ShopItem
{
    public ResourceType ResourceType { get; set; }

    public int Cost { get; set; }

    public int HealthChange { get; set; }

    public int HangoverChange { get; set; }

    public int DrunkennessChange { get; set; }

    public int FuelChange { get; set; }

    public int RemainingTimeChange { get; set; }
}
