using DrunkenBestManEFSM.Domain.Enums.Blackjack;
using DrunkenBestManEFSM.Domain.Models.Blackjack;

namespace DrunkenBestManEFSM.Domain.Rules.Blackjack;

/// <summary>
/// Contains rules related to placing bets in Blackjack, including validation of bet amounts and checking if the
/// player has enough money to place a bet.
/// </summary>
public static class BlackjackBetRules
{
    public static bool IsValidBet(int betAmount, int availableMoney) =>
        betAmount > 0
        && IsWithinBetLimits(betAmount)
        && betAmount <= availableMoney;

    public static bool HasEnoughMoneyForMinimumBet(int availableMoney) =>
        availableMoney >= BlackjackRulesConfiguration.MinimumBet;

    public static bool IsWithinBetLimits(int betAmount) =>
        betAmount >= BlackjackRulesConfiguration.MinimumBet
        && betAmount <= BlackjackRulesConfiguration.MaximumBet;

    public static bool CanPlaceBet(BlackjackGameState state, int betAmount, int availableMoney) =>
        state.State == BlackjackState.WaitingForBet
        && IsValidBet(betAmount, availableMoney);
}
