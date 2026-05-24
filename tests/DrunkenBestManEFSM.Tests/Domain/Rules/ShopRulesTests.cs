using DrunkenBestManEFSM.Domain.Rules;
using DrunkenBestManEFSM.Tests.Domain.TestHelpers;

namespace DrunkenBestManEFSM.Tests.Domain.Rules;

public sealed class ShopRulesTests
{
    [Fact]
    public void CanBuyElectrolytes_ShouldReturnTrue_WhenMoneyIsEnough()
    {
        var state = GameStateTestFactory.CreateWithStats(money: GameEconomy.ElectrolyteCost);

        var result = ShopRules.CanBuyElectrolytes(state);

        Assert.True(result);
    }

    [Fact]
    public void CanBuyElectrolytes_ShouldReturnFalse_WhenMoneyIsNotEnough()
    {
        var state = GameStateTestFactory.CreateWithStats(money: GameEconomy.ElectrolyteCost - 1);

        var result = ShopRules.CanBuyElectrolytes(state);

        Assert.False(result);
    }

    [Fact]
    public void CanBuyFuel_ShouldReturnTrue_WhenMoneyIsEnoughAndFuelIsNotFull()
    {
        var state = GameStateTestFactory.CreateWithStats(money: GameEconomy.FuelCost, fuel: GameLimits.MaxFuel - 1);

        var result = ShopRules.CanBuyFuel(state);

        Assert.True(result);
    }

    [Fact]
    public void CanBuyFuel_ShouldReturnFalse_WhenFuelIsFull()
    {
        var state = GameStateTestFactory.CreateWithStats(money: GameEconomy.FuelCost, fuel: GameLimits.MaxFuel);

        var result = ShopRules.CanBuyFuel(state);

        Assert.False(result);
    }

    [Fact]
    public void CanBuyFuel_ShouldReturnFalse_WhenMoneyIsNotEnough()
    {
        var state = GameStateTestFactory.CreateWithStats(money: GameEconomy.FuelCost - 1, fuel: GameLimits.MaxFuel - 1);

        var result = ShopRules.CanBuyFuel(state);

        Assert.False(result);
    }

    [Fact]
    public void CanBuyAlcohol_ShouldReturnTrue_WhenMoneyHealthAndTimeAllowIt()
    {
        var state = GameStateTestFactory.CreateWithStats(
            money: GameEconomy.AlcoholCost,
            health: GameEconomy.AlcoholHealthCost + 1,
            remainingTime: GameLimits.MaxRemainingTime - 1,
            drunkenness: GameLimits.MaxDrunkenness - 1);

        var result = ShopRules.CanBuyAlcohol(state);

        Assert.True(result);
    }

    [Fact]
    public void CanBuyAlcohol_ShouldReturnFalse_WhenMoneyIsNotEnough()
    {
        var state = GameStateTestFactory.CreateWithStats(money: GameEconomy.AlcoholCost - 1);

        var result = ShopRules.CanBuyAlcohol(state);

        Assert.False(result);
    }

    [Fact]
    public void CanBuyAlcohol_ShouldReturnFalse_WhenHealthIsTooLow()
    {
        var state = GameStateTestFactory.CreateWithStats(health: GameEconomy.AlcoholHealthCost);

        var result = ShopRules.CanBuyAlcohol(state);

        Assert.False(result);
    }

    [Fact]
    public void CanBuyAlcohol_ShouldReturnFalse_WhenRemainingTimeIsAtMaximum()
    {
        var state = GameStateTestFactory.CreateWithStats(remainingTime: GameLimits.MaxRemainingTime);

        var result = ShopRules.CanBuyAlcohol(state);

        Assert.False(result);
    }
}
