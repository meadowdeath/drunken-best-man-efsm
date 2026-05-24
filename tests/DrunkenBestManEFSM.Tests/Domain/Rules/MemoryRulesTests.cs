using DrunkenBestManEFSM.Domain.Rules;
using DrunkenBestManEFSM.Tests.Domain.TestHelpers;

namespace DrunkenBestManEFSM.Tests.Domain.Rules;

public sealed class MemoryRulesTests
{
    [Fact]
    public void CanRememberCorrectChurch_ShouldReturnTrue_WhenHangoverAndDrunkennessAreLowEnough()
    {
        var state = GameStateTestFactory.CreateWithStats(hangover: 35, drunkenness: GameLimits.RequiredDrunkennessForMemory - 1);
        state.MemoryThreshold = 35;

        var result = MemoryRules.CanRememberCorrectChurch(state);

        Assert.True(result);
    }

    [Fact]
    public void CanRememberCorrectChurch_ShouldReturnFalse_WhenHangoverIsTooHigh()
    {
        var state = GameStateTestFactory.CreateWithStats(hangover: 41, drunkenness: GameLimits.RequiredDrunkennessForMemory - 1);
        state.MemoryThreshold = 40;

        var result = MemoryRules.CanRememberCorrectChurch(state);

        Assert.False(result);
    }

    [Fact]
    public void CanRememberCorrectChurch_ShouldReturnFalse_WhenDrunkennessIsTooHigh()
    {
        var state = GameStateTestFactory.CreateWithStats(hangover: 35, drunkenness: GameLimits.RequiredDrunkennessForMemory);
        state.MemoryThreshold = 40;

        var result = MemoryRules.CanRememberCorrectChurch(state);

        Assert.False(result);
    }
}
