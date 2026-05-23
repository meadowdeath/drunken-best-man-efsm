using DrunkenBestManEFSM.Domain.Enums;
using DrunkenBestManEFSM.Domain.Models;

namespace DrunkenBestManEFSM.Domain.Rules;

/// <summary>
/// Evaluates victory and defeat conditions without mutating state.
/// </summary>
public static class GameOutcomeRules
{
    public static bool IsVictory(GameState state) =>
        state.CurrentLocation == ChurchCatalog.ToLocation(state.CorrectChurch)
        && state.HasRings
        && state.CharacterStats.RemainingTime > GameLimits.MinRemainingTime
        && state.CharacterStats.Health > GameLimits.MinHealth
        && state.CharacterStats.Hangover < GameLimits.MaxHangover;

    public static bool IsDefeat(GameState state) =>
        StatRules.IsOutOfTime(state.CharacterStats)
        || StatRules.IsHealthDepleted(state.CharacterStats)
        || StatRules.IsDehydrated(state.CharacterStats);

    public static GameResult GetGameResult(GameState state)
    {
        if (IsDefeat(state))
        {
            return GameResult.Defeat;
        }

        return IsVictory(state) ? GameResult.Victory : GameResult.InProgress;
    }
}
