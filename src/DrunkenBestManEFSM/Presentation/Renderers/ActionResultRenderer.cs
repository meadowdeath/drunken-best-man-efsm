using DrunkenBestManEFSM.Application.Contracts;
using DrunkenBestManEFSM.Application.DTOs;
using DrunkenBestManEFSM.Application.Results;
using DrunkenBestManEFSM.Domain.Enums;
using DrunkenBestManEFSM.Presentation.Console;

namespace DrunkenBestManEFSM.Presentation.Renderers;

public sealed class ActionResultRenderer
{
    private readonly ConsolePrinter printer;
    private readonly ITextProvider textProvider;
    private readonly GameStatusRenderer statusRenderer;

    public ActionResultRenderer(ConsolePrinter printer, ITextProvider textProvider, GameStatusRenderer statusRenderer)
    {
        this.printer = printer;
        this.textProvider = textProvider;
        this.statusRenderer = statusRenderer;
    }

    public void Render(UseCaseResult<GameActionResultDto> result)
    {
        printer.WriteSection("Result");

        if (result.Success || result.MessageKey == "Menu.Cancelled")
        {
            printer.WriteLine(textProvider.GetText(result.MessageKey));
        }
        else
        {
            printer.WriteError(textProvider.GetText(result.MessageKey));
        }

        var actionResult = result.Data?.ActionResult;
        if (actionResult is not null)
        {
            printer.WriteLine(textProvider.GetText(actionResult.MessageKey));

            if (actionResult.RandomEvent is { Occurred: true } randomEvent)
            {
                printer.WriteLine(textProvider.GetText($"RandomEvent.{randomEvent.EventType}"));
            }

            RenderOutcomeMessage(actionResult.GameResult);
        }

        if (result.Data?.GameStatus is not null)
        {
            statusRenderer.Render(result.Data.GameStatus);
        }
    }

    public void Render(UseCaseResult<GameStatusDto> result)
    {
        printer.WriteSection("Result");

        if (result.Success)
        {
            printer.WriteLine(textProvider.GetText(result.MessageKey));
        }
        else
        {
            printer.WriteError(textProvider.GetText(result.MessageKey));
        }

        if (result.Data is not null)
        {
            statusRenderer.Render(result.Data);
        }
    }

    private void RenderOutcomeMessage(GameResult result)
    {
        if (result == GameResult.Victory)
        {
            printer.WriteLine(textProvider.GetText("Rules.Game.Victory"));
        }
        else if (result == GameResult.Defeat)
        {
            printer.WriteLine(textProvider.GetText("Location.Defeat"));
        }
    }
}
