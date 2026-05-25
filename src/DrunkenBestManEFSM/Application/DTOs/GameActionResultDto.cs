using DrunkenBestManEFSM.Domain.Results;

namespace DrunkenBestManEFSM.Application.DTOs;

public class GameActionResultDto
{
    public ActionResult? ActionResult { get; init; }

    public GameStatusDto? GameStatus { get; init; }
}
