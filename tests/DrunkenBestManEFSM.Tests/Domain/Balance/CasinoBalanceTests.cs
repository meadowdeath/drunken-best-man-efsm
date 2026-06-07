using DrunkenBestManEFSM.Domain.Enums;
using DrunkenBestManEFSM.Domain.Maps;
using DrunkenBestManEFSM.Domain.Rules;
using DrunkenBestManEFSM.Domain.Rules.Blackjack;
using DrunkenBestManEFSM.Domain.Transitions;
using DrunkenBestManEFSM.Tests.Domain.TestHelpers;

namespace DrunkenBestManEFSM.Tests.Domain.Balance;

public sealed class CasinoBalanceTests
{
    [Fact]
    public void ApplyBlackjackRoundResult_ShouldIncreaseMoney_WhenPlayerWins()
    {
        var state = CreateCasinoState(money: 50, remainingTime: 40, hangover: 50, drunkenness: 50);
        var simulator = new GamePathSimulator(state);

        var result = simulator.ApplyBlackjackWin(betAmount: 10);

        Assert.True(result.Success);
        Assert.Equal(60, state.CharacterStats.Money);
        Assert.Equal(40 - BlackjackRulesConfiguration.BlackjackRoundTimeCost, state.CharacterStats.RemainingTime);
        Assert.Equal(Location.Casino, state.CurrentLocation);
        Assert.Equal(GameResult.InProgress, state.Result);
    }

    [Fact]
    public void ApplyBlackjackRoundResult_ShouldDecreaseMoney_WhenDealerWins()
    {
        var state = CreateCasinoState(money: 50, remainingTime: 40);
        var simulator = new GamePathSimulator(state);

        var result = simulator.ApplyBlackjackLoss(betAmount: 10);

        Assert.True(result.Success);
        Assert.Equal(40, state.CharacterStats.Money);
        Assert.Equal(40 - BlackjackRulesConfiguration.BlackjackRoundTimeCost, state.CharacterStats.RemainingTime);
        Assert.Equal(Location.Casino, state.CurrentLocation);
    }

    [Fact]
    public void ApplyBlackjackRoundResult_ShouldKeepMoney_WhenRoundDraws()
    {
        var state = CreateCasinoState(money: 50, remainingTime: 40);
        var simulator = new GamePathSimulator(state);

        var result = simulator.ApplyBlackjackDraw(betAmount: 10);

        Assert.True(result.Success);
        Assert.Equal(50, state.CharacterStats.Money);
        Assert.Equal(40 - BlackjackRulesConfiguration.BlackjackRoundTimeCost, state.CharacterStats.RemainingTime);
        Assert.Equal(Location.Casino, state.CurrentLocation);
    }

    [Fact]
    public void ApplyBlackjackRoundResult_ShouldNotConsumeTime_WhenPlayerExitsBeforeBet()
    {
        var state = CreateCasinoState(money: 50, remainingTime: 40, hangover: 50, drunkenness: 50);
        var simulator = new GamePathSimulator(state);

        var result = simulator.ExitBlackjack();

        Assert.True(result.Success);
        Assert.Equal(50, state.CharacterStats.Money);
        Assert.Equal(40, state.CharacterStats.RemainingTime);
        Assert.Equal(50, state.CharacterStats.Hangover);
        Assert.Equal(50, state.CharacterStats.Drunkenness);
        Assert.Equal(Location.Casino, state.CurrentLocation);
    }

    [Fact]
    public void PlayBlackjack_ShouldFail_WhenNotAtCasino()
    {
        var state = CreateCasinoState(location: Location.GasStation, money: 50, remainingTime: 40);
        var simulator = new GamePathSimulator(state);

        var result = simulator.ApplyBlackjackWin(betAmount: 10);

        Assert.False(result.Success);
        Assert.Equal("Rules.Blackjack.NotAtCasino", result.MessageKey);
        Assert.Equal(50, state.CharacterStats.Money);
        Assert.Equal(40, state.CharacterStats.RemainingTime);
        Assert.Equal(Location.GasStation, state.CurrentLocation);
    }

    [Fact]
    public void PlayBlackjack_ShouldFail_WhenRoundResultIsMissing()
    {
        var state = CreateCasinoState(money: 50, remainingTime: 40);

        var result = EfsmTransitionResolver.Resolve(
            state,
            new TransitionRequest { ActionType = ActionType.PlayBlackjack });

        Assert.False(result.Success);
        Assert.Equal("Rules.Blackjack.NoRoundResult", result.MessageKey);
        Assert.Equal(50, state.CharacterStats.Money);
        Assert.Equal(40, state.CharacterStats.RemainingTime);
        Assert.Equal(Location.Casino, state.CurrentLocation);
    }

    [Fact]
    public void PlayBlackjack_ShouldFail_WhenPlayerHasInsufficientMoneyForMinimumBet()
    {
        var state = CreateCasinoState(
            money: BlackjackRulesConfiguration.MinimumBet - 1,
            remainingTime: 40);
        var simulator = new GamePathSimulator(state);

        var result = simulator.ApplyBlackjackWin(betAmount: 10);

        Assert.False(result.Success);
        Assert.Equal("Rules.Blackjack.NotEnoughMoney", result.MessageKey);
        Assert.Equal(BlackjackRulesConfiguration.MinimumBet - 1, state.CharacterStats.Money);
        Assert.Equal(40, state.CharacterStats.RemainingTime);
        Assert.Equal(Location.Casino, state.CurrentLocation);
    }

