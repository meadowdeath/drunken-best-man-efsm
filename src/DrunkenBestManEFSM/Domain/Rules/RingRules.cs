using DrunkenBestManEFSM.Domain.Enums;
using DrunkenBestManEFSM.Domain.Models;

namespace DrunkenBestManEFSM.Domain.Rules;

/// <summary>
/// Evaluates ring pickup eligibility without mutating state.
/// </summary>
public static class RingRules
{
    public static bool CanPickUpRings(GameState state) =>
        state.CurrentLocation == Location.JewelryStore
        && !state.HasRings
        && TravelRules.HasEnoughTime(state.CharacterStats, GameEconomy.PickUpRingsTimeCost);
}
