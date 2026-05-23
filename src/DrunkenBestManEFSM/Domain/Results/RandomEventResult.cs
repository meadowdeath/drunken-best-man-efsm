using DrunkenBestManEFSM.Domain.Enums;

namespace DrunkenBestManEFSM.Domain.Results;

/// <summary>
/// Represents the outcome of a random event.
/// </summary>
public sealed class RandomEventResult
{
    public RandomEventType EventType { get; set; }

    public bool Occurred { get; set; }

    public int HealthChange { get; set; }

    public int HangoverChange { get; set; }

    public int DrunkennessChange { get; set; }

    public int MoneyChange { get; set; }

    public int RemainingTimeChange { get; set; }

    public string MessageKey { get; set; } = string.Empty;
}
