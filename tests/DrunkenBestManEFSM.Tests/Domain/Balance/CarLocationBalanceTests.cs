using DrunkenBestManEFSM.Domain.Enums;
using DrunkenBestManEFSM.Domain.Rules;
using DrunkenBestManEFSM.Tests.Domain.TestHelpers;

namespace DrunkenBestManEFSM.Tests.Domain.Balance;

public sealed class CarLocationBalanceTests
{
    [Fact]
    public void Walking_ShouldLeaveCarBehind_AndDrivingShouldRequireCarAtCurrentLocation()
    {
        var simulator = new GamePathSimulator();

        simulator.Travel(Location.GasStation, TravelMode.Walk);
        var result = simulator.Travel(Location.JewelryStore, TravelMode.Drive);

        Assert.Equal(Location.GasStation, simulator.State.CurrentLocation);
        Assert.Equal(Location.StripClub, simulator.State.CarLocation);
        Assert.False(result.Success);
        Assert.Equal("Rules.Travel.CarNotHere", result.MessageKey);
    }

    [Fact]
    public void Driving_ShouldMoveCarWithPlayer()
    {
        var simulator = new GamePathSimulator();

        simulator.Travel(Location.GasStation, TravelMode.Drive);

        Assert.Equal(Location.GasStation, simulator.State.CurrentLocation);
        Assert.Equal(Location.GasStation, simulator.State.CarLocation);
    }
}
