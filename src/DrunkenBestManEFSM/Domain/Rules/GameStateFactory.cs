using DrunkenBestManEFSM.Domain.Enums;
using DrunkenBestManEFSM.Domain.Models;

namespace DrunkenBestManEFSM.Domain.Rules;

/// <summary>
/// Creates initial game state instances from domain configuration values.
/// </summary>
public static class GameStateFactory
{
    public static GameState CreateInitialState(ChurchLocation correctChurch, int memoryThreshold) =>
        new()
        {
            CurrentLocation = GameDefaults.InitialLocation,
            CarLocation = GameDefaults.InitialCarLocation,
            CharacterStats = new CharacterStats
            {
                Health = GameDefaults.InitialHealth,
                Hangover = GameDefaults.InitialHangover,
                Drunkenness = GameDefaults.InitialDrunkenness,
                Fuel = GameDefaults.InitialFuel,
                RemainingTime = GameDefaults.InitialRemainingTime,
                Money = GameDefaults.InitialMoney
            },
            HasRings = GameDefaults.InitialHasRings,
            CorrectChurchKnown = GameDefaults.InitialCorrectChurchKnown,
            CorrectChurch = correctChurch,
            MemoryThreshold = memoryThreshold,
            Result = GameResult.InProgress
        };
}
