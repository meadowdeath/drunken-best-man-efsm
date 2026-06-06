using DrunkenBestManEFSM.Domain.Enums.Blackjack;

namespace DrunkenBestManEFSM.Domain.Models.Blackjack;

/// <summary>
/// Represents the extended state owned by the nested Blackjack EFSM.
/// </summary>
public sealed class BlackjackGameState
{
    public BlackjackState State { get; set; } = BlackjackState.WaitingForBet;

    public BlackjackHand PlayerHand { get; set; } = new();

    public BlackjackHand DealerHand { get; set; } = new();

    public BlackjackDeck Deck { get; set; } = new(Array.Empty<Card>());

    public int BetAmount { get; set; }

    public BlackjackResult Result { get; set; } = BlackjackResult.None;
}
