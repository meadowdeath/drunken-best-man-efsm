using DrunkenBestManEFSM.Domain.Enums;
using DrunkenBestManEFSM.Domain.Rules;
using DrunkenBestManEFSM.Tests.Domain.TestHelpers;

namespace DrunkenBestManEFSM.Tests.Domain.Rules;

public sealed class TravelRulesTests
{
    [Fact]
    public void CanWalk_ShouldReturnTrue_WhenPlayerHasTime()
    {
        var state = GameStateTestFactory.CreateWithStats(remainingTime: 10);

        var result = TravelRules.CanWalk(state);

        Assert.True(result);
    }

    [Fact]
    public void CanWalk_ShouldReturnFalse_WhenPlayerHasNoTime()
    {
        var state = GameStateTestFactory.CreateWithStats(remainingTime: GameLimits.MinRemainingTime);

        var result = TravelRules.CanWalk(state);

        Assert.False(result);
    }

    [Fact]
    public void CanDrive_ShouldReturnTrue_WhenCarIsHereFuelIsEnoughTimeIsEnoughAndDrunkennessIsBelowLimit()
    {
        var state = GameStateTestFactory.CreateWithStats(fuel: 20, remainingTime: 20, drunkenness: GameLimits.DrivingDrunkennessLimit - 1);
        state.CurrentLocation = Location.StripClub;
        state.CarLocation = Location.StripClub;

        var result = TravelRules.CanTravel(state, TravelMode.Drive, timeCost: 5, fuelCost: 10);

        Assert.True(result);
    }

    [Fact]
    public void CanDrive_ShouldReturnFalse_WhenCarIsNotAtCurrentLocation()
    {
        var state = GameStateTestFactory.CreateWithStats(fuel: 20, remainingTime: 20, drunkenness: GameLimits.DrivingDrunkennessLimit - 1);
        state.CurrentLocation = Location.StripClub;
        state.CarLocation = Location.GasStation;

        var result = TravelRules.CanTravel(state, TravelMode.Drive, timeCost: 5, fuelCost: 10);

        Assert.False(result);
    }

    [Fact]
    public void CanDrive_ShouldReturnFalse_WhenFuelIsNotEnough()
    {
        var state = GameStateTestFactory.CreateWithStats(fuel: 5, remainingTime: 20, drunkenness: GameLimits.DrivingDrunkennessLimit - 1);

        var result = TravelRules.CanTravel(state, TravelMode.Drive, timeCost: 5, fuelCost: 10);

        Assert.False(result);
    }

    [Fact]
    public void CanDrive_ShouldReturnFalse_WhenTimeIsNotEnough()
    {
        var state = GameStateTestFactory.CreateWithStats(fuel: 20, remainingTime: 4, drunkenness: GameLimits.DrivingDrunkennessLimit - 1);

        var result = TravelRules.CanTravel(state, TravelMode.Drive, timeCost: 5, fuelCost: 10);

        Assert.False(result);
    }

    [Fact]
    public void CanDrive_ShouldReturnFalse_WhenDrunkennessIsAtOrAboveDrivingLimit()
    {
        var state = GameStateTestFactory.CreateWithStats(fuel: 20, remainingTime: 20, drunkenness: GameLimits.DrivingDrunkennessLimit);

        var result = TravelRules.CanTravel(state, TravelMode.Drive, timeCost: 5, fuelCost: 10);

        Assert.False(result);
    }
}
