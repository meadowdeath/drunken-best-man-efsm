namespace DrunkenBestManEFSM.Domain.Rules.Blackjack;

/// <summary>
/// Contains configuration constants for the Blackjack game, such as minimum and maximum bet amounts, dealer stand
/// value, target value for Blackjack, and values for face cards and aces.
/// </summary>
public static class BlackjackRulesConfiguration
{
    public const int MinimumBet = 10;
    public const int MaximumBet = 30;
    public const int DealerStandValue = 17;
    public const int BlackjackTargetValue = 21;
    public const int FaceCardValue = 10;
    public const int AceHighValue = 11;
    public const int AceLowAdjustment = 10;
}
