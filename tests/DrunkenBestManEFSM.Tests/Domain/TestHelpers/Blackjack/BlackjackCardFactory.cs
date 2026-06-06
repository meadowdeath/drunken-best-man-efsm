using DrunkenBestManEFSM.Domain.Enums.Blackjack;
using DrunkenBestManEFSM.Domain.Models.Blackjack;

namespace DrunkenBestManEFSM.Tests.Domain.TestHelpers.Blackjack;

public static class BlackjackCardFactory
{
    public static Card Card(CardRank rank, CardSuit suit = CardSuit.Spades) =>
        new(suit, rank);

    public static Card Ace(CardSuit suit = CardSuit.Spades) =>
        Card(CardRank.Ace, suit);

    public static Card Two(CardSuit suit = CardSuit.Spades) =>
        Card(CardRank.Two, suit);

    public static Card Three(CardSuit suit = CardSuit.Spades) =>
        Card(CardRank.Three, suit);

    public static Card Four(CardSuit suit = CardSuit.Spades) =>
        Card(CardRank.Four, suit);

    public static Card Five(CardSuit suit = CardSuit.Spades) =>
        Card(CardRank.Five, suit);

    public static Card Six(CardSuit suit = CardSuit.Spades) =>
        Card(CardRank.Six, suit);

    public static Card Seven(CardSuit suit = CardSuit.Spades) =>
        Card(CardRank.Seven, suit);

    public static Card Eight(CardSuit suit = CardSuit.Spades) =>
        Card(CardRank.Eight, suit);

    public static Card Nine(CardSuit suit = CardSuit.Spades) =>
        Card(CardRank.Nine, suit);

    public static Card Ten(CardSuit suit = CardSuit.Spades) =>
        Card(CardRank.Ten, suit);

    public static Card Jack(CardSuit suit = CardSuit.Spades) =>
        Card(CardRank.Jack, suit);

    public static Card Queen(CardSuit suit = CardSuit.Spades) =>
        Card(CardRank.Queen, suit);

    public static Card King(CardSuit suit = CardSuit.Spades) =>
        Card(CardRank.King, suit);

    public static BlackjackHand Hand(params Card[] cards)
    {
        var hand = new BlackjackHand();
        foreach (var card in cards)
        {
            hand.AddCard(card);
        }

        return hand;
    }
}
