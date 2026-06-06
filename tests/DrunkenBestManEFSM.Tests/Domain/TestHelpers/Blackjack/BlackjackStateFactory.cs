using DrunkenBestManEFSM.Domain.Enums.Blackjack;
using DrunkenBestManEFSM.Domain.Models.Blackjack;
using static DrunkenBestManEFSM.Tests.Domain.TestHelpers.Blackjack.BlackjackCardFactory;

namespace DrunkenBestManEFSM.Tests.Domain.TestHelpers.Blackjack;

public static class BlackjackStateFactory
{
    public static BlackjackGameState CreateWaitingForBetStateWithDeck(params Card[] drawOrder) =>
        new()
        {
            State = BlackjackState.WaitingForBet,
            PlayerHand = new BlackjackHand(),
            DealerHand = new BlackjackHand(),
            Deck = BlackjackDeckFactory.CreateDeckWithDrawOrder(drawOrder),
            BetAmount = 0,
            Result = BlackjackResult.None
        };

    public static BlackjackGameState CreatePlayerTurnState(
        int betAmount = 10,
        IEnumerable<Card>? drawOrder = null,
        BlackjackHand? playerHand = null,
        BlackjackHand? dealerHand = null) =>
        new()
        {
            State = BlackjackState.PlayerTurn,
            PlayerHand = playerHand ?? Hand(Ten(), Six()),
            DealerHand = dealerHand ?? Hand(Nine(), Seven()),
            Deck = BlackjackDeckFactory.CreateDeckWithDrawOrder((drawOrder ?? []).ToArray()),
            BetAmount = betAmount,
            Result = BlackjackResult.None
        };

    public static BlackjackGameState CreateDealerTurnState(
        int betAmount = 10,
        IEnumerable<Card>? drawOrder = null,
        BlackjackHand? playerHand = null,
        BlackjackHand? dealerHand = null) =>
        new()
        {
            State = BlackjackState.DealerTurn,
            PlayerHand = playerHand ?? Hand(Ten(), Nine()),
            DealerHand = dealerHand ?? Hand(Ten(), Six()),
            Deck = BlackjackDeckFactory.CreateDeckWithDrawOrder((drawOrder ?? []).ToArray()),
            BetAmount = betAmount,
            Result = BlackjackResult.None
        };
}
