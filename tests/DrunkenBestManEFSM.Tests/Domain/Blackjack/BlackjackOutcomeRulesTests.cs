using DrunkenBestManEFSM.Domain.Enums.Blackjack;
using DrunkenBestManEFSM.Domain.Rules.Blackjack;
using static DrunkenBestManEFSM.Tests.Domain.TestHelpers.Blackjack.BlackjackCardFactory;

namespace DrunkenBestManEFSM.Tests.Domain.Blackjack;

public sealed class BlackjackOutcomeRulesTests
{
    [Fact]
    public void ResolveOutcome_ShouldReturnPlayerBlackjack_WhenOnlyPlayerHasNaturalBlackjack()
    {
        var playerHand = Hand(Ace(), King());
        var dealerHand = Hand(Ten(), Nine());

        var result = BlackjackOutcomeRules.ResolveOutcome(playerHand, dealerHand);

        Assert.Equal(BlackjackResult.PlayerBlackjack, result);
    }

    [Fact]
    public void ResolveOutcome_ShouldReturnDealerBlackjack_WhenOnlyDealerHasNaturalBlackjack()
    {
        var playerHand = Hand(Ten(), Nine());
        var dealerHand = Hand(Ace(), King());

        var result = BlackjackOutcomeRules.ResolveOutcome(playerHand, dealerHand);

        Assert.Equal(BlackjackResult.DealerBlackjack, result);
    }

    [Fact]
    public void ResolveOutcome_ShouldReturnDraw_WhenBothHaveNaturalBlackjack()
    {
        var playerHand = Hand(Ace(), King());
        var dealerHand = Hand(Ace(CardSuit.Hearts), Queen());

        var result = BlackjackOutcomeRules.ResolveOutcome(playerHand, dealerHand);

        Assert.Equal(BlackjackResult.Draw, result);
    }

    [Fact]
    public void ResolveOutcome_ShouldReturnDealerWin_WhenPlayerBusts()
    {
        var playerHand = Hand(Ten(), Nine(), Five());
        var dealerHand = Hand(Ten(), Eight());

        var result = BlackjackOutcomeRules.ResolveOutcome(playerHand, dealerHand);

        Assert.Equal(BlackjackResult.DealerWin, result);
    }

    [Fact]
    public void ResolveOutcome_ShouldReturnPlayerWin_WhenDealerBusts()
    {
        var playerHand = Hand(Ten(), Nine());
        var dealerHand = Hand(Ten(), Nine(), Five());

        var result = BlackjackOutcomeRules.ResolveOutcome(playerHand, dealerHand);

        Assert.Equal(BlackjackResult.PlayerWin, result);
    }

    [Fact]
    public void ResolveOutcome_ShouldReturnPlayerWin_WhenPlayerValueIsHigher()
    {
        var playerHand = Hand(Ten(), Nine());
        var dealerHand = Hand(Ten(), Eight());

        var result = BlackjackOutcomeRules.ResolveOutcome(playerHand, dealerHand);

        Assert.Equal(BlackjackResult.PlayerWin, result);
    }

    [Fact]
    public void ResolveOutcome_ShouldReturnDealerWin_WhenDealerValueIsHigher()
    {
        var playerHand = Hand(Ten(), Seven());
        var dealerHand = Hand(Ten(), Nine());

        var result = BlackjackOutcomeRules.ResolveOutcome(playerHand, dealerHand);

        Assert.Equal(BlackjackResult.DealerWin, result);
    }

    [Fact]
    public void ResolveOutcome_ShouldReturnDraw_WhenValuesAreEqual()
    {
        var playerHand = Hand(Ten(), Eight());
        var dealerHand = Hand(Queen(), Eight());

        var result = BlackjackOutcomeRules.ResolveOutcome(playerHand, dealerHand);

        Assert.Equal(BlackjackResult.Draw, result);
    }
}
