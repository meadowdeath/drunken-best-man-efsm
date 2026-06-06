using DrunkenBestManEFSM.Domain.Rules.Blackjack;
using static DrunkenBestManEFSM.Tests.Domain.TestHelpers.Blackjack.BlackjackCardFactory;

namespace DrunkenBestManEFSM.Tests.Domain.Blackjack;

public sealed class BlackjackDealerRulesTests
{
    [Fact]
    public void ShouldHit_ShouldReturnTrue_WhenDealerValueIsBelowSeventeen()
    {
        var hand = Hand(Ten(), Six());

        var result = BlackjackDealerRules.ShouldHit(hand);

        Assert.True(result);
    }

    [Fact]
    public void ShouldHit_ShouldReturnFalse_WhenDealerValueIsSeventeenOrMore()
    {
        var hand = Hand(Ten(), Seven());

        var result = BlackjackDealerRules.ShouldHit(hand);

        Assert.False(result);
    }

    [Fact]
    public void ShouldStand_ShouldReturnTrue_WhenDealerValueIsSeventeenOrMore()
    {
        var hand = Hand(Ten(), Seven());

        var result = BlackjackDealerRules.ShouldStand(hand);

        Assert.True(result);
    }

    [Fact]
    public void ShouldStand_ShouldReturnFalse_WhenDealerValueIsBelowSeventeen()
    {
        var hand = Hand(Ten(), Six());

        var result = BlackjackDealerRules.ShouldStand(hand);

        Assert.False(result);
    }

    [Fact]
    public void ShouldStand_ShouldReturnTrue_WhenDealerHasSoftSeventeen()
    {
        var hand = Hand(Ace(), Six());

        var result = BlackjackDealerRules.ShouldStand(hand);

        Assert.True(result);
    }
}
