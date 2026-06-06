using DrunkenBestManEFSM.Application.DTOs.Blackjack;
using DrunkenBestManEFSM.Application.Results;
using DrunkenBestManEFSM.Domain.Enums.Blackjack;
using DrunkenBestManEFSM.Domain.Models.Blackjack;
using DrunkenBestManEFSM.Domain.Transitions.Blackjack;

namespace DrunkenBestManEFSM.Application.Services.Blackjack;

/// <summary>
/// Service responsible for executing player actions in a blackjack round by interacting with the domain EFSM.
/// It processes actions like Hit, Stand, and Leave, advances the dealer's turn as necessary, and
/// constructs the result DTO to communicate the outcome back to the client.
/// </summary>

public sealed class BlackjackActionService
{
    private const int MaxDealerSteps = 20;

    private readonly BlackjackSessionService sessionService;

    public BlackjackActionService(BlackjackSessionService sessionService)
    {
        this.sessionService = sessionService;
    }

    public UseCaseResult<BlackjackActionResultDto> Hit()
    {
        var state = sessionService.GetCurrentState();
        if (state is null)
        {
            return UseCaseResult<BlackjackActionResultDto>.Fail("UseCase.Blackjack.NoActiveRound");
        }

        var actionResult = BlackjackTransitionResolver.Resolve(
            state,
            new BlackjackTransitionRequest { Action = BlackjackAction.Hit },
            availableMoney: 0);

        if (actionResult.Success && state.State == BlackjackState.DealerTurn)
        {
            actionResult = AdvanceDealerUntilFinished(state, actionResult);
            if (state.State == BlackjackState.DealerTurn)
            {
                return CreateDealerTurnLimitFailure(state, actionResult);
            }
        }

        return CreateResult(state, actionResult);
    }

    public UseCaseResult<BlackjackActionResultDto> Stand()
    {
        var state = sessionService.GetCurrentState();
        if (state is null)
        {
            return UseCaseResult<BlackjackActionResultDto>.Fail("UseCase.Blackjack.NoActiveRound");
        }

        var actionResult = BlackjackTransitionResolver.Resolve(
            state,
            new BlackjackTransitionRequest { Action = BlackjackAction.Stand },
            availableMoney: 0);

        if (!actionResult.Success)
        {
            return CreateResult(state, actionResult);
        }

        actionResult = AdvanceDealerUntilFinished(state, actionResult);

        if (state.State == BlackjackState.DealerTurn)
        {
            return CreateDealerTurnLimitFailure(state, actionResult);
        }

        return CreateResult(state, actionResult);
    }

    public UseCaseResult<BlackjackActionResultDto> Leave()
    {
        var state = sessionService.GetCurrentState();
        if (state is null)
        {
            return UseCaseResult<BlackjackActionResultDto>.Fail("UseCase.Blackjack.NoActiveRound");
        }

        var actionResult = BlackjackTransitionResolver.Resolve(
            state,
            new BlackjackTransitionRequest { Action = BlackjackAction.Leave },
            availableMoney: 0);

        return CreateResult(state, actionResult);
    }

    private static BlackjackActionResult AdvanceDealerUntilFinished(
        BlackjackGameState state,
        BlackjackActionResult currentResult)
    {
        var actionResult = currentResult;
        var dealerSteps = 0;

        while (state.State == BlackjackState.DealerTurn && dealerSteps < MaxDealerSteps)
        {
            actionResult = BlackjackTransitionResolver.AdvanceDealerTurn(state);
            dealerSteps++;
        }

        return actionResult;
    }

    private UseCaseResult<BlackjackActionResultDto> CreateResult(
        BlackjackGameState state,
        BlackjackActionResult actionResult)
    {
        var status = BlackjackQueryService.ToStatusDto(state);
        var isFinished = state.State == BlackjackState.Finished;
        var roundResult = actionResult.RoundResult;

        if (isFinished)
        {
            sessionService.ClearRound();
        }

        return new UseCaseResult<BlackjackActionResultDto>
        {
            Success = actionResult.Success,
            MessageKey = !actionResult.Success
                ? actionResult.MessageKey
                : isFinished
                ? "UseCase.Blackjack.RoundFinished"
                : "UseCase.Blackjack.ActionCompleted",
            Data = new BlackjackActionResultDto
            {
                ActionResult = actionResult,
                Status = status,
                RoundFinished = isFinished,
                RoundResult = roundResult
            }
        };
    }

    private static UseCaseResult<BlackjackActionResultDto> CreateDealerTurnLimitFailure(
        BlackjackGameState state,
        BlackjackActionResult actionResult) =>
        new()
        {
            Success = false,
            MessageKey = "UseCase.Blackjack.DealerTurnLimitExceeded",
            Data = new BlackjackActionResultDto
            {
                ActionResult = actionResult,
                Status = BlackjackQueryService.ToStatusDto(state),
                RoundFinished = false
            }
        };
}
