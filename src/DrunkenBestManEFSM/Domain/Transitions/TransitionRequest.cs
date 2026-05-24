using DrunkenBestManEFSM.Domain.Enums;
using DrunkenBestManEFSM.Domain.Results;

namespace DrunkenBestManEFSM.Domain.Transitions;

/// <summary>
/// Represents an attempted player action.
/// </summary>
public sealed class TransitionRequest
{
    public ActionType ActionType { get; set; }

    public Location? Destination { get; set; }

    public TravelMode? TravelMode { get; set; }

    public RandomEventResult? RandomEvent { get; set; }
}
