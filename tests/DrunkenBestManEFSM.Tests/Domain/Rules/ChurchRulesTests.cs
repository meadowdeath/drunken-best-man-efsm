using DrunkenBestManEFSM.Domain.Enums;
using DrunkenBestManEFSM.Domain.Rules;
using DrunkenBestManEFSM.Tests.Domain.TestHelpers;

namespace DrunkenBestManEFSM.Tests.Domain.Rules;

public sealed class ChurchRulesTests
{
    public static TheoryData<Location> ChurchLocations =>
    [
        Location.LostLoveParish,
        Location.ForbiddenRoseChapel,
        Location.LastGoodbyeSanctuary,
        Location.SecretsOfTheSoulChurch,
        Location.FinalDestinyCathedral
    ];

    public static TheoryData<Location> NonChurchLocations =>
    [
        Location.StripClub,
        Location.GasStation,
        Location.JewelryStore,
        Location.Bar,
        Location.Victory,
        Location.Defeat
    ];

    [Theory]
    [MemberData(nameof(ChurchLocations))]
    public void IsChurch_ShouldReturnTrue_ForAllChurchLocations(Location location)
    {
        var result = ChurchRules.IsChurch(location);

        Assert.True(result);
    }

    [Theory]
    [MemberData(nameof(NonChurchLocations))]
    public void IsChurch_ShouldReturnFalse_ForNonChurchLocations(Location location)
    {
        var result = ChurchRules.IsChurch(location);

        Assert.False(result);
    }

    [Fact]
    public void IsCorrectChurch_ShouldReturnTrue_WhenLocationMatchesCorrectChurch()
    {
        var state = GameStateTestFactory.CreateDefault();

        var result = ChurchRules.IsCorrectChurch(state, Location.LostLoveParish);

        Assert.True(result);
    }

    [Fact]
    public void IsCorrectChurch_ShouldReturnFalse_WhenLocationDoesNotMatchCorrectChurch()
    {
        var state = GameStateTestFactory.CreateDefault();

        var result = ChurchRules.IsCorrectChurch(state, Location.ForbiddenRoseChapel);

        Assert.False(result);
    }

    [Fact]
    public void CanWinAtCurrentChurch_ShouldReturnTrue_WhenAtCorrectChurchWithRings()
    {
        var state = GameStateTestFactory.CreateAtCorrectChurchWithRings();

        var result = ChurchRules.CanWinAtCurrentChurch(state);

        Assert.True(result);
    }

    [Fact]
    public void CanWinAtCurrentChurch_ShouldReturnFalse_WhenAtWrongChurch()
    {
        var state = GameStateTestFactory.CreateAtCorrectChurchWithRings();
        state.CurrentLocation = Location.ForbiddenRoseChapel;

        var result = ChurchRules.CanWinAtCurrentChurch(state);

        Assert.False(result);
    }

    [Fact]
    public void CanWinAtCurrentChurch_ShouldReturnFalse_WhenMissingRings()
    {
        var state = GameStateTestFactory.CreateAtCorrectChurchWithRings();
        state.HasRings = false;

        var result = ChurchRules.CanWinAtCurrentChurch(state);

        Assert.False(result);
    }
}
