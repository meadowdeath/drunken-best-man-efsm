using DrunkenBestManEFSM.Domain.Enums;
using DrunkenBestManEFSM.Domain.Models;
using DrunkenBestManEFSM.Domain.Rules;

namespace DrunkenBestManEFSM.Domain.Effects;

/// <summary>
/// Applies church entry consequences without deciding whether a church is correct.
/// </summary>
public static class ChurchEffects
{
    public static void ApplyWrongChurchPenalty(GameState state) =>
        StatEffects.AddRemainingTime(state.CharacterStats, -GameEconomy.WrongChurchTimePenalty);

    public static void ApplyVictory(GameState state)
    {
        state.Result = GameResult.Victory;
        state.CurrentLocation = Location.Victory;
    }
}
