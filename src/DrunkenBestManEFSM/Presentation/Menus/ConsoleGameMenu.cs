using DrunkenBestManEFSM.Application.DTOs;
using DrunkenBestManEFSM.Application.Services;
using DrunkenBestManEFSM.Domain.Enums;
using DrunkenBestManEFSM.Presentation.Console;
using DrunkenBestManEFSM.Presentation.Renderers;

namespace DrunkenBestManEFSM.Presentation.Menus;

public sealed class ConsoleGameMenu
{
    private readonly ConsoleInputReader inputReader;
    private readonly ConsolePrinter printer;
    private readonly GameQueryService queryService;
    private readonly GameActionService actionService;
    private readonly GameStatusRenderer statusRenderer;
    private readonly AvailableActionsRenderer actionsRenderer;
    private readonly AvailableDestinationsRenderer destinationsRenderer;
    private readonly ActionResultRenderer actionResultRenderer;

    public ConsoleGameMenu(
        ConsoleInputReader inputReader,
        ConsolePrinter printer,
        GameQueryService queryService,
        GameActionService actionService,
        GameStatusRenderer statusRenderer,
        AvailableActionsRenderer actionsRenderer,
        AvailableDestinationsRenderer destinationsRenderer,
        ActionResultRenderer actionResultRenderer)
    {
        this.inputReader = inputReader;
        this.printer = printer;
        this.queryService = queryService;
        this.actionService = actionService;
        this.statusRenderer = statusRenderer;
        this.actionsRenderer = actionsRenderer;
        this.destinationsRenderer = destinationsRenderer;
        this.actionResultRenderer = actionResultRenderer;
    }

    public void Run()
    {
        while (true)
        {
            printer.Clear();

            var status = queryService.GetStatus();
            if (status is null)
            {
                printer.WriteError("No active game.");
                inputReader.WaitForEnter();
                return;
            }

            statusRenderer.Render(status);

            if (status.Result is GameResult.Victory or GameResult.Defeat)
            {
                inputReader.WaitForEnter();
                return;
            }

            var actions = queryService.GetAvailableActions();
            actionsRenderer.Render(actions);
            var action = ReadAction(actions);

            if (!action.IsAvailable)
            {
                printer.WriteError("That action is not available.");
                inputReader.WaitForEnter();
                continue;
            }

            var result = ExecuteAction(action);
            actionResultRenderer.Render(result);
            inputReader.WaitForEnter();
        }
    }

    private AvailableActionDto ReadAction(IReadOnlyList<AvailableActionDto> actions)
    {
        var choice = inputReader.ReadIntInRange("Choose an action: ", 1, actions.Count);
        return actions[choice - 1];
    }

    private Application.Results.UseCaseResult ExecuteAction(AvailableActionDto action) =>
        action.ActionType switch
        {
            ActionType.Travel => ExecuteTravel(),
            ActionType.BuyElectrolytes => actionService.BuyElectrolytes(),
            ActionType.BuyFuel => actionService.BuyFuel(),
            ActionType.BuyAlcohol => actionService.BuyAlcohol(),
            ActionType.PickUpRings => actionService.PickUpRings(),
            ActionType.EnterChurch => actionService.EnterChurch(),
            ActionType.CheckStats => actionService.CheckStats(),
            _ => actionService.CheckStats()
        };

    private Application.Results.UseCaseResult ExecuteTravel()
    {
        var destinations = queryService.GetAvailableDestinations();
        destinationsRenderer.Render(destinations);

        var destinationChoice = inputReader.ReadIntInRange("Choose a destination: ", 1, destinations.Count);
        var destination = destinations[destinationChoice - 1];

        printer.WriteSection("Travel Mode");
        printer.WriteLine($"1. Walk ({FormatAvailability(destination.CanWalk)})");
        printer.WriteLine($"2. Drive ({FormatAvailability(destination.CanDrive)})");

        var modeChoice = inputReader.ReadIntInRange("Choose a travel mode: ", 1, 2);
        var travelMode = modeChoice == 1 ? TravelMode.Walk : TravelMode.Drive;

        return actionService.TravelTo(destination.Destination, travelMode);
    }

    private static string FormatAvailability(bool isAvailable) =>
        isAvailable ? "available" : "unavailable";
}
