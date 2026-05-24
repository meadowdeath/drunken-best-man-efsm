using DrunkenBestManEFSM.Domain.Enums;
using DrunkenBestManEFSM.Domain.Rules;
using DrunkenBestManEFSM.Tests.Domain.TestHelpers;

namespace DrunkenBestManEFSM.Tests.Domain.Balance;

public sealed class FailurePathBalanceTests
{
    [Fact]
    public void PathWithoutElectrolytes_ShouldLoseByDehydrationOrBecomeUnviable()
    {
        var simulator = new GamePathSimulator();

        for (var step = 0; step < 10 && simulator.State.Result == GameResult.InProgress; step++)
        {
            simulator.Travel(Location.Bar, TravelMode.Walk);
            if (simulator.State.Result != GameResult.InProgress)
            {
                break;
            }

            simulator.Travel(Location.StripClub, TravelMode.Walk);
        }

        Assert.True(
            simulator.State.CharacterStats.Hangover >= GameLimits.MaxHangover
            || simulator.State.Result == GameResult.Defeat);
    }

    [Fact]
    public void CorrectChurchWithoutRings_ShouldNotWin()
    {
        var simulator = new GamePathSimulator();
        simulator.State.CurrentLocation = Location.LastGoodbyeSanctuary;
        simulator.State.HasRings = false;

        var result = simulator.EnterChurch();

        Assert.NotEqual(GameResult.Victory, simulator.State.Result);
        Assert.NotEqual(Location.Victory, simulator.State.CurrentLocation);
        Assert.Equal("Actions.Church.Enter.CorrectChurchMissingRings", result.MessageKey);
    }

    [Fact]
    public void EnteringWrongChurch_ShouldConsumeTimeAndNotWin()
    {
        var simulator = new GamePathSimulator();
        simulator.State.CurrentLocation = Location.LostLoveParish;
        simulator.State.HasRings = true;
        simulator.State.CharacterStats.RemainingTime = 40;
        var initialTime = simulator.State.CharacterStats.RemainingTime;

        simulator.EnterChurch();

        Assert.NotEqual(GameResult.Victory, simulator.State.Result);
        Assert.True(simulator.State.CharacterStats.RemainingTime < initialTime);
        Assert.NotEqual(Location.Victory, simulator.State.CurrentLocation);
    }
}
