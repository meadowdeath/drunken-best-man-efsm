using DrunkenBestManEFSM.Domain.Enums;

namespace DrunkenBestManEFSM.Application.DTOs;

public sealed class AvailableDestinationDto
{
    public Location Destination { get; set; }

    public bool CanWalk { get; set; }

    public int WalkTimeCost { get; set; }

    public string? WalkUnavailableReasonKey { get; set; }

    public bool CanDrive { get; set; }

    public int DriveTimeCost { get; set; }

    public int DriveFuelCost { get; set; }

    public string? DriveUnavailableReasonKey { get; set; }
}
