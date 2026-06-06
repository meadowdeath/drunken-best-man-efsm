using DrunkenBestManEFSM.Domain.Enums.Blackjack;
using DrunkenBestManEFSM.Domain.Rules.Blackjack;
using DrunkenBestManEFSM.Domain.Transitions.Blackjack;
using DrunkenBestManEFSM.Tests.Domain.TestHelpers.Blackjack;
using static DrunkenBestManEFSM.Tests.Domain.TestHelpers.Blackjack.BlackjackCardFactory;

namespace DrunkenBestManEFSM.Tests.Domain.Blackjack;

public sealed class BlackjackTransitionResolverTests
{
    [Fact]
    public void Resolve_ShouldMoveToPlayerTurn_WhenPlaceBetIsValidAndNoNaturalBlackjack()
    {
        var state = BlackjackStateFactory.CreateWaitingForBetStateWithDeck(
            Ten(),
            Nine(),
            Six(),
            Seven());

        var result = PlaceBet(state);

        Assert.True(result.Success);
        Assert.Equal(BlackjackState.PlayerTurn, state.State);
        Assert.Equal(10, state.BetAmount);
        Assert.Equal(2, state.PlayerHand.Cards.Count);
        Assert.Equal(2, state.DealerHand.Cards.Count);
        Assert.Null(result.RoundResult);
    }

    [Fact]
    public void Resolve_ShouldFail_WhenPlaceBetIsInvalid()
    {
        var state = BlackjackStateFactory.CreateWaitingForBetStateWithDeck();

        var result = PlaceBet(state, betAmount: 5);

        Assert.False(result.Success);
        Assert.Equal(BlackjackState.WaitingForBet, state.State);
        Assert.Equal("Blackjack.PlaceBet.InvalidBet", result.MessageKey);
    }

    [Fact]
    public void Resolve_ShouldFinishRound_WhenInitialDealCreatesPlayerBlackjack()
    {
        var state = BlackjackStateFactory.CreateWaitingForBetStateWithDeck(
            Ace(),
            Nine(),
            King(),
            Seven());

        var result = PlaceBet(state);

        AssertFinishedRound(result, BlackjackResult.PlayerBlackjack, expectedMoneyChange: 10);
        Assert.Equal(BlackjackState.Finished, state.State);
    }

    [Fact]
    public void Resolve_ShouldFinishRound_WhenInitialDealCreatesDealerBlackjack()
    {
        var state = BlackjackStateFactory.CreateWaitingForBetStateWithDeck(
            Nine(),
            Ace(),
            Seven(),
            King());

        var result = PlaceBet(state);

        AssertFinishedRound(result, BlackjackResult.DealerBlackjack, expectedMoneyChange: -10);
        Assert.Equal(BlackjackState.Finished, state.State);
    }

    [Fact]
    public void Resolve_ShouldFinishWithExited_WhenLeaveFromWaitingForBet()
    {
        var state = BlackjackStateFactory.CreateWaitingForBetStateWithDeck();

        var result = BlackjackTransitionResolver.Resolve(
            state,
            new BlackjackTransitionRequest { Action = BlackjackAction.Leave },
            availableMoney: 50);

        Assert.True(result.Success);
        Assert.Equal(BlackjackState.Finished, state.State);
        Assert.NotNull(result.RoundResult);
        Assert.Equal(BlackjackResult.Exited, result.RoundResult.Result);
        Assert.Equal(0, result.RoundResult.MoneyChange);
        Assert.Equal(BlackjackRulesConfiguration.BlackjackExitTimeCost, result.RoundResult.TimeCost);
    }

    [Fact]
    public void Resolve_ShouldFail_WhenLeaveAfterCardsAreDealt()
    {
        var state = BlackjackStateFactory.CreatePlayerTurnState();

        var result = BlackjackTransitionResolver.Resolve(
            state,
            new BlackjackTransitionRequest { Action = BlackjackAction.Leave },
            availableMoney: 50);

        Assert.False(result.Success);
        Assert.Equal(BlackjackState.PlayerTurn, state.State);
        Assert.Equal("Blackjack.InvalidState", result.MessageKey);
    }

    [Fact]
    public void Resolve_ShouldAddCardToPlayerHand_WhenHitDuringPlayerTurn()
    {
        var state = BlackjackStateFactory.CreatePlayerTurnState(drawOrder: [Four()]);

        var result = Hit(state);

        Assert.True(result.Success);
        Assert.Equal(3, state.PlayerHand.Cards.Count);
        Assert.Equal(CardRank.Four, state.PlayerHand.Cards[^1].Rank);
    }

