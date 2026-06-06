namespace DrunkenBestManEFSM.Domain.Models.Blackjack;

/// <summary>
/// Represents an ordered Blackjack deck in draw order.
/// </summary>
public sealed class BlackjackDeck
{
    private readonly Queue<Card> cards;

    public BlackjackDeck(IEnumerable<Card> cards)
    {
        this.cards = new Queue<Card>(cards);
    }

    public int RemainingCards => cards.Count;

    public Card Draw()
    {
        if (cards.Count == 0)
        {
            throw new InvalidOperationException("Cannot draw from an empty Blackjack deck.");
        }

        return cards.Dequeue();
    }
}
