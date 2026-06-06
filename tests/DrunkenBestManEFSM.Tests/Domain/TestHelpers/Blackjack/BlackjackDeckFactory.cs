using DrunkenBestManEFSM.Domain.Models.Blackjack;

namespace DrunkenBestManEFSM.Tests.Domain.TestHelpers.Blackjack;

public static class BlackjackDeckFactory
{
    public static BlackjackDeck CreateDeckWithDrawOrder(params Card[] cards) =>
        new(cards);
}
