using DrunkenBestManEFSM.Domain.Models;
using DrunkenBestManEFSM.Domain.Rules;

namespace DrunkenBestManEFSM.Domain.Effects;

/// <summary>
/// Centralizes stat and resource changes with clamping.
/// </summary>
public static class StatEffects
{
    public static void ClampStats(CharacterStats stats)
    {
        stats.Health = Clamp(stats.Health, GameLimits.MinHealth, GameLimits.MaxHealth);
        stats.Hangover = Clamp(stats.Hangover, GameLimits.MinHangover, GameLimits.MaxHangover);
        stats.Drunkenness = Clamp(stats.Drunkenness, GameLimits.MinDrunkenness, GameLimits.MaxDrunkenness);
        stats.Fuel = Clamp(stats.Fuel, GameLimits.MinFuel, GameLimits.MaxFuel);
        stats.RemainingTime = Clamp(stats.RemainingTime, GameLimits.MinRemainingTime, GameLimits.MaxRemainingTime);
        stats.Money = Math.Max(stats.Money, GameLimits.MinMoney);
    }

    public static void AddHealth(CharacterStats stats, int amount)
    {
        stats.Health += amount;
        ClampStats(stats);
    }

    public static void AddHangover(CharacterStats stats, int amount)
    {
        stats.Hangover += amount;
        ClampStats(stats);
    }

    public static void AddDrunkenness(CharacterStats stats, int amount)
    {
        stats.Drunkenness += amount;
        ClampStats(stats);
    }

    public static void AddFuel(CharacterStats stats, int amount)
    {
        stats.Fuel += amount;
        ClampStats(stats);
    }

    public static void AddRemainingTime(CharacterStats stats, int amount)
    {
        stats.RemainingTime += amount;
        ClampStats(stats);
    }

    public static void AddMoney(CharacterStats stats, int amount)
    {
        stats.Money += amount;
        ClampStats(stats);
    }

    private static int Clamp(int value, int min, int max) =>
        Math.Min(Math.Max(value, min), max);
}
