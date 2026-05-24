using DrunkenBestManEFSM.Domain.Enums;

namespace DrunkenBestManEFSM.Application.DTOs;

public sealed class AvailableActionDto
{
    public ActionType ActionType { get; set; }

    public bool IsAvailable { get; set; }

    public string LabelKey { get; set; } = string.Empty;

    public string? UnavailableReasonKey { get; set; }
}
