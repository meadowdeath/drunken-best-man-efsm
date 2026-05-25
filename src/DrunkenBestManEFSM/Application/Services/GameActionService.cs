using DrunkenBestManEFSM.Application.DTOs;
using DrunkenBestManEFSM.Application.Results;
using DrunkenBestManEFSM.Domain.Enums;
using DrunkenBestManEFSM.Domain.Results;
using DrunkenBestManEFSM.Domain.Transitions;

namespace DrunkenBestManEFSM.Application.Services;

/// <summary>
/// Executes player actions through the domain EFSM transition resolver.
/// </summary>
public sealed class GameActionService
{
    private readonly GameSessionService sessionService;
    private readonly GameQueryService queryService;

    public GameActionService(GameSessionService sessionService, GameQueryService queryService)
    {
        this.sessionService = sessionService;
        this.queryService = queryService;
    }

    public UseCaseResult<GameActionResultDto> TravelTo(Location destination, TravelMode travelMode) =>
        Execute(new TransitionRequest
        {
            ActionType = ActionType.Travel,
            Destination = destination,
            TravelMode = travelMode
        });

    public UseCaseResult<GameActionResultDto> BuyElectrolytes() =>
        Execute(new TransitionRequest { ActionType = ActionType.BuyElectrolytes });

    public UseCaseResult<GameActionResultDto> BuyFuel() =>
        Execute(new TransitionRequest { ActionType = ActionType.BuyFuel });

    public UseCaseResult<GameActionResultDto> BuyAlcohol() =>
        Execute(new TransitionRequest { ActionType = ActionType.BuyAlcohol });

    public UseCaseResult<GameActionResultDto> PickUpRings() =>
        Execute(new TransitionRequest { ActionType = ActionType.PickUpRings });

    public UseCaseResult<GameActionResultDto> EnterChurch() =>
        Execute(new TransitionRequest { ActionType = ActionType.EnterChurch });

    public UseCaseResult<GameActionResultDto> CheckStats() =>
        Execute(new TransitionRequest { ActionType = ActionType.CheckStats });

    private UseCaseResult<GameActionResultDto> Execute(TransitionRequest request)
    {
        var state = sessionService.GetCurrentState();
        if (state is null)
        {
            return UseCaseResult<GameActionResultDto>.Fail("UseCase.Game.NoActiveGame");
        }

        var actionResult = EfsmTransitionResolver.Resolve(state, request);

        return CreateResult(actionResult);
    }

    private UseCaseResult<GameActionResultDto> CreateResult(ActionResult actionResult) =>
        new()
        {
            Success = actionResult.Success,
            MessageKey = actionResult.Success ? "UseCase.Action.Completed" : "UseCase.Action.Failed",
            Data = new GameActionResultDto
            {
                ActionResult = actionResult,
                GameStatus = queryService.GetStatus().Data
            }
        };
}
