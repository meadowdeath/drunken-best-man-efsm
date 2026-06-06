using DrunkenBestManEFSM.Domain.Enums.Blackjack;
using DrunkenBestManEFSM.Domain.Results.Blackjack;

namespace DrunkenBestManEFSM.Domain.Transitions.Blackjack;

/// <summary>
/// Represents the result of one nested Blackjack transition.
/// </summary>
public sealed class BlackjackActionResult
{
    public bool Success { get; set; }

    public BlackjackState PreviousState { get; set; }

    public BlackjackState CurrentState { get; set; }

    public BlackjackResult Result { get; set; }

    public string MessageKey { get; set; } = string.Empty;

    public BlackjackRoundResult? RoundResult { get; set; }
}