    [Fact]
    public void Resolve_ShouldRemainInPlayerTurn_WhenPlayerDoesNotBustOrReachTwentyOne()
    {
        var state = BlackjackStateFactory.CreatePlayerTurnState(
            drawOrder: [Three()],
            playerHand: Hand(Ten(), Six()));

        var result = Hit(state);

        Assert.True(result.Success);
        Assert.Equal(BlackjackState.PlayerTurn, state.State);
        Assert.Equal("Blackjack.Hit.Success", result.MessageKey);
        Assert.Null(result.RoundResult);
    }

    [Fact]
    public void Resolve_ShouldFinishWithDealerWin_WhenPlayerBusts()
    {
        var state = BlackjackStateFactory.CreatePlayerTurnState(
            drawOrder: [Six()],
            playerHand: Hand(Ten(), Nine()));

        var result = Hit(state);

        AssertFinishedRound(result, BlackjackResult.DealerWin, expectedMoneyChange: -10);
        Assert.Equal(BlackjackState.Finished, state.State);
    }

    [Fact]
    public void Resolve_ShouldMoveToDealerTurn_WhenPlayerReachesTwentyOne()
    {
        var state = BlackjackStateFactory.CreatePlayerTurnState(
            drawOrder: [Five()],
            playerHand: Hand(Ten(), Six()));

        var result = Hit(state);

        Assert.True(result.Success);
        Assert.Equal(BlackjackState.DealerTurn, state.State);
        Assert.Equal("Blackjack.Hit.ReachedTwentyOne", result.MessageKey);
        Assert.Null(result.RoundResult);
    }

    [Fact]
    public void Resolve_ShouldMoveToDealerTurn_WhenPlayerStands()
    {
        var state = BlackjackStateFactory.CreatePlayerTurnState();

        var result = BlackjackTransitionResolver.Resolve(
            state,
            new BlackjackTransitionRequest { Action = BlackjackAction.Stand },
            availableMoney: 0);

        Assert.True(result.Success);
        Assert.Equal(BlackjackState.DealerTurn, state.State);
        Assert.Equal("Blackjack.Stand.Success", result.MessageKey);
    }

    [Fact]
    public void AdvanceDealerTurn_ShouldDrawCard_WhenDealerValueIsBelowSeventeen()
    {
        var state = BlackjackStateFactory.CreateDealerTurnState(
            drawOrder: [Two()],
            dealerHand: Hand(Ten(), Six()));

        var result = BlackjackTransitionResolver.AdvanceDealerTurn(state);

        Assert.True(result.Success);
        Assert.Equal(BlackjackState.DealerTurn, state.State);
        Assert.Equal(3, state.DealerHand.Cards.Count);
        Assert.Equal("Blackjack.Dealer.Draw", result.MessageKey);
    }

    [Fact]
    public void AdvanceDealerTurn_ShouldFinishWithPlayerWin_WhenDealerBusts()
    {
        var state = BlackjackStateFactory.CreateDealerTurnState(
            drawOrder: [Ten()],
            playerHand: Hand(Ten(), Nine()),
            dealerHand: Hand(Ten(), Six()));

        var result = BlackjackTransitionResolver.AdvanceDealerTurn(state);

        AssertFinishedRound(result, BlackjackResult.PlayerWin, expectedMoneyChange: 10);
        Assert.Equal(BlackjackState.Finished, state.State);
    }

    [Fact]
    public void AdvanceDealerTurn_ShouldResolveRound_WhenDealerStands()
    {
        var state = BlackjackStateFactory.CreateDealerTurnState(
            playerHand: Hand(Ten(), Eight()),
            dealerHand: Hand(Ten(), Seven()));

        var result = BlackjackTransitionResolver.AdvanceDealerTurn(state);

        AssertFinishedRound(result, BlackjackResult.PlayerWin, expectedMoneyChange: 10);
        Assert.Equal(BlackjackState.Finished, state.State);
    }

    [Fact]
    public void Resolve_ShouldFail_WhenHitBeforeBet()
    {
        var state = BlackjackStateFactory.CreateWaitingForBetStateWithDeck();

        var result = Hit(state);

        Assert.False(result.Success);
        Assert.Equal(BlackjackState.WaitingForBet, state.State);
        Assert.Equal("Blackjack.InvalidState", result.MessageKey);
    }

