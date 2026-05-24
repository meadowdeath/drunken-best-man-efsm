using DrunkenBestManEFSM.Domain.Enums;
using DrunkenBestManEFSM.Domain.Rules;
using DrunkenBestManEFSM.Tests.Domain.TestHelpers;

namespace DrunkenBestManEFSM.Tests.Domain.Balance;

public sealed class IdealPathBalanceTests
{
    [Fact]
    public void IdealPath_ShouldReachVictory_WhenPlayerManagesResourcesCorrectly()
    {
        var simulator = new GamePathSimulator();

        simulator.Travel(Location.GasStation, TravelMode.Drive);
        simulator.BuyElectrolytes();
        simulator.BuyFuel();
        simulator.Travel(Location.JewelryStore, TravelMode.Drive);
        simulator.PickUpRings();
        simulator.Travel(Location.LastGoodbyeSanctuary, TravelMode.Walk);
        simulator.EnterChurch();

        Assert.Equal(GameResult.Victory, simulator.State.Result);
        Assert.Equal(Location.Victory, simulator.State.CurrentLocation);
        Assert.True(simulator.State.HasRings);
        Assert.True(simulator.State.CharacterStats.Health > GameLimits.MinHealth);
        Assert.True(simulator.State.CharacterStats.RemainingTime > GameLimits.MinRemainingTime);
        Assert.True(simulator.State.CharacterStats.Hangover < GameLimits.MaxHangover);
    }
}
