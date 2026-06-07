using DrunkenBestManEFSM.Domain.Enums.Blackjack;
using DrunkenBestManEFSM.Domain.Results.Blackjack;
using DrunkenBestManEFSM.Domain.Rules.Blackjack;

namespace DrunkenBestManEFSM.Tests.Domain.TestHelpers;

public static class BlackjackRoundResultFactory
{
    public static BlackjackRoundResult PlayerWin(int betAmount = 10) =>
        Completed(BlackjackResult.PlayerWin, betAmount, betAmount);

    public static BlackjackRoundResult DealerWin(int betAmount = 10) =>
        Completed(BlackjackResult.DealerWin, betAmount, -betAmount);

    public static BlackjackRoundResult Draw(int betAmount = 10) =>
        Completed(BlackjackResult.Draw, betAmount, moneyChange: 0);

    public static BlackjackRoundResult Exited() =>
        new()
        {
            Result = BlackjackResult.Exited,
            BetAmount = 0,
            MoneyChange = 0,
            TimeCost = BlackjackRulesConfiguration.BlackjackExitTimeCost
        };

    private static BlackjackRoundResult Completed(BlackjackResult result, int betAmount, int moneyChange) =>
        new()
        {
            Result = result,
            BetAmount = betAmount,
            MoneyChange = moneyChange,
            TimeCost = BlackjackRulesConfiguration.BlackjackRoundTimeCost
        };
}
