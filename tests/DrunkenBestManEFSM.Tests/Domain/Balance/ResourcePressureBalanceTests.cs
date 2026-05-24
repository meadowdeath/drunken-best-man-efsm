using DrunkenBestManEFSM.Domain.Enums;
using DrunkenBestManEFSM.Domain.Rules;
using DrunkenBestManEFSM.Tests.Domain.TestHelpers;

namespace DrunkenBestManEFSM.Tests.Domain.Balance;

public sealed class ResourcePressureBalanceTests
{
    [Fact]
    public void AlcoholAbuse_ShouldIncreaseDrunkennessAndRiskMobility()
    {
        var simulator = new GamePathSimulator();
        simulator.State.CurrentLocation = Location.Bar;
        simulator.State.CarLocation = Location.Bar;
        simulator.State.CharacterStats.Money = 200;
        simulator.State.CharacterStats.Health = 100;
        simulator.State.CharacterStats.Hangover = 80;
        simulator.State.CharacterStats.Drunkenness = 20;
        simulator.State.CharacterStats.RemainingTime = 40;
        var initialHealth = simulator.State.CharacterStats.Health;
        var initialDrunkenness = simulator.State.CharacterStats.Drunkenness;

        for (var step = 0; step < 4 && ShopRules.CanBuyAlcohol(simulator.State); step++)
        {
            simulator.BuyAlcohol();
        }

        Assert.True(simulator.State.CharacterStats.Drunkenness > initialDrunkenness);
        Assert.True(simulator.State.CharacterStats.Health < initialHealth);
        Assert.True(simulator.State.CharacterStats.RemainingTime <= GameLimits.MaxRemainingTime);
        Assert.False(TravelRules.CanTravel(simulator.State, TravelMode.Drive, timeCost: 1, fuelCost: 1));
    }

    [Fact]
    public void BuyingAlcohol_ShouldIncreaseRemainingTimeUpToMaximum()
    {
        var simulator = CreateAlcoholReadySimulator(remainingTime: 40);

        simulator.BuyAlcohol();

        Assert.True(simulator.State.CharacterStats.RemainingTime > 40);
    }

    [Fact]
    public void BuyingAlcohol_ShouldClampRemainingTimeAtMaximum()
    {
        var simulator = CreateAlcoholReadySimulator(remainingTime: 73);

        simulator.BuyAlcohol();

        Assert.Equal(GameLimits.MaxRemainingTime, simulator.State.CharacterStats.RemainingTime);
    }

    private static GamePathSimulator CreateAlcoholReadySimulator(int remainingTime)
    {
        var simulator = new GamePathSimulator();
        simulator.State.CurrentLocation = Location.Bar;
        simulator.State.CharacterStats.Money = GameEconomy.AlcoholCost;
        simulator.State.CharacterStats.Health = GameEconomy.AlcoholHealthCost + 20;
        simulator.State.CharacterStats.Hangover = 80;
        simulator.State.CharacterStats.Drunkenness = 20;
        simulator.State.CharacterStats.RemainingTime = remainingTime;
        return simulator;
    }
}
