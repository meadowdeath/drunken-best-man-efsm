using DrunkenBestManEFSM.Domain.Results.Blackjack;
using DrunkenBestManEFSM.Domain.Transitions.Blackjack;

namespace DrunkenBestManEFSM.Application.DTOs.Blackjack;

/// <summary>
/// Represents the result of a player's action in a blackjack round, including the action taken, the 
/// resulting status of the round, whether the round has finished, and the final result if it has finished. 
/// This DTO is used to communicate the outcome of a blackjack action back to the client after processing 
/// the action through the domain EFSM.
/// </summary>
public sealed class BlackjackActionResultDto
{
    public BlackjackActionResult ActionResult { get; init; } = new();

    public BlackjackStatusDto Status { get; init; } = new();

    public bool RoundFinished { get; init; }

    public BlackjackRoundResult? RoundResult { get; init; }
}
