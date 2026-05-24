using DrunkenBestManEFSM.Domain.Rules;
using DrunkenBestManEFSM.Tests.Domain.TestHelpers;

namespace DrunkenBestManEFSM.Tests.Domain.Rules;

public sealed class StatRulesTests
{
    [Fact]
    public void IsHealthDepleted_ShouldReturnTrue_WhenHealthIsZero()
    {
        var state = GameStateTestFactory.CreateWithStats(health: GameLimits.MinHealth);

        var result = StatRules.IsHealthDepleted(state.CharacterStats);

        Assert.True(result);
    }

    [Fact]
    public void IsHealthDepleted_ShouldReturnFalse_WhenHealthIsPositive()
    {
        var state = GameStateTestFactory.CreateWithStats(health: GameLimits.MinHealth + 1);

        var result = StatRules.IsHealthDepleted(state.CharacterStats);

        Assert.False(result);
    }

    [Fact]
    public void IsOutOfTime_ShouldReturnTrue_WhenRemainingTimeIsZero()
    {
        var state = GameStateTestFactory.CreateWithStats(remainingTime: GameLimits.MinRemainingTime);

        var result = StatRules.IsOutOfTime(state.CharacterStats);

        Assert.True(result);
    }

    [Fact]
    public void IsOutOfTime_ShouldReturnFalse_WhenRemainingTimeIsPositive()
    {
        var state = GameStateTestFactory.CreateWithStats(remainingTime: GameLimits.MinRemainingTime + 1);

        var result = StatRules.IsOutOfTime(state.CharacterStats);

        Assert.False(result);
    }

    [Fact]
    public void IsDehydrated_ShouldReturnTrue_WhenHangoverReachesMaximum()
    {
        var state = GameStateTestFactory.CreateWithStats(hangover: GameLimits.MaxHangover);

        var result = StatRules.IsDehydrated(state.CharacterStats);

        Assert.True(result);
    }

    [Fact]
    public void IsDehydrated_ShouldReturnFalse_WhenHangoverIsBelowMaximum()
    {
        var state = GameStateTestFactory.CreateWithStats(hangover: GameLimits.MaxHangover - 1);

        var result = StatRules.IsDehydrated(state.CharacterStats);

        Assert.False(result);
    }

    [Fact]
    public void IsDrunkennessAtMaximum_ShouldReturnTrue_WhenDrunkennessReachesMaximum()
    {
        var state = GameStateTestFactory.CreateWithStats(drunkenness: GameLimits.MaxDrunkenness);

        var result = StatRules.IsDrunkennessAtMaximum(state.CharacterStats);

        Assert.True(result);
    }
}
