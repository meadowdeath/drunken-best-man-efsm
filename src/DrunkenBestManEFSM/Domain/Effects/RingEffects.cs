using DrunkenBestManEFSM.Domain.Models;
using DrunkenBestManEFSM.Domain.Rules;

namespace DrunkenBestManEFSM.Domain.Effects;

/// <summary>
/// Applies ring-related effects without deciding whether they are allowed.
/// </summary>
public static class RingEffects
{
    public static void ApplyPickUpRings(GameState state)
    {
        state.HasRings = true;
        StatEffects.AddRemainingTime(state.CharacterStats, -GameEconomy.PickUpRingsTimeCost);
    }
}
