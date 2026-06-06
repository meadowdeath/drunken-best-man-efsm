using DrunkenBestManEFSM.Domain.Enums.Blackjack;
using DrunkenBestManEFSM.Domain.Rules.Blackjack;
using static DrunkenBestManEFSM.Tests.Domain.TestHelpers.Blackjack.BlackjackCardFactory;

namespace DrunkenBestManEFSM.Tests.Domain.Blackjack;

public sealed class BlackjackHandRulesTests
{
    [Fact]
    public void CalculateValue_ShouldReturnNumericValue_ForNumberCards()
    {
        var hand = Hand(Two(), Nine(), Five());

        var value = BlackjackHandRules.CalculateValue(hand);

        Assert.Equal(16, value);
    }

    [Theory]
    [InlineData(CardRank.Jack)]
    [InlineData(CardRank.Queen)]
    [InlineData(CardRank.King)]
    public void CalculateValue_ShouldReturnTen_ForFaceCards(CardRank rank)
    {
        var hand = Hand(Card(rank));

        var value = BlackjackHandRules.CalculateValue(hand);

        Assert.Equal(10, value);
    }

    [Fact]
    public void CalculateValue_ShouldTreatAceAsEleven_WhenItDoesNotBust()
    {
        var hand = Hand(Ace(), King());

        var value = BlackjackHandRules.CalculateValue(hand);

        Assert.Equal(21, value);
    }

    [Fact]
    public void CalculateValue_ShouldTreatAceAsOne_WhenElevenWouldBust()
    {
        var hand = Hand(Ace(), Nine(), Five());

        var value = BlackjackHandRules.CalculateValue(hand);

        Assert.Equal(15, value);
    }

    [Theory]
    [MemberData(nameof(MultipleAceHands))]
    public void CalculateValue_ShouldHandleMultipleAcesCorrectly(DrunkenBestManEFSM.Domain.Models.Blackjack.BlackjackHand hand, int expectedValue)
    {
        var value = BlackjackHandRules.CalculateValue(hand);

        Assert.Equal(expectedValue, value);
    }

    [Fact]
    public void IsBust_ShouldReturnTrue_WhenHandValueExceedsTwentyOne()
    {
        var hand = Hand(Ten(), Nine(), Five());

        var isBust = BlackjackHandRules.IsBust(hand);

        Assert.True(isBust);
    }

    [Theory]
    [MemberData(nameof(NonBustHands))]
    public void IsBust_ShouldReturnFalse_WhenHandValueIsTwentyOneOrLess(DrunkenBestManEFSM.Domain.Models.Blackjack.BlackjackHand hand)
    {
        var isBust = BlackjackHandRules.IsBust(hand);

        Assert.False(isBust);
    }

    [Fact]
    public void IsNaturalBlackjack_ShouldReturnTrue_WhenTwoCardsEqualTwentyOne()
    {
        var hand = Hand(Ace(), King());

        var isNatural = BlackjackHandRules.IsNaturalBlackjack(hand);

        Assert.True(isNatural);
    }

    [Fact]
    public void IsNaturalBlackjack_ShouldReturnFalse_WhenThreeCardsEqualTwentyOne()
    {
        var hand = Hand(Ace(), Five(), Five());

        var isNatural = BlackjackHandRules.IsNaturalBlackjack(hand);

        Assert.False(isNatural);
    }

    [Fact]
    public void IsNaturalBlackjack_ShouldReturnFalse_WhenTwoCardsDoNotEqualTwentyOne()
    {
        var hand = Hand(Ten(), Nine());

        var isNatural = BlackjackHandRules.IsNaturalBlackjack(hand);

        Assert.False(isNatural);
    }

    [Fact]
    public void HasTwentyOne_ShouldReturnTrue_WhenCalculatedValueIsTwentyOne()
    {
        var hand = Hand(Ace(), Ace(), Nine());

        var hasTwentyOne = BlackjackHandRules.HasTwentyOne(hand);

        Assert.True(hasTwentyOne);
    }

    [Fact]
    public void HasTwentyOne_ShouldReturnFalse_WhenCalculatedValueIsNotTwentyOne()
    {
        var hand = Hand(Ten(), Eight());

        var hasTwentyOne = BlackjackHandRules.HasTwentyOne(hand);

        Assert.False(hasTwentyOne);
    }

    public static TheoryData<DrunkenBestManEFSM.Domain.Models.Blackjack.BlackjackHand, int> MultipleAceHands() =>
        new()
        {
            { Hand(Ace(), Ace(), Nine()), 21 },
            { Hand(Ace(), Ace(), Nine(), King()), 21 }
        };

    public static TheoryData<DrunkenBestManEFSM.Domain.Models.Blackjack.BlackjackHand> NonBustHands() =>
        new()
        {
            Hand(Ace(), King()),
            Hand(Ten(), Nine()),
            Hand(Ace(), Ace(), Nine(), King())
        };
}
