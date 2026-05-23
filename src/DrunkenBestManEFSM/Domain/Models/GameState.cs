using DrunkenBestManEFSM.Domain.Enums;

namespace DrunkenBestManEFSM.Domain.Models;

/// <summary>
/// Represents the extended state of the EFSM.
/// </summary>
public sealed class GameState
{
    public Location CurrentLocation { get; set; }

    public Location CarLocation { get; set; }

    public CharacterStats CharacterStats { get; set; } = new();

    public bool HasRings { get; set; }

    public bool CorrectChurchKnown { get; set; }

    public ChurchLocation CorrectChurch { get; set; }

    public int MemoryThreshold { get; set; }

    public GameResult Result { get; set; }
}
