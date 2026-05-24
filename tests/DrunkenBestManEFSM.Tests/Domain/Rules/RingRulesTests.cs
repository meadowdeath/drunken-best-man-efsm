using DrunkenBestManEFSM.Domain.Enums;
using DrunkenBestManEFSM.Domain.Rules;
using DrunkenBestManEFSM.Tests.Domain.TestHelpers;

namespace DrunkenBestManEFSM.Tests.Domain.Rules;

public sealed class RingRulesTests
{
    [Fact]
    public void CanPickUpRings_ShouldReturnTrue_WhenAtJewelryStoreAndDoesNotHaveRings()
    {
        var state = GameStateTestFactory.CreateAtLocation(Location.JewelryStore);
        state.HasRings = false;

        var result = RingRules.CanPickUpRings(state);

        Assert.True(result);
    }

    [Fact]
    public void CanPickUpRings_ShouldReturnFalse_WhenNotAtJewelryStore()
    {
        var state = GameStateTestFactory.CreateAtLocation(Location.GasStation);
        state.HasRings = false;

        var result = RingRules.CanPickUpRings(state);

        Assert.False(result);
    }

    [Fact]
    public void CanPickUpRings_ShouldReturnFalse_WhenAlreadyHasRings()
    {
        var state = GameStateTestFactory.CreateAtLocation(Location.JewelryStore);
        state.HasRings = true;

        var result = RingRules.CanPickUpRings(state);

        Assert.False(result);
    }
}
