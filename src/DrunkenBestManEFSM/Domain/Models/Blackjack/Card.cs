using DrunkenBestManEFSM.Domain.Enums.Blackjack;

namespace DrunkenBestManEFSM.Domain.Models.Blackjack;

/// <summary>
/// Represents a playing card in Blackjack, defined by its suit and rank.
/// </summary>
public sealed record Card(CardSuit Suit, CardRank Rank);
