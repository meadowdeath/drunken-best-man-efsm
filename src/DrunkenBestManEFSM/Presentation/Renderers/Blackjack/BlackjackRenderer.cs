using DrunkenBestManEFSM.Application.Contracts;
using DrunkenBestManEFSM.Application.DTOs.Blackjack;
using DrunkenBestManEFSM.Domain.Enums.Blackjack;
using DrunkenBestManEFSM.Domain.Transitions.Blackjack;
using DrunkenBestManEFSM.Presentation.Console;

namespace DrunkenBestManEFSM.Presentation.Renderers.Blackjack;

public sealed class BlackjackRenderer
{
    private readonly ConsolePrinter printer;
    private readonly ITextProvider textProvider;

    public BlackjackRenderer(ConsolePrinter printer, ITextProvider textProvider)
    {
        this.printer = printer;
        this.textProvider = textProvider;
    }

    public void RenderStatus(BlackjackStatusDto status)
    {
        printer.WriteTitle(textProvider.GetText("Blackjack.Title"));
        printer.WriteLine($"{textProvider.GetText("Blackjack.Label.Bet")} ${status.BetAmount}");
        printer.WriteLine($"{textProvider.GetText("Blackjack.Label.State")} {textProvider.GetText($"Blackjack.State.{status.State}")}");

        printer.WriteSection(textProvider.GetText("Blackjack.Label.Dealer"));
        RenderCards(status.VisibleDealerCards);
        if (status.DealerValue is null)
        {
            printer.WriteLine(textProvider.GetText("Blackjack.Label.HiddenCard"));
        }
        else
        {
            printer.WriteLine($"{textProvider.GetText("Blackjack.Label.Value")} {status.DealerValue}");
        }

        printer.WriteSection(textProvider.GetText("Blackjack.Label.Player"));
        RenderCards(status.PlayerCards);
        printer.WriteLine($"{textProvider.GetText("Blackjack.Label.Value")} {status.PlayerValue}");

        if (status.IsFinished)
        {
            printer.WriteSection(textProvider.GetText("Blackjack.Label.Result"));
            printer.WriteLine(textProvider.GetText($"Blackjack.Result.{status.Result}"));
        }
    }

    public void RenderActionResult(BlackjackActionResult actionResult)
    {
        if (actionResult.Success)
        {
            printer.WriteLine(textProvider.GetText(actionResult.MessageKey));
        }
        else
        {
            printer.WriteError(textProvider.GetText(actionResult.MessageKey));
        }
    }

    private void RenderCards(IReadOnlyList<BlackjackCardDto> cards)
    {
        if (cards.Count == 0)
        {
            printer.WriteLine("-");
            return;
        }

        foreach (var card in cards)
        {
            var cardText = textProvider.GetText(card.DisplayKey);
            printer.WriteLine(IsMissingText(cardText) ? FormatCard(card) : cardText);
        }
    }

    private static bool IsMissingText(string text) =>
        text.StartsWith("[Missing text:", StringComparison.Ordinal);

    private static string FormatCard(BlackjackCardDto card) =>
        $"{card.Rank} of {card.Suit}";
}
