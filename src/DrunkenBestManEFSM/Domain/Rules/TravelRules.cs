using DrunkenBestManEFSM.Domain.Enums;
using DrunkenBestManEFSM.Domain.Models;

namespace DrunkenBestManEFSM.Domain.Rules;

/// <summary>
/// Evaluates travel eligibility without applying travel costs.
/// </summary>
public static class TravelRules
{
    public static bool CanWalk(GameState state) =>
        state.CharacterStats.RemainingTime > GameLimits.MinRemainingTime;

    public static bool CanDrive(GameState state) =>
        CanDriveFromCurrentLocation(state)
        && state.CharacterStats.Drunkenness < GameLimits.DrivingDrunkennessLimit
        && state.CharacterStats.Fuel > GameLimits.MinFuel
        && state.CharacterStats.RemainingTime > GameLimits.MinRemainingTime;

    public static bool CanDriveFromCurrentLocation(GameState state) =>
        state.CurrentLocation == state.CarLocation;

    public static bool HasEnoughFuel(CharacterStats stats, int fuelCost) =>
        stats.Fuel >= fuelCost;

    public static bool HasEnoughTime(CharacterStats stats, int timeCost) =>
        stats.RemainingTime >= timeCost;

    public static bool CanTravel(GameState state, TravelMode travelMode, int timeCost, int fuelCost) =>
        travelMode switch
        {
            TravelMode.Walk => CanWalk(state) && HasEnoughTime(state.CharacterStats, timeCost),
            TravelMode.Drive => CanDriveFromCurrentLocation(state)
                && HasEnoughFuel(state.CharacterStats, fuelCost)
                && HasEnoughTime(state.CharacterStats, timeCost)
                && state.CharacterStats.Drunkenness < GameLimits.DrivingDrunkennessLimit,
            _ => false
        };
}
