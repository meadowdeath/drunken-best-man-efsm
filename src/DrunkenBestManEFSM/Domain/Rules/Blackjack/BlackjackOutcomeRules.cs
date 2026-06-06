using DrunkenBestManEFSM.Domain.Enums.Blackjack;
using DrunkenBestManEFSM.Domain.Models.Blackjack;

namespace DrunkenBestManEFSM.Domain.Rules.Blackjack;

/// <summary>
/// Contains rules related to resolving the outcome of a Blackjack round, including determining the winner based
/// on the player's and dealer's hands, and handling special cases such as natural Blackjack and busts.
/// </summary>
public static class BlackjackOutcomeRules
{
    public static BlackjackResult ResolveOutcome(BlackjackHand playerHand, BlackjackHand dealerHand)
    {
        var playerHasNaturalBlackjack = BlackjackHandRules.IsNaturalBlackjack(playerHand);
        var dealerHasNaturalBlackjack = BlackjackHandRules.IsNaturalBlackjack(dealerHand);

        if (playerHasNaturalBlackjack && !dealerHasNaturalBlackjack)
        {
            return BlackjackResult.PlayerBlackjack;
        }

        if (dealerHasNaturalBlackjack && !playerHasNaturalBlackjack)
        {
            return BlackjackResult.DealerBlackjack;
        }

        if (playerHasNaturalBlackjack && dealerHasNaturalBlackjack)
        {
            return BlackjackResult.Draw;
        }

        if (BlackjackHandRules.IsBust(playerHand))
        {
            return BlackjackResult.DealerWin;
        }

        if (BlackjackHandRules.IsBust(dealerHand))
        {
            return BlackjackResult.PlayerWin;
        }

        var playerValue = BlackjackHandRules.CalculateValue(playerHand);
        var dealerValue = BlackjackHandRules.CalculateValue(dealerHand);

        if (playerValue > dealerValue)
        {
            return BlackjackResult.PlayerWin;
        }

        if (dealerValue > playerValue)
        {
            return BlackjackResult.DealerWin;
        }

        return BlackjackResult.Draw;
    }
}
