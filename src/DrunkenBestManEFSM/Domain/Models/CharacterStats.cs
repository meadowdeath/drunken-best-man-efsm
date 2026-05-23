namespace DrunkenBestManEFSM.Domain.Models;

/// <summary>
/// Represents the player's current stats and resources.
/// </summary>
public sealed class CharacterStats
{
    public int Health { get; set; }

    public int Hangover { get; set; }

    public int Drunkenness { get; set; }

    public int Fuel { get; set; }

    public int RemainingTime { get; set; }

    public int Money { get; set; }
}
