using DrunkenBestManEFSM.Domain.Enums;

namespace DrunkenBestManEFSM.Application.DTOs;

public sealed class GameStatusDto
{
    public Location CurrentLocation { get; set; }

    public Location CarLocation { get; set; }

    public int Health { get; set; }

    public int Hangover { get; set; }

    public int Drunkenness { get; set; }

    public int Fuel { get; set; }

    public int RemainingTime { get; set; }

    public int Money { get; set; }

    public bool HasRings { get; set; }

    public bool CorrectChurchKnown { get; set; }

    public ChurchLocation? CorrectChurchToDisplay { get; set; }

    public GameResult Result { get; set; }
}
