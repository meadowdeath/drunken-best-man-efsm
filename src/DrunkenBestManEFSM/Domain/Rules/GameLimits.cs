namespace DrunkenBestManEFSM.Domain.Rules;

/// <summary>
/// Defines stat, resource, and configuration limits.
/// </summary>
public static class GameLimits
{
    public const int MinHealth = 0;
    public const int MaxHealth = 100;
    public const int MinHangover = 0;
    public const int MaxHangover = 100;
    public const int MinDrunkenness = 0;
    public const int MaxDrunkenness = 100;
    public const int MinFuel = 0;
    public const int MaxFuel = 100;
    public const int MinRemainingTime = 0;
    public const int MaxRemainingTime = 75;
    public const int MinMoney = 0;
    public const int MinMemoryThreshold = 35;
    public const int MaxMemoryThreshold = 50;
    public const int RequiredDrunkennessForMemory = 65;
    public const int DrivingDrunkennessLimit = 85;
}
