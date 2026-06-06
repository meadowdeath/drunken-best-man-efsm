using DrunkenBestManEFSM.Domain.Enums.Blackjack;

namespace DrunkenBestManEFSM.Application.DTOs.Blackjack;

/// <summary>
/// Represents the current status of a blackjack round, including the player's hand, the dealer's visible
/// cards, the current bet, and the result if the round has finished.
/// </summary>
public sealed class BlackjackStatusDto
{
    public BlackjackState State { get; init; }

    public IReadOnlyList<BlackjackCardDto> PlayerCards { get; init; } = [];

    public IReadOnlyList<BlackjackCardDto> VisibleDealerCards { get; init; } = [];

    public int PlayerValue { get; init; }

    public int? DealerValue { get; init; }

    public int BetAmount { get; init; }

    public BlackjackResult Result { get; init; }

    public bool IsFinished { get; init; }
}
