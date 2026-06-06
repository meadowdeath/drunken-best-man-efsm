using DrunkenBestManEFSM.Domain.Enums.Blackjack;
using DrunkenBestManEFSM.Domain.Models.Blackjack;

namespace DrunkenBestManEFSM.Domain.Rules.Blackjack;

/// <summary>
/// Contains rules related to evaluating the player's and dealer's hands in Blackjack, including calculating hand
/// values, determining if a hand is a bust, and checking for natural Blackjack.
/// </summary>
public static class BlackjackHandRules
{
    public static int GetCardBaseValue(Card card) =>
        card.Rank switch
        {
            CardRank.Ace => BlackjackRulesConfiguration.AceHighValue,
            CardRank.Two => 2,
            CardRank.Three => 3,
            CardRank.Four => 4,
            CardRank.Five => 5,
            CardRank.Six => 6,
            CardRank.Seven => 7,
            CardRank.Eight => 8,
            CardRank.Nine => 9,
            CardRank.Ten => BlackjackRulesConfiguration.FaceCardValue,
            CardRank.Jack => BlackjackRulesConfiguration.FaceCardValue,
            CardRank.Queen => BlackjackRulesConfiguration.FaceCardValue,
            CardRank.King => BlackjackRulesConfiguration.FaceCardValue,
            _ => throw new ArgumentOutOfRangeException(nameof(card), card.Rank, null)
        };

    public static int CalculateValue(BlackjackHand hand)
    {
        var total = 0;
        var acesCountedHigh = 0;

        foreach (var card in hand.Cards)
        {
            total += GetCardBaseValue(card);

            if (card.Rank == CardRank.Ace)
            {
                acesCountedHigh++;
            }
        }

        while (total > BlackjackRulesConfiguration.BlackjackTargetValue && acesCountedHigh > 0)
        {
            total -= BlackjackRulesConfiguration.AceLowAdjustment;
            acesCountedHigh--;
        }

        return total;
    }

    public static bool IsBust(BlackjackHand hand) =>
        CalculateValue(hand) > BlackjackRulesConfiguration.BlackjackTargetValue;

    public static bool IsNaturalBlackjack(BlackjackHand hand) =>
        hand.Cards.Count == 2 && HasTwentyOne(hand);

    public static bool HasTwentyOne(BlackjackHand hand) =>
        CalculateValue(hand) == BlackjackRulesConfiguration.BlackjackTargetValue;
}
