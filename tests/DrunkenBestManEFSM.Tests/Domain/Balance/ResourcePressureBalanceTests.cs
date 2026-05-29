using DrunkenBestManEFSM.Domain.Enums;
using DrunkenBestManEFSM.Domain.Rules;
using DrunkenBestManEFSM.Tests.Domain.TestHelpers;

namespace DrunkenBestManEFSM.Tests.Domain.Balance;

public sealed class ResourcePressureBalanceTests
{
    [Fact]
    public void BuyingElectrolytes_ShouldReduceHangoverButOnlyRestoreLimitedHealth()
    {
        var simulator = new GamePathSimulator();
        simulator.State.CurrentLocation = Location.GasStation;
        simulator.State.CharacterStats.Money = GameEconomy.ElectrolyteCost;
        simulator.State.CharacterStats.Health = 30;
        simulator.State.CharacterStats.Hangover = 80;
        var initialHealth = simulator.State.CharacterStats.Health;
        var initialHangover = simulator.State.CharacterStats.Hangover;

        simulator.BuyElectrolytes();

        Assert.Equal(initialHealth + GameEconomy.ElectrolyteHealthGain, simulator.State.CharacterStats.Health);
        Assert.True(simulator.State.CharacterStats.Hangover < initialHangover);
        Assert.True(GameEconomy.ElectrolyteHealthGain < GameEconomy.StripClubServiceMaxHealthGain);
    }

    [Fact]
    public void RestAtStripClub_ShouldRestoreProvidedHealthGainAndConsumeMoneyAndTime()
    {
        var simulator = CreateStripClubRecoveryReadySimulator();
        var initialHealth = simulator.State.CharacterStats.Health;
        var initialMoney = simulator.State.CharacterStats.Money;
        var initialTime = simulator.State.CharacterStats.RemainingTime;
        var healthGain = GameEconomy.StripClubServiceMaxHealthGain;

        simulator.RestAtStripClub(healthGain);

        Assert.Equal(initialHealth + healthGain, simulator.State.CharacterStats.Health);
        Assert.Equal(initialMoney - GameEconomy.StripClubServiceCost, simulator.State.CharacterStats.Money);
        Assert.Equal(initialTime - GameEconomy.StripClubServiceTimeCost, simulator.State.CharacterStats.RemainingTime);
    }

    [Fact]
    public void RestAtStripClub_ShouldClampHealthAtMaximum()
    {
        var simulator = CreateStripClubRecoveryReadySimulator();
        simulator.State.CharacterStats.Health = GameLimits.MaxHealth - 5;

        simulator.RestAtStripClub(GameEconomy.StripClubServiceMaxHealthGain);

        Assert.Equal(GameLimits.MaxHealth, simulator.State.CharacterStats.Health);
    }

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

    private static GamePathSimulator CreateStripClubRecoveryReadySimulator()
    {
        var simulator = new GamePathSimulator();
        simulator.State.CurrentLocation = Location.StripClub;
        simulator.State.CharacterStats.Money = GameEconomy.StripClubServiceCost;
        simulator.State.CharacterStats.Health = 40;
        simulator.State.CharacterStats.RemainingTime = 40;
        return simulator;
    }
}
