using DrunkenBestManEFSM.Application.DTOs;
using DrunkenBestManEFSM.Application.Contracts;
using DrunkenBestManEFSM.Application.Results;
using DrunkenBestManEFSM.Application.Services;
using DrunkenBestManEFSM.Domain.Enums;
using DrunkenBestManEFSM.Presentation.Console;
using DrunkenBestManEFSM.Presentation.Renderers;

namespace DrunkenBestManEFSM.Presentation.Menus;

public sealed class ConsoleGameMenu
{
    private readonly ConsoleInputReader inputReader;
    private readonly ConsolePrinter printer;
    private readonly ITextProvider textProvider;
    private readonly GameQueryService queryService;
    private readonly GameActionService actionService;
    private readonly GameStatusRenderer statusRenderer;
    private readonly AvailableActionsRenderer actionsRenderer;
    private readonly AvailableDestinationsRenderer destinationsRenderer;
    private readonly ActionResultRenderer actionResultRenderer;

    public ConsoleGameMenu(
        ConsoleInputReader inputReader,
        ConsolePrinter printer,
        ITextProvider textProvider,
        GameQueryService queryService,
        GameActionService actionService,
        GameStatusRenderer statusRenderer,
        AvailableActionsRenderer actionsRenderer,
        AvailableDestinationsRenderer destinationsRenderer,
        ActionResultRenderer actionResultRenderer)
    {
        this.inputReader = inputReader;
        this.printer = printer;
        this.textProvider = textProvider;
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

            var statusResult = queryService.GetStatus();
            var status = statusResult.Data;
            if (!statusResult.Success || status is null)
            {
                printer.WriteError(textProvider.GetText("UseCase.Game.NoActiveGame"));
                inputReader.WaitForEnter();
                return;
            }

            statusRenderer.Render(status);

            if (status.Result is GameResult.Victory or GameResult.Defeat)
            {
                inputReader.WaitForEnter();
                return;
            }

            var actions = queryService.GetAvailableActions().Data ?? [];
            actionsRenderer.Render(actions);
            var action = ReadAction(actions);
            if (action is null)
            {
                return;
            }

            if (!action.IsAvailable)
            {
                printer.WriteError(textProvider.GetText("Menu.Game.ActionUnavailable"));
                inputReader.WaitForEnter();
                continue;
            }

            var result = ExecuteAction(action);
            actionResultRenderer.Render(result);
            inputReader.WaitForEnter();
        }
    }

    private AvailableActionDto? ReadAction(IReadOnlyList<AvailableActionDto> actions)
    {
        var choice = inputReader.ReadIntInRange(textProvider.GetText("Menu.Game.ChooseAction"), 0, actions.Count);
        if (choice == 0)
        {
            return null;
        }

        return actions[choice - 1];
    }

    private UseCaseResult<GameActionResultDto> ExecuteAction(AvailableActionDto action) =>
        action.ActionType switch
        {
            ActionType.Travel => ExecuteTravel(),
            ActionType.BuyElectrolytes => ExecutePurchase(ActionType.BuyElectrolytes, actionService.BuyElectrolytes),
            ActionType.BuyFuel => ExecutePurchase(ActionType.BuyFuel, actionService.BuyFuel),
            ActionType.BuyAlcohol => ExecutePurchase(ActionType.BuyAlcohol, actionService.BuyAlcohol),
            ActionType.RestAtStripClub => ExecutePurchase(ActionType.RestAtStripClub, actionService.RestAtStripClub),
            ActionType.PickUpRings => ExecuteConfirmedAction(
                "Actions.Rings.PickUp.Label",
                "Menu.ConfirmRings",
                actionService.PickUpRings),
            ActionType.EnterChurch => ExecuteConfirmedAction(
                "Actions.Church.Enter.Label",
                "Menu.ConfirmChurch",
                actionService.EnterChurch),
            ActionType.CheckStats => actionService.CheckStats(),
            _ => actionService.CheckStats()
        };

    private UseCaseResult<GameActionResultDto> ExecuteTravel()
    {
        var destinations = queryService.GetAvailableDestinations().Data ?? [];
        destinationsRenderer.Render(destinations);

        var destinationChoice = inputReader.ReadIntInRange(textProvider.GetText("Menu.Game.ChooseDestination"), 0, destinations.Count);
        if (destinationChoice == 0)
        {
            return CreateCancelledResult();
        }

        var destination = destinations[destinationChoice - 1];

        printer.WriteSection(textProvider.GetText("Menu.Game.TravelMode"));
        printer.WriteLine($"1. {textProvider.GetText("TravelMode.Walk")} ({FormatAvailability(destination.CanWalk)})");
        printer.WriteLine($"2. {textProvider.GetText("TravelMode.Drive")} ({FormatAvailability(destination.CanDrive)})");
        printer.WriteLine($"0. {textProvider.GetText("Menu.Cancel")}");

        var modeChoice = inputReader.ReadIntInRange(textProvider.GetText("Menu.Game.ChooseTravelMode"), 0, 2);
        if (modeChoice == 0)
        {
            return CreateCancelledResult();
        }

        var travelMode = modeChoice == 1 ? TravelMode.Walk : TravelMode.Drive;
        var timeCost = modeChoice == 1 ? destination.WalkTimeCost : destination.DriveTimeCost;
        var fuelCost = modeChoice == 1 ? 0 : destination.DriveFuelCost;
        var destinationName = textProvider.GetText($"Location.{destination.Destination}");
        var modeName = textProvider.GetText($"TravelMode.{travelMode}");

        printer.WriteSection(textProvider.GetText("Menu.ConfirmTravel"));
        printer.WriteLine($"{modeName} -> {destinationName}");
        printer.WriteLine($"Cost: {timeCost} min, {fuelCost} fuel");

        if (!inputReader.ReadConfirmation(textProvider.GetText("Menu.Confirm")))
        {
            return CreateCancelledResult();
        }

        return actionService.TravelTo(destination.Destination, travelMode);
    }

    private UseCaseResult<GameActionResultDto> ExecutePurchase(
        ActionType actionType,
        Func<UseCaseResult<GameActionResultDto>> execute)
    {
        var summary = queryService.GetShopActionSummary(actionType).Data;
        if (summary is not null)
        {
            printer.WriteSection(textProvider.GetText(summary.TitleKey));
            printer.WriteLine($"Cost: ${summary.Cost}");
            printer.WriteLine($"Effects: {FormatShopEffects(summary)}");

            if (summary.RemainingTimeLimit is not null)
            {
                printer.WriteLine($"Limit: {textProvider.GetText("Shop.RemainingTimeLimit")} {summary.RemainingTimeLimit}");
            }
        }

        if (!inputReader.ReadConfirmation(textProvider.GetText("Menu.ConfirmPurchase")))
        {
            return CreateCancelledResult();
        }

        return execute();
    }

    private UseCaseResult<GameActionResultDto> ExecuteConfirmedAction(
        string titleKey,
        string descriptionKey,
        Func<UseCaseResult<GameActionResultDto>> execute)
    {
        printer.WriteSection(textProvider.GetText(titleKey));
        printer.WriteLine(textProvider.GetText(descriptionKey));

        if (!inputReader.ReadConfirmation(textProvider.GetText("Menu.Confirm")))
        {
            return CreateCancelledResult();
        }

        return execute();
    }

    private UseCaseResult<GameActionResultDto> CreateCancelledResult() =>
        new()
        {
            Success = false,
            MessageKey = "Menu.Cancelled",
            Data = new GameActionResultDto
            {
                GameStatus = queryService.GetStatus().Data
            }
        };

    private static string FormatShopEffects(ShopActionSummaryDto summary)
    {
        var effects = new List<string>();
        AddHealthEffect(effects, summary);
        AddEffect(effects, "Hangover", summary.HangoverChange);
        AddEffect(effects, "Drunkenness", summary.DrunkennessChange);
        AddEffect(effects, "Fuel", summary.FuelChange);
        AddEffect(effects, "min", summary.RemainingTimeChange);
        return string.Join(", ", effects);
    }

    private static void AddHealthEffect(List<string> effects, ShopActionSummaryDto summary)
    {
        if (summary.MinHealthChange is not null && summary.MaxHealthChange is not null)
        {
            effects.Add($"+{summary.MinHealthChange} to +{summary.MaxHealthChange} Health");
            return;
        }

        AddEffect(effects, "Health", summary.HealthChange);
    }

    private static void AddEffect(List<string> effects, string label, int value)
    {
        if (value == 0)
        {
            return;
        }

        effects.Add($"{value:+#;-#} {label}");
    }

    private static string FormatAvailability(bool isAvailable) =>
        isAvailable ? "available" : "unavailable";
}
