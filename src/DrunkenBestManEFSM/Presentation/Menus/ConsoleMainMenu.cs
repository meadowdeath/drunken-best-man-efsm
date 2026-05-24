using DrunkenBestManEFSM.Application.Contracts;
using DrunkenBestManEFSM.Application.Services;
using DrunkenBestManEFSM.Presentation.Console;
using DrunkenBestManEFSM.Presentation.Renderers;

namespace DrunkenBestManEFSM.Presentation.Menus;

public sealed class ConsoleMainMenu
{
    private readonly ConsoleInputReader inputReader;
    private readonly ConsolePrinter printer;
    private readonly ITextProvider textProvider;
    private readonly GameSessionService sessionService;
    private readonly ConsoleGameMenu gameMenu;
    private readonly ActionResultRenderer actionResultRenderer;

    public ConsoleMainMenu(
        ConsoleInputReader inputReader,
        ConsolePrinter printer,
        ITextProvider textProvider,
        GameSessionService sessionService,
        ConsoleGameMenu gameMenu,
        ActionResultRenderer actionResultRenderer)
    {
        this.inputReader = inputReader;
        this.printer = printer;
        this.textProvider = textProvider;
        this.sessionService = sessionService;
        this.gameMenu = gameMenu;
        this.actionResultRenderer = actionResultRenderer;
    }

    public void Run()
    {
        while (true)
        {
            printer.Clear();
            printer.WriteTitle(textProvider.GetText("Game.Title"));
            printer.WriteLine("1. Start new game");
            printer.WriteLine("2. Exit");
            printer.WriteEmptyLine();

            var choice = inputReader.ReadIntInRange("Choose an option: ", 1, 2);
            if (choice == 2)
            {
                return;
            }

            var result = sessionService.StartNewGame();
            actionResultRenderer.Render(result);
            inputReader.WaitForEnter();
            gameMenu.Run();
        }
    }
}
