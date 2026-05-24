using DrunkenBestManEFSM.Application.DTOs;
using DrunkenBestManEFSM.Domain.Results;

namespace DrunkenBestManEFSM.Application.Results;

public sealed class UseCaseResult
{
    public bool Success { get; set; }

    public string MessageKey { get; set; } = string.Empty;

    public ActionResult? ActionResult { get; set; }

    public GameStatusDto? GameStatus { get; set; }
}
