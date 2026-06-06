using DrunkenBestManEFSM.Application.Contracts;
using DrunkenBestManEFSM.Application.Services;
using DrunkenBestManEFSM.Application.Services.Blackjack;
using DrunkenBestManEFSM.Domain.Enums.Blackjack;
using DrunkenBestManEFSM.Presentation.Console;
using DrunkenBestManEFSM.Presentation.Renderers;
using DrunkenBestManEFSM.Presentation.Renderers.Blackjack;

namespace DrunkenBestManEFSM.Presentation.Menus.Blackjack;

public sealed class BlackjackMenu
{
    private readonly ConsoleInputReader inputReader;
    private readonly ConsolePrinter printer;
    private readonly ITextProvider textProvider;
    private readonly GameQueryService gameQueryService;
    private readonly GameActionService gameActionService;
    private readonly BlackjackSessionService blackjackSessionService;
    private readonly BlackjackQueryService blackjackQueryService;
    private readonly BlackjackActionService blackjackActionService;
    private readonly BlackjackRenderer blackjackRenderer;
    private readonly ActionResultRenderer actionResultRenderer;

    public BlackjackMenu(
        ConsoleInputReader inputReader,
        ConsolePrinter printer,
        ITextProvider textProvider,
        GameQueryService gameQueryService,
        GameActionService gameActionService,
        BlackjackSessionService blackjackSessionService,
        BlackjackQueryService blackjackQueryService,
        BlackjackActionService blackjackActionService,
        BlackjackRenderer blackjackRenderer,
        ActionResultRenderer actionResultRenderer)
    {
        this.inputReader = inputReader;
        this.printer = printer;
        this.textProvider = textProvider;
        this.gameQueryService = gameQueryService;
        this.gameActionService = gameActionService;
        this.blackjackSessionService = blackjackSessionService;
        this.blackjackQueryService = blackjackQueryService;
        this.blackjackActionService = blackjackActionService;
        this.blackjackRenderer = blackjackRenderer;
        this.actionResultRenderer = actionResultRenderer;
    }

    public void Run()
    {
        printer.Clear();
        printer.WriteTitle(textProvider.GetText("Blackjack.Title"));
        printer.WriteLine(textProvider.GetText("Blackjack.Intro"));
        printer.WriteEmptyLine();

        var gameStatus = gameQueryService.GetStatus().Data;
        if (gameStatus is null)
        {
            printer.WriteError(textProvider.GetText("UseCase.Game.NoActiveGame"));
            inputReader.WaitForEnter();
            return;
        }

        printer.WriteLine($"{textProvider.GetText("Status.Money")} ${gameStatus.Money}");
        printer.WriteLine($"0. {textProvider.GetText("Blackjack.Cancel")}");

        var betAmount = inputReader.ReadIntInRange(textProvider.GetText("Blackjack.PlaceBetPrompt"), 0, gameStatus.Money);
        if (betAmount == 0)
        {
            printer.WriteLine(textProvider.GetText("Blackjack.Cancelled"));
            inputReader.WaitForEnter();
            return;
        }

        var startResult = blackjackSessionService.StartNewRound(betAmount, gameStatus.Money);
        if (!startResult.Success || startResult.Data is null)
        {
            printer.WriteError(textProvider.GetText(startResult.MessageKey));
            inputReader.WaitForEnter();
            return;
        }

        blackjackRenderer.RenderStatus(startResult.Data);

        if (startResult.Data.IsFinished)
        {
            ApplyFinishedRound();
            inputReader.WaitForEnter();
            return;
        }

        RunActionLoop();
    }

    private void RunActionLoop()
    {
        while (true)
        {
            var statusResult = blackjackQueryService.GetStatus().Data;

            if (statusResult is null || statusResult.IsFinished)
            {
                ApplyFinishedRound();
                inputReader.WaitForEnter();
                return;
            }

            printer.Clear();
            blackjackRenderer.RenderStatus(statusResult);

            if (statusResult.State != BlackjackState.PlayerTurn)
            {
                ApplyFinishedRound();
                inputReader.WaitForEnter();
                return;
            }

            printer.WriteSection(textProvider.GetText("Blackjack.Title"));
            printer.WriteLine($"1. {textProvider.GetText("Blackjack.Action.Hit")}");
            printer.WriteLine($"2. {textProvider.GetText("Blackjack.Action.Stand")}");

            var choice = inputReader.ReadIntInRange(textProvider.GetText("Menu.Game.ChooseAction"), 1, 2);
            var actionResult = choice == 1
                ? blackjackActionService.Hit()
                : blackjackActionService.Stand();

            if (actionResult.Data?.ActionResult is not null)
            {
                blackjackRenderer.RenderActionResult(actionResult.Data.ActionResult);
            }

            if (actionResult.Data?.Status is not null)
            {
                blackjackRenderer.RenderStatus(actionResult.Data.Status);
            }

            if (actionResult.Data?.RoundFinished == true)
            {
                ApplyFinishedRound(actionResult.Data.RoundResult);
                inputReader.WaitForEnter();
                return;
            }

            inputReader.WaitForEnter();
        }
    }

    private void ApplyFinishedRound(Domain.Results.Blackjack.BlackjackRoundResult? roundResult = null)
    {
        var resultToApply = roundResult ?? blackjackSessionService.GetCompletedRoundResult();
        if (resultToApply is null)
        {
            return;
        }

        var mainResult = gameActionService.ApplyBlackjackRoundResult(resultToApply);
        actionResultRenderer.Render(mainResult);
        blackjackSessionService.ClearRound();
    }
}
