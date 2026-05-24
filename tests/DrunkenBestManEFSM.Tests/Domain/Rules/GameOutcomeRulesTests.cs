using DrunkenBestManEFSM.Domain.Enums;
using DrunkenBestManEFSM.Domain.Rules;
using DrunkenBestManEFSM.Tests.Domain.TestHelpers;

namespace DrunkenBestManEFSM.Tests.Domain.Rules;

public sealed class GameOutcomeRulesTests
{
    [Fact]
    public void IsVictory_ShouldReturnTrue_WhenAtCorrectChurchWithRingsAndValidStats()
    {
        var state = GameStateTestFactory.CreateAtCorrectChurchWithRings();

        var result = GameOutcomeRules.IsVictory(state);

        Assert.True(result);
    }

    [Fact]
    public void IsDefeat_ShouldReturnTrue_WhenRemainingTimeIsZero()
    {
        var state = GameStateTestFactory.CreateWithStats(remainingTime: GameLimits.MinRemainingTime);

        var result = GameOutcomeRules.IsDefeat(state);

        Assert.True(result);
    }

    [Fact]
    public void IsDefeat_ShouldReturnTrue_WhenHealthIsZero()
    {
        var state = GameStateTestFactory.CreateWithStats(health: GameLimits.MinHealth);

        var result = GameOutcomeRules.IsDefeat(state);

        Assert.True(result);
    }

    [Fact]
    public void IsDefeat_ShouldReturnTrue_WhenHangoverReachesMaximum()
    {
        var state = GameStateTestFactory.CreateWithStats(hangover: GameLimits.MaxHangover);

        var result = GameOutcomeRules.IsDefeat(state);

        Assert.True(result);
    }

    [Fact]
    public void IsDefeat_ShouldReturnFalse_WhenOnlyDrunkennessIsMaximum()
    {
        var state = GameStateTestFactory.CreateWithStats(
            health: 50,
            hangover: 50,
            drunkenness: GameLimits.MaxDrunkenness,
            remainingTime: 30);

        var result = GameOutcomeRules.IsDefeat(state);

        Assert.False(result);
    }
}
