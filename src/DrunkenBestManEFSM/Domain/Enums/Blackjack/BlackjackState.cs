namespace DrunkenBestManEFSM.Domain.Enums.Blackjack;

/// <summary>
/// Represents the current state of a nested Blackjack round.
/// </summary>
public enum BlackjackState
{
    WaitingForBet,
    InitialDeal,
    PlayerTurn,
    DealerTurn,
    Resolving,
    Finished
}
