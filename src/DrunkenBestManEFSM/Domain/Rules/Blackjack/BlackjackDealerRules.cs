using DrunkenBestManEFSM.Domain.Models.Blackjack;

namespace DrunkenBestManEFSM.Domain.Rules.Blackjack;

/// <summary>
/// Contains rules related to the dealer's actions in Blackjack, such as determining when the dealer should
/// hit or stand based on the value of the dealer's hand.
/// </summary>
public static class BlackjackDealerRules
{
    public static bool ShouldHit(BlackjackHand dealerHand) =>
        BlackjackHandRules.CalculateValue(dealerHand) < BlackjackRulesConfiguration.DealerStandValue;

    public static bool ShouldStand(BlackjackHand dealerHand) =>
        BlackjackHandRules.CalculateValue(dealerHand) >= BlackjackRulesConfiguration.DealerStandValue;
}
