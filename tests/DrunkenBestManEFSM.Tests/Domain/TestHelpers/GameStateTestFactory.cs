using DrunkenBestManEFSM.Domain.Enums;
using DrunkenBestManEFSM.Domain.Models;
using DrunkenBestManEFSM.Domain.Rules;

namespace DrunkenBestManEFSM.Tests.Domain.TestHelpers;

public static class GameStateTestFactory
{
    public static GameState CreateDefault() =>
        GameStateFactory.CreateInitialState(ChurchLocation.LostLoveParish, memoryThreshold: 40);

    public static GameState CreateAtLocation(Location location)
    {
        var state = CreateDefault();
        state.CurrentLocation = location;
        return state;
    }

    public static GameState CreateWithStats(
        int health = GameDefaults.InitialHealth,
        int hangover = GameDefaults.InitialHangover,
        int drunkenness = GameDefaults.InitialDrunkenness,
        int fuel = GameDefaults.InitialFuel,
        int remainingTime = GameDefaults.InitialRemainingTime,
        int money = GameDefaults.InitialMoney)
    {
        var state = CreateDefault();
        state.CharacterStats = new CharacterStats
        {
            Health = health,
            Hangover = hangover,
            Drunkenness = drunkenness,
            Fuel = fuel,
            RemainingTime = remainingTime,
            Money = money
        };

        return state;
    }

    public static GameState CreateAtCorrectChurchWithRings()
    {
        var state = CreateDefault();
        state.CurrentLocation = Location.LostLoveParish;
        state.HasRings = true;
        state.CharacterStats.Health = 50;
        state.CharacterStats.Hangover = 50;
        state.CharacterStats.RemainingTime = 30;
        return state;
    }
}
