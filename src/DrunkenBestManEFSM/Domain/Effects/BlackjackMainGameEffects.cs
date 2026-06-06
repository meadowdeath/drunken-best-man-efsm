using DrunkenBestManEFSM.Domain.Models;
using DrunkenBestManEFSM.Domain.Results.Blackjack;

namespace DrunkenBestManEFSM.Domain.Effects;

/// <summary>
/// Applies completed nested Blackjack results to the main game state.
/// </summary>
public static class BlackjackMainGameEffects
{
    public static void ApplyRoundResult(GameState state, BlackjackRoundResult roundResult)
    {
        StatEffects.AddMoney(state.CharacterStats, roundResult.MoneyChange);
        StatEffects.AddRemainingTime(state.CharacterStats, -roundResult.TimeCost);
    }
}
