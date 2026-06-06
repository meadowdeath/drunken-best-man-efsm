using DrunkenBestManEFSM.Domain.Enums.Blackjack;
using DrunkenBestManEFSM.Domain.Rules.Blackjack;
using DrunkenBestManEFSM.Tests.Domain.TestHelpers.Blackjack;

namespace DrunkenBestManEFSM.Tests.Domain.Blackjack;

public sealed class BlackjackBetRulesTests
{
    [Fact]
    public void IsValidBet_ShouldReturnTrue_WhenBetIsWithinLimitsAndPlayerHasMoney()
    {
        var result = BlackjackBetRules.IsValidBet(20, availableMoney: 50);

        Assert.True(result);
    }

    [Fact]
    public void IsValidBet_ShouldReturnFalse_WhenBetIsBelowMinimum()
    {
        var result = BlackjackBetRules.IsValidBet(BlackjackRulesConfiguration.MinimumBet - 1, availableMoney: 50);

        Assert.False(result);
    }

    [Fact]
    public void IsValidBet_ShouldReturnFalse_WhenBetIsAboveMaximum()
    {
        var result = BlackjackBetRules.IsValidBet(BlackjackRulesConfiguration.MaximumBet + 1, availableMoney: 100);

        Assert.False(result);
    }

    [Fact]
    public void IsValidBet_ShouldReturnFalse_WhenBetExceedsAvailableMoney()
    {
        var result = BlackjackBetRules.IsValidBet(20, availableMoney: 10);

        Assert.False(result);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void IsValidBet_ShouldReturnFalse_WhenBetIsZeroOrNegative(int betAmount)
    {
        var result = BlackjackBetRules.IsValidBet(betAmount, availableMoney: 50);

        Assert.False(result);
    }

    [Fact]
    public void HasEnoughMoneyForMinimumBet_ShouldReturnTrue_WhenAvailableMoneyMeetsMinimum()
    {
        var result = BlackjackBetRules.HasEnoughMoneyForMinimumBet(BlackjackRulesConfiguration.MinimumBet);

        Assert.True(result);
    }

    [Fact]
    public void HasEnoughMoneyForMinimumBet_ShouldReturnFalse_WhenAvailableMoneyIsBelowMinimum()
    {
        var result = BlackjackBetRules.HasEnoughMoneyForMinimumBet(BlackjackRulesConfiguration.MinimumBet - 1);

        Assert.False(result);
    }

    [Fact]
    public void CanPlaceBet_ShouldReturnTrue_WhenStateIsWaitingForBetAndBetIsValid()
    {
        var state = BlackjackStateFactory.CreateWaitingForBetStateWithDeck();

        var result = BlackjackBetRules.CanPlaceBet(state, 10, availableMoney: 50);

        Assert.True(result);
    }

    [Fact]
    public void CanPlaceBet_ShouldReturnFalse_WhenStateIsNotWaitingForBet()
    {
        var state = BlackjackStateFactory.CreatePlayerTurnState();

        var result = BlackjackBetRules.CanPlaceBet(state, 10, availableMoney: 50);

        Assert.False(result);
    }

    [Fact]
    public void CanPlaceBet_ShouldReturnFalse_WhenBetIsInvalid()
    {
        var state = BlackjackStateFactory.CreateWaitingForBetStateWithDeck();

        var result = BlackjackBetRules.CanPlaceBet(state, 5, availableMoney: 50);

        Assert.False(result);
    }
}
