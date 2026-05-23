using DrunkenBestManEFSM.Domain.Models;
using DrunkenBestManEFSM.Domain.Rules;

namespace DrunkenBestManEFSM.Domain.Effects;

/// <summary>
/// Applies passive effects that happen after a turn.
/// </summary>
public static class TurnEffects
{
    public static void ApplyPassiveTurnEffects(GameState state)
    {
        StatEffects.AddHangover(state.CharacterStats, GameEconomy.PassiveHangoverIncrease);
        StatEffects.AddDrunkenness(state.CharacterStats, -GameEconomy.PassiveDrunkennessReduction);
    }
}
