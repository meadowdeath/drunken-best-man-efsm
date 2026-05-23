using DrunkenBestManEFSM.Domain.Enums;
using DrunkenBestManEFSM.Domain.Models;

namespace DrunkenBestManEFSM.Domain.Rules;

/// <summary>
/// Evaluates church-related conditions without mutating state.
/// </summary>
public static class ChurchRules
{
    public static bool IsChurch(Location location) =>
        ChurchCatalog.IsChurch(location);

    public static bool IsCorrectChurch(GameState state, Location location) =>
        location == ChurchCatalog.ToLocation(state.CorrectChurch);

    public static bool CanEnterChurch(GameState state) =>
        IsChurch(state.CurrentLocation);

    public static bool CanWinAtCurrentChurch(GameState state) =>
        IsCorrectChurch(state, state.CurrentLocation)
        && state.HasRings
        && !GameOutcomeRules.IsDefeat(state);
}