    [Fact]
    public void Resolve_ShouldFail_WhenStandBeforePlayerTurn()
    {
        var state = BlackjackStateFactory.CreateWaitingForBetStateWithDeck();

        var result = BlackjackTransitionResolver.Resolve(
            state,
            new BlackjackTransitionRequest { Action = BlackjackAction.Stand },
            availableMoney: 0);

        Assert.False(result.Success);
        Assert.Equal(BlackjackState.WaitingForBet, state.State);
        Assert.Equal("Blackjack.InvalidState", result.MessageKey);
    }

    [Fact]
    public void AdvanceDealerTurn_ShouldFail_WhenStateIsNotDealerTurn()
    {
        var state = BlackjackStateFactory.CreatePlayerTurnState();

        var result = BlackjackTransitionResolver.AdvanceDealerTurn(state);

        Assert.False(result.Success);
        Assert.Equal(BlackjackState.PlayerTurn, state.State);
        Assert.Equal("Blackjack.InvalidState", result.MessageKey);
    }

    [Theory]
    [InlineData(BlackjackResult.PlayerWin, 10)]
    [InlineData(BlackjackResult.PlayerBlackjack, 10)]
    [InlineData(BlackjackResult.DealerWin, -10)]
    [InlineData(BlackjackResult.DealerBlackjack, -10)]
    [InlineData(BlackjackResult.Draw, 0)]
    public void ResolveCurrentRound_ShouldCreateRoundResultWithExpectedMoneyChange_WhenRoundFinishes(
        BlackjackResult expectedResult,
        int expectedMoneyChange)
    {
        var state = CreateResolvingStateFor(expectedResult);

        var result = BlackjackTransitionResolver.ResolveCurrentRound(state);

        AssertFinishedRound(result, expectedResult, expectedMoneyChange);
        Assert.Equal(BlackjackRulesConfiguration.BlackjackRoundTimeCost, result.RoundResult!.TimeCost);
    }

    private static BlackjackActionResult PlaceBet(
        DrunkenBestManEFSM.Domain.Models.Blackjack.BlackjackGameState state,
        int betAmount = 10,
        int availableMoney = 50) =>
        BlackjackTransitionResolver.Resolve(
            state,
            new BlackjackTransitionRequest
            {
                Action = BlackjackAction.PlaceBet,
                BetAmount = betAmount
            },
            availableMoney);

    private static BlackjackActionResult Hit(DrunkenBestManEFSM.Domain.Models.Blackjack.BlackjackGameState state) =>
        BlackjackTransitionResolver.Resolve(
            state,
            new BlackjackTransitionRequest { Action = BlackjackAction.Hit },
            availableMoney: 0);

    private static void AssertFinishedRound(
        BlackjackActionResult result,
        BlackjackResult expectedResult,
        int expectedMoneyChange)
    {
        Assert.True(result.Success);
        Assert.Equal(BlackjackState.Finished, result.CurrentState);
        Assert.Equal(expectedResult, result.Result);
        Assert.NotNull(result.RoundResult);
        Assert.Equal(expectedResult, result.RoundResult.Result);
        Assert.Equal(10, result.RoundResult.BetAmount);
        Assert.Equal(expectedMoneyChange, result.RoundResult.MoneyChange);
    }

    private static DrunkenBestManEFSM.Domain.Models.Blackjack.BlackjackGameState CreateResolvingStateFor(BlackjackResult result) =>
        result switch
        {
            BlackjackResult.PlayerWin => CreateResolvingState(Hand(Ten(), Nine()), Hand(Ten(), Eight())),
            BlackjackResult.PlayerBlackjack => CreateResolvingState(Hand(Ace(), King()), Hand(Ten(), Nine())),
            BlackjackResult.DealerWin => CreateResolvingState(Hand(Ten(), Eight()), Hand(Ten(), Nine())),
            BlackjackResult.DealerBlackjack => CreateResolvingState(Hand(Ten(), Nine()), Hand(Ace(), King())),
            BlackjackResult.Draw => CreateResolvingState(Hand(Ten(), Eight()), Hand(Queen(), Eight())),
            _ => throw new ArgumentOutOfRangeException(nameof(result), result, null)
        };

    private static DrunkenBestManEFSM.Domain.Models.Blackjack.BlackjackGameState CreateResolvingState(
        DrunkenBestManEFSM.Domain.Models.Blackjack.BlackjackHand playerHand,
        DrunkenBestManEFSM.Domain.Models.Blackjack.BlackjackHand dealerHand) =>
        new()
        {
            State = BlackjackState.Resolving,
            PlayerHand = playerHand,
            DealerHand = dealerHand,
            Deck = BlackjackDeckFactory.CreateDeckWithDrawOrder(),
            BetAmount = 10,
            Result = BlackjackResult.None
        };
}
