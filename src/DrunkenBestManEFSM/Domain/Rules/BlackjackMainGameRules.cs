using DrunkenBestManEFSM.Domain.Enums;
using DrunkenBestManEFSM.Domain.Models;
using DrunkenBestManEFSM.Domain.Rules.Blackjack;

namespace DrunkenBestManEFSM.Domain.Rules;

/// <summary>
/// Evaluates whether the main game can consume a nested Blackjack round.
/// </summary>
public static class BlackjackMainGameRules
{
    public static bool IsAtCasino(GameState state) =>
        state.CurrentLocation == Location.Casino;

    public static bool CanPlayBlackjack(GameState state) =>
        IsAtCasino(state)
        && state.Result == GameResult.InProgress
        && HasEnoughMoneyForBlackjack(state)
        && HasEnoughTimeForBlackjack(state)
        && state.CharacterStats.Health > GameLimits.MinHealth
        && state.CharacterStats.Hangover < GameLimits.MaxHangover;

    public static bool HasEnoughMoneyForBlackjack(GameState state) =>
        BlackjackBetRules.HasEnoughMoneyForMinimumBet(state.CharacterStats.Money);

    public static bool HasEnoughTimeForBlackjack(GameState state) =>
        state.CharacterStats.RemainingTime > BlackjackRulesConfiguration.BlackjackRoundTimeCost;
}
