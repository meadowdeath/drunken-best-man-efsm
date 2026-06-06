using DrunkenBestManEFSM.Application.Contracts;
using DrunkenBestManEFSM.Application.DTOs.Blackjack;
using DrunkenBestManEFSM.Application.Results;
using DrunkenBestManEFSM.Domain.Enums.Blackjack;
using DrunkenBestManEFSM.Domain.Models.Blackjack;
using DrunkenBestManEFSM.Domain.Rules.Blackjack;
using DrunkenBestManEFSM.Domain.Transitions.Blackjack;

namespace DrunkenBestManEFSM.Application.Services.Blackjack;

/// <summary>
/// Service responsible for managing the state of a blackjack round within a player's session. 
/// It handles starting new rounds, maintaining the current game state, and providing a shuffled deck of cards. 
/// This service acts as the central point for storing and retrieving the state of an active blackjack round, 
/// allowing other services to interact with the current game state as players take actions and progress through 
/// the round.
/// </summary>
public sealed class BlackjackSessionService
{
    private readonly IRandomProvider randomProvider;
    private BlackjackGameState? currentState;
    private Domain.Results.Blackjack.BlackjackRoundResult? completedRoundResult;

    public BlackjackSessionService(IRandomProvider randomProvider)
    {
        this.randomProvider = randomProvider;
    }

    public UseCaseResult<BlackjackStatusDto> StartNewRound(int betAmount, int availableMoney)
    {
        completedRoundResult = null;

        if (!BlackjackBetRules.IsValidBet(betAmount, availableMoney))
        {
            return UseCaseResult<BlackjackStatusDto>.Fail("UseCase.Blackjack.InvalidBet");
        }

        var state = new BlackjackGameState
        {
            State = BlackjackState.WaitingForBet,
            PlayerHand = new BlackjackHand(),
            DealerHand = new BlackjackHand(),
            Deck = new BlackjackDeck(CreateShuffledDeck()),
            BetAmount = 0,
            Result = BlackjackResult.None
        };

        var actionResult = BlackjackTransitionResolver.Resolve(
            state,
            new BlackjackTransitionRequest
            {
                Action = BlackjackAction.PlaceBet,
                BetAmount = betAmount
            },
            availableMoney);

        if (!actionResult.Success)
        {
            return UseCaseResult<BlackjackStatusDto>.Fail(actionResult.MessageKey);
        }

        currentState = state;
        completedRoundResult = actionResult.RoundResult;

        return UseCaseResult<BlackjackStatusDto>.Ok(
            BlackjackQueryService.ToStatusDto(state),
            state.State == BlackjackState.Finished
                ? "UseCase.Blackjack.RoundFinished"
                : "UseCase.Blackjack.Started");
    }

    public bool HasActiveRound() =>
        currentState is not null;

    public BlackjackGameState? GetCurrentState() =>
        currentState;

    public Domain.Results.Blackjack.BlackjackRoundResult? GetCompletedRoundResult() =>
        completedRoundResult;

    public void StoreCompletedRoundResult(Domain.Results.Blackjack.BlackjackRoundResult? roundResult) =>
        completedRoundResult = roundResult;

    public void ClearRound() =>
        currentState = null;

    private IReadOnlyList<Card> CreateShuffledDeck()
    {
        var cards = Enum.GetValues<CardSuit>()
            .SelectMany(suit => Enum.GetValues<CardRank>().Select(rank => new Card(suit, rank)))
            .ToList();

        for (var index = cards.Count - 1; index > 0; index--)
        {
            var swapIndex = randomProvider.Next(0, index + 1);
            (cards[index], cards[swapIndex]) = (cards[swapIndex], cards[index]);
        }

        return cards;
    }
}
