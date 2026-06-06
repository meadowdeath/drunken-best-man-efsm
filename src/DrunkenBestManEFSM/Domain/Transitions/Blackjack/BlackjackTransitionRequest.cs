using DrunkenBestManEFSM.Domain.Enums.Blackjack;

namespace DrunkenBestManEFSM.Domain.Transitions.Blackjack;

/// <summary>
/// Represents an attempted action inside the nested Blackjack EFSM.
/// </summary>
public sealed class BlackjackTransitionRequest
{
    public BlackjackAction Action { get; set; }

    public int? BetAmount { get; set; }
}
