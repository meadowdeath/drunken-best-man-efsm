namespace DrunkenBestManEFSM.Domain.Enums.Blackjack;

/// <summary>
/// Represents results supported by the initial Blackjack scope.
/// </summary>
public enum BlackjackResult
{
    None,
    PlayerWin,
    DealerWin,
    Draw,
    PlayerBlackjack,
    DealerBlackjack,
    Exited
}
