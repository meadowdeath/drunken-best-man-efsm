using DrunkenBestManEFSM.Domain.Enums.Blackjack;
using DrunkenBestManEFSM.Domain.Models.Blackjack;
using DrunkenBestManEFSM.Domain.Results.Blackjack;
using DrunkenBestManEFSM.Domain.Rules.Blackjack;

namespace DrunkenBestManEFSM.Domain.Transitions.Blackjack;

/// <summary>
/// Coordinates state transitions for the nested Blackjack EFSM.
/// </summary>
public static class BlackjackTransitionResolver
{
    public static BlackjackActionResult Resolve(
        BlackjackGameState state,
        BlackjackTransitionRequest request,
        int availableMoney) =>
        request.Action switch
        {
            BlackjackAction.PlaceBet => ResolvePlaceBet(state, request, availableMoney),
            BlackjackAction.Hit => ResolveHit(state),
            BlackjackAction.Stand => ResolveStand(state),
            BlackjackAction.Leave => ResolveLeave(state),
            _ => CreateFailureResult(state, state.State, "Blackjack.InvalidAction")
        };

    public static BlackjackActionResult AdvanceDealerTurn(BlackjackGameState state)
    {
        var previousState = state.State;

        if (state.State != BlackjackState.DealerTurn)
        {
            return CreateFailureResult(state, previousState, "Blackjack.InvalidState");
        }

        if (BlackjackDealerRules.ShouldHit(state.DealerHand))
        {
            state.DealerHand.AddCard(state.Deck.Draw());

            if (BlackjackHandRules.IsBust(state.DealerHand))
            {
                state.Result = BlackjackResult.PlayerWin;
                state.State = BlackjackState.Finished;
                return CreateSuccessResult(
                    state,
                    previousState,
                    "Blackjack.Round.PlayerWin",
                    CreateRoundResult(state));
            }

            return CreateSuccessResult(state, previousState, "Blackjack.Dealer.Draw");
        }

        state.State = BlackjackState.Resolving;
        return ResolveCurrentRound(state, previousState);
    }

    public static BlackjackActionResult ResolveCurrentRound(BlackjackGameState state)
    {
        var previousState = state.State;
        return ResolveCurrentRound(state, previousState);
    }

    private static BlackjackActionResult ResolvePlaceBet(
        BlackjackGameState state,
        BlackjackTransitionRequest request,
        int availableMoney)
    {
        var previousState = state.State;

        if (state.State != BlackjackState.WaitingForBet)
        {
            return CreateFailureResult(state, previousState, "Blackjack.InvalidState");
        }

        if (request.BetAmount is null || !BlackjackBetRules.IsValidBet(request.BetAmount.Value, availableMoney))
        {
            return CreateFailureResult(state, previousState, "Blackjack.PlaceBet.InvalidBet");
        }

        state.BetAmount = request.BetAmount.Value;
        state.State = BlackjackState.InitialDeal;
        DealInitialCards(state);

        var playerHasNaturalBlackjack = BlackjackHandRules.IsNaturalBlackjack(state.PlayerHand);
        var dealerHasNaturalBlackjack = BlackjackHandRules.IsNaturalBlackjack(state.DealerHand);

        if (playerHasNaturalBlackjack || dealerHasNaturalBlackjack)
        {
            state.Result = BlackjackOutcomeRules.ResolveOutcome(state.PlayerHand, state.DealerHand);
            state.State = BlackjackState.Finished;
            return CreateSuccessResult(
                state,
                previousState,
                GetResultMessageKey(state.Result),
                CreateRoundResult(state));
        }

        state.State = BlackjackState.PlayerTurn;
        return CreateSuccessResult(state, previousState, "Blackjack.PlaceBet.Success");
    }

    private static BlackjackActionResult ResolveLeave(BlackjackGameState state)
    {
        var previousState = state.State;

        if (state.State != BlackjackState.WaitingForBet)
        {
            return CreateFailureResult(state, previousState, "Blackjack.InvalidState");
        }

        state.Result = BlackjackResult.Exited;
        state.State = BlackjackState.Finished;

        return CreateSuccessResult(
            state,
            previousState,
            "Blackjack.Leave.Success",
            CreateRoundResult(state));
    }

