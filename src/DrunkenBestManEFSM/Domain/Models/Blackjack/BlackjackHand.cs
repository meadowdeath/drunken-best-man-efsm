namespace DrunkenBestManEFSM.Domain.Models.Blackjack;

/// <summary>
/// Stores cards for a Blackjack participant without evaluating hand rules.
/// </summary>
public sealed class BlackjackHand
{
    private readonly List<Card> cards = [];

    public IReadOnlyList<Card> Cards => cards;

    public void AddCard(Card card) =>
        cards.Add(card);

    public void Clear() =>
        cards.Clear();
}
