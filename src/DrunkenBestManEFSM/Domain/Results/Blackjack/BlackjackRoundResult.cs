using DrunkenBestManEFSM.Domain.Enums.Blackjack;

namespace DrunkenBestManEFSM.Domain.Results.Blackjack;

/// <summary>
/// Stores the nested Blackjack round output for later main EFSM consumption.
/// </summary>
public sealed class BlackjackRoundResult
{
    public BlackjackResult Result { get; set; }

    public int BetAmount { get; set; }

    public int MoneyChange { get; set; }

    public int TimeCost { get; set; }
}
