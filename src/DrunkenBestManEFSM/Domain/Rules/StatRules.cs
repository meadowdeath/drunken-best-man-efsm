using DrunkenBestManEFSM.Domain.Models;

namespace DrunkenBestManEFSM.Domain.Rules;

/// <summary>
/// Evaluates stat boundary conditions without mutating state.
/// </summary>
public static class StatRules
{
    public static bool IsHealthDepleted(CharacterStats stats) =>
        stats.Health <= GameLimits.MinHealth;

    public static bool IsOutOfTime(CharacterStats stats) =>
        stats.RemainingTime <= GameLimits.MinRemainingTime;

    public static bool IsDehydrated(CharacterStats stats) =>
        stats.Hangover >= GameLimits.MaxHangover;

    public static bool IsFuelEmpty(CharacterStats stats) =>
        stats.Fuel <= GameLimits.MinFuel;

    public static bool IsHangoverAtMaximum(CharacterStats stats) =>
        stats.Hangover >= GameLimits.MaxHangover;

    public static bool IsDrunkennessAtMaximum(CharacterStats stats) =>
        stats.Drunkenness >= GameLimits.MaxDrunkenness;

    public static bool IsHealthWithinLimits(CharacterStats stats) =>
        stats.Health >= GameLimits.MinHealth && stats.Health <= GameLimits.MaxHealth;

    public static bool IsHangoverWithinLimits(CharacterStats stats) =>
        stats.Hangover >= GameLimits.MinHangover && stats.Hangover <= GameLimits.MaxHangover;

    public static bool IsDrunkennessWithinLimits(CharacterStats stats) =>
        stats.Drunkenness >= GameLimits.MinDrunkenness && stats.Drunkenness <= GameLimits.MaxDrunkenness;

    public static bool IsFuelWithinLimits(CharacterStats stats) =>
        stats.Fuel >= GameLimits.MinFuel && stats.Fuel <= GameLimits.MaxFuel;

    public static bool IsRemainingTimeWithinLimits(CharacterStats stats) =>
        stats.RemainingTime >= GameLimits.MinRemainingTime && stats.RemainingTime <= GameLimits.MaxRemainingTime;

    public static bool IsMoneyWithinLimits(CharacterStats stats) =>
        stats.Money >= GameLimits.MinMoney;
}
