using DrunkenBestManEFSM.Domain.Enums;
using DrunkenBestManEFSM.Domain.Models;
using DrunkenBestManEFSM.Domain.Rules;

namespace DrunkenBestManEFSM.Domain.Effects;

/// <summary>
/// Applies travel effects without deciding whether travel is allowed.
/// </summary>
public static class TravelEffects
{
    public static void ApplyWalk(GameState state, Location destination, int timeCost)
    {
        state.CurrentLocation = destination;
        StatEffects.AddRemainingTime(state.CharacterStats, -timeCost);
        StatEffects.AddDrunkenness(state.CharacterStats, -GameEconomy.WalkDrunkennessReduction);
        StatEffects.AddHangover(state.CharacterStats, GameEconomy.WalkHangoverIncrease);
    }

    public static void ApplyDrive(GameState state, Location destination, int timeCost, int fuelCost)
    {
        state.CurrentLocation = destination;
        state.CarLocation = destination;
        StatEffects.AddRemainingTime(state.CharacterStats, -timeCost);
        StatEffects.AddFuel(state.CharacterStats, -fuelCost);
    }
}
