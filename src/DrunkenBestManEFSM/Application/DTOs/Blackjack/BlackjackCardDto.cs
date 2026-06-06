using DrunkenBestManEFSM.Domain.Enums.Blackjack;

namespace DrunkenBestManEFSM.Application.DTOs.Blackjack;

/// <summary>
/// Represents a single card in blackjack, including its suit, rank, and a display key for UI purposes.
/// </summary>
public sealed class BlackjackCardDto
{
    public CardSuit Suit { get; init; }

    public CardRank Rank { get; init; }

    public string DisplayKey { get; init; } = string.Empty;
}
