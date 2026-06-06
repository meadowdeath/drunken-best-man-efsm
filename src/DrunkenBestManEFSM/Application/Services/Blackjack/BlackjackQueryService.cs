using DrunkenBestManEFSM.Application.DTOs.Blackjack;
using DrunkenBestManEFSM.Application.Results;
using DrunkenBestManEFSM.Domain.Enums.Blackjack;
using DrunkenBestManEFSM.Domain.Models.Blackjack;
using DrunkenBestManEFSM.Domain.Rules.Blackjack;

namespace DrunkenBestManEFSM.Application.Services.Blackjack;

/// <summary>
/// Service responsible for querying the current status of a blackjack round. It retrieves the current game state
/// from the session service and constructs a status DTO that includes the player's hand, the dealer's
/// visible cards, the current bet, and the result if the round has finished. This service is used to provide
/// the client with up-to-date information about the ongoing blackjack round.
/// </summary>
public sealed class BlackjackQueryService
{
    private readonly BlackjackSessionService sessionService;

    public BlackjackQueryService(BlackjackSessionService sessionService)
    {
        this.sessionService = sessionService;
    }

    public UseCaseResult<BlackjackStatusDto> GetStatus()
    {
        var state = sessionService.GetCurrentState();
        return state is null
            ? UseCaseResult<BlackjackStatusDto>.Fail("UseCase.Blackjack.NoActiveRound")
            : UseCaseResult<BlackjackStatusDto>.Ok(ToStatusDto(state), "UseCase.Blackjack.ActionCompleted");
    }

    public static BlackjackStatusDto ToStatusDto(BlackjackGameState state)
    {
        var revealDealerHand = ShouldRevealDealerHand(state.State);
        var visibleDealerCards = revealDealerHand
            ? state.DealerHand.Cards
            : state.DealerHand.Cards.Take(1);

        return new BlackjackStatusDto
        {
            State = state.State,
            PlayerCards = state.PlayerHand.Cards.Select(ToCardDto).ToList(),
            VisibleDealerCards = visibleDealerCards.Select(ToCardDto).ToList(),
            PlayerValue = BlackjackHandRules.CalculateValue(state.PlayerHand),
            DealerValue = revealDealerHand ? BlackjackHandRules.CalculateValue(state.DealerHand) : null,
            BetAmount = state.BetAmount,
            Result = state.Result,
            IsFinished = state.State == BlackjackState.Finished
        };
    }

    private static bool ShouldRevealDealerHand(BlackjackState state) =>
        state is BlackjackState.DealerTurn or BlackjackState.Resolving or BlackjackState.Finished;

    private static BlackjackCardDto ToCardDto(Card card) =>
        new()
        {
            Suit = card.Suit,
            Rank = card.Rank,
            DisplayKey = $"Blackjack.Card.{card.Rank}.{card.Suit}"
        };
}