    private static BlackjackActionResult ResolveHit(BlackjackGameState state)
    {
        var previousState = state.State;

        if (state.State != BlackjackState.PlayerTurn)
        {
            return CreateFailureResult(state, previousState, "Blackjack.InvalidState");
        }

        state.PlayerHand.AddCard(state.Deck.Draw());

        if (BlackjackHandRules.IsBust(state.PlayerHand))
        {
            state.Result = BlackjackResult.DealerWin;
            state.State = BlackjackState.Finished;
            return CreateSuccessResult(
                state,
                previousState,
                "Blackjack.Hit.Bust",
                CreateRoundResult(state));
        }

        if (BlackjackHandRules.HasTwentyOne(state.PlayerHand))
        {
            state.State = BlackjackState.DealerTurn;
            return CreateSuccessResult(state, previousState, "Blackjack.Hit.ReachedTwentyOne");
        }

        return CreateSuccessResult(state, previousState, "Blackjack.Hit.Success");
    }

    private static BlackjackActionResult ResolveStand(BlackjackGameState state)
    {
        var previousState = state.State;

        if (state.State != BlackjackState.PlayerTurn)
        {
            return CreateFailureResult(state, previousState, "Blackjack.InvalidState");
        }

        state.State = BlackjackState.DealerTurn;
        return CreateSuccessResult(state, previousState, "Blackjack.Stand.Success");
    }

    private static BlackjackActionResult ResolveCurrentRound(BlackjackGameState state, BlackjackState previousState)
    {
        if (state.State != BlackjackState.Resolving && state.State != BlackjackState.DealerTurn)
        {
            return CreateFailureResult(state, previousState, "Blackjack.InvalidState");
        }

        state.Result = BlackjackOutcomeRules.ResolveOutcome(state.PlayerHand, state.DealerHand);
        state.State = BlackjackState.Finished;

        return CreateSuccessResult(
            state,
            previousState,
            GetResultMessageKey(state.Result),
            CreateRoundResult(state));
    }

    private static void DealInitialCards(BlackjackGameState state)
    {
        state.PlayerHand.AddCard(state.Deck.Draw());
        state.DealerHand.AddCard(state.Deck.Draw());
        state.PlayerHand.AddCard(state.Deck.Draw());
        state.DealerHand.AddCard(state.Deck.Draw());
    }

    private static BlackjackRoundResult CreateRoundResult(BlackjackGameState state) =>
        new()
        {
            Result = state.Result,
            BetAmount = state.BetAmount,
            MoneyChange = GetMoneyChange(state.Result, state.BetAmount),
            TimeCost = state.Result == BlackjackResult.Exited
                ? BlackjackRulesConfiguration.BlackjackExitTimeCost
                : BlackjackRulesConfiguration.BlackjackRoundTimeCost
        };

    private static int GetMoneyChange(BlackjackResult result, int betAmount) =>
        result switch
        {
            BlackjackResult.PlayerWin => betAmount,
            BlackjackResult.PlayerBlackjack => betAmount,
            BlackjackResult.DealerWin => -betAmount,
            BlackjackResult.DealerBlackjack => -betAmount,
            BlackjackResult.Draw => 0,
            BlackjackResult.Exited => 0,
            _ => 0
        };

    private static string GetResultMessageKey(BlackjackResult result) =>
        result switch
        {
            BlackjackResult.PlayerWin => "Blackjack.Round.PlayerWin",
            BlackjackResult.PlayerBlackjack => "Blackjack.Round.PlayerBlackjack",
            BlackjackResult.DealerWin => "Blackjack.Round.DealerWin",
            BlackjackResult.DealerBlackjack => "Blackjack.Round.DealerBlackjack",
            BlackjackResult.Draw => "Blackjack.Round.Draw",
            BlackjackResult.Exited => "Blackjack.Leave.Success",
            _ => "Blackjack.InvalidAction"
        };

    private static BlackjackActionResult CreateFailureResult(
        BlackjackGameState state,
        BlackjackState previousState,
        string messageKey) =>
        new()
        {
            Success = false,
            PreviousState = previousState,
            CurrentState = state.State,
            Result = state.Result,
            MessageKey = messageKey
        };

    private static BlackjackActionResult CreateSuccessResult(
        BlackjackGameState state,
        BlackjackState previousState,
        string messageKey,
        BlackjackRoundResult? roundResult = null) =>
        new()
        {
            Success = true,
            PreviousState = previousState,
            CurrentState = state.State,
            Result = state.Result,
            MessageKey = messageKey,
            RoundResult = roundResult
        };
}
