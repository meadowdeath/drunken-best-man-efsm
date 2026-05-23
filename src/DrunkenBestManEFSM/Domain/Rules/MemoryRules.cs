using DrunkenBestManEFSM.Domain.Models;

namespace DrunkenBestManEFSM.Domain.Rules;

/// <summary>
/// Evaluates whether the player can remember the correct church.
/// </summary>
public static class MemoryRules
{
    public static bool CanRememberCorrectChurch(GameState state) =>
        state.CharacterStats.Hangover <= state.MemoryThreshold
        && state.CharacterStats.Drunkenness < GameLimits.RequiredDrunkennessForMemory;

    public static bool IsChurchAlreadyKnown(GameState state) =>
        state.CorrectChurchKnown;
}