    [Fact]
    public void PlayBlackjack_ShouldFail_WhenPlayerHasNoTimeForRound()
    {
        var startingTime = BlackjackRulesConfiguration.BlackjackRoundTimeCost;
        var state = CreateCasinoState(money: 50, remainingTime: startingTime);
        var simulator = new GamePathSimulator(state);

        var result = simulator.ApplyBlackjackWin(betAmount: 10);

        Assert.False(result.Success);
        Assert.Equal("Rules.Blackjack.NotEnoughTime", result.MessageKey);
        Assert.Equal(50, state.CharacterStats.Money);
        Assert.Equal(startingTime, state.CharacterStats.RemainingTime);
        Assert.Equal(Location.Casino, state.CurrentLocation);
    }

    [Fact]
    public void CompletedBlackjackRound_ShouldApplyPassiveEffects_WhenTimeCostIsGreaterThanZero()
    {
        var state = CreateCasinoState(money: 50, remainingTime: 40, hangover: 50, drunkenness: 50);
        var simulator = new GamePathSimulator(state);

        var result = simulator.ApplyBlackjackDraw(betAmount: 10);

        Assert.True(result.Success);
        Assert.Equal(40 - BlackjackRulesConfiguration.BlackjackRoundTimeCost, state.CharacterStats.RemainingTime);
        Assert.Equal(50 + GameEconomy.PassiveHangoverIncrease, state.CharacterStats.Hangover);
        Assert.Equal(50 - GameEconomy.PassiveDrunkennessReduction, state.CharacterStats.Drunkenness);
        Assert.Equal(GameResult.InProgress, state.Result);
    }

    [Fact]
    public void RepeatedBlackjackLosses_ShouldCreateResourcePressure()
    {
        var state = CreateCasinoState(money: 80, remainingTime: 50, hangover: 40, drunkenness: 50);
        var simulator = new GamePathSimulator(state);

        for (var round = 0; round < 3; round++)
        {
            var result = simulator.ApplyBlackjackLoss(betAmount: 10);
            Assert.True(result.Success);
        }

        Assert.Equal(50, state.CharacterStats.Money);
        Assert.Equal(50 - (3 * BlackjackRulesConfiguration.BlackjackRoundTimeCost), state.CharacterStats.RemainingTime);
        Assert.Equal(40 + (3 * GameEconomy.PassiveHangoverIncrease), state.CharacterStats.Hangover);
        Assert.Equal(50 - (3 * GameEconomy.PassiveDrunkennessReduction), state.CharacterStats.Drunkenness);
        Assert.Equal(Location.Casino, state.CurrentLocation);
    }

    [Fact]
    public void RepeatedCasinoRounds_ShouldEventuallyCauseDefeat_WhenTimeOrHangoverRunsOut()
    {
        var state = CreateCasinoState(money: 80, remainingTime: 30, hangover: GameLimits.MaxHangover - 5);
        var simulator = new GamePathSimulator(state);

        for (var round = 0; round < 5 && state.Result == GameResult.InProgress; round++)
        {
            simulator.ApplyBlackjackDraw(betAmount: 10);
        }

        Assert.Equal(GameResult.Defeat, state.Result);
        Assert.Equal(Location.Defeat, state.CurrentLocation);
    }

    [Fact]
    public void TravelMap_ShouldIncludeCasinoRoutes()
    {
        AssertRouteExists(Location.GasStation, Location.Casino);
        AssertRouteExists(Location.Casino, Location.GasStation);
        AssertRouteExists(Location.Bar, Location.Casino);
        AssertRouteExists(Location.Casino, Location.Bar);
        AssertRouteExists(Location.JewelryStore, Location.Casino);
        AssertRouteExists(Location.Casino, Location.JewelryStore);
    }

    [Fact]
    public void Casino_ShouldBeReachableWithoutBreakingMainPath()
    {
        var gasStationToCasino = TravelMap.GetRouteCost(Location.GasStation, Location.Casino, TravelMode.Walk);
        var casinoToJewelryStore = TravelMap.GetRouteCost(Location.Casino, Location.JewelryStore, TravelMode.Walk);
        var casinoToBar = TravelMap.GetRouteCost(Location.Casino, Location.Bar, TravelMode.Walk);

        Assert.NotNull(gasStationToCasino);
        Assert.NotNull(casinoToJewelryStore);
        Assert.NotNull(casinoToBar);
    }

    private static DrunkenBestManEFSM.Domain.Models.GameState CreateCasinoState(
        Location location = Location.Casino,
        int money = 50,
        int remainingTime = 40,
        int health = 50,
        int hangover = 50,
        int drunkenness = 50)
    {
        var state = GameStateTestFactory.CreateWithStats(
            health: health,
            hangover: hangover,
            drunkenness: drunkenness,
            remainingTime: remainingTime,
            money: money);

        state.CurrentLocation = location;
        state.CarLocation = location;
        state.Result = GameResult.InProgress;
        return state;
    }

    private static void AssertRouteExists(Location from, Location to)
    {
        Assert.True(TravelMap.RouteExists(from, to, TravelMode.Walk));
        Assert.True(TravelMap.RouteExists(from, to, TravelMode.Drive));
    }
}
