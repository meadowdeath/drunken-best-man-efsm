using DrunkenBestManEFSM.Domain.Enums;

namespace DrunkenBestManEFSM.Domain.Results;

/// <summary>
/// Represents the result of a player action.
/// </summary>
public sealed class ActionResult
{
    public bool Success { get; set; }

    public Location PreviousLocation { get; set; }

    public Location CurrentLocation { get; set; }

    public GameResult GameResult { get; set; }

    public string MessageKey { get; set; } = string.Empty;

    public RandomEventResult? RandomEvent { get; set; }
}
