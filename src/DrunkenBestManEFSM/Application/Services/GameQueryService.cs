using DrunkenBestManEFSM.Application.DTOs;
using DrunkenBestManEFSM.Domain.Enums;
using DrunkenBestManEFSM.Domain.Maps;
using DrunkenBestManEFSM.Domain.Models;
using DrunkenBestManEFSM.Domain.Rules;

namespace DrunkenBestManEFSM.Application.Services;

/// <summary>
/// Exposes read-only game information for presentation.
/// </summary>
public sealed class GameQueryService
{
    private readonly GameSessionService sessionService;

    public GameQueryService(GameSessionService sessionService)
    {
        this.sessionService = sessionService;
    }

    public GameStatusDto? GetStatus()
    {
        var state = sessionService.GetCurrentState();
        return state is null ? null : ToStatusDto(state);
    }

    public IReadOnlyList<AvailableActionDto> GetAvailableActions()
    {
        var state = sessionService.GetCurrentState();
        if (state is null)
        {
            return [];
        }

        var hasDestinations = TravelMap.GetDestinationsFrom(state.CurrentLocation).Count > 0;

        return
        [
            CreateAction(ActionType.CheckStats, isAvailable: true, "Actions.CheckStats.Label"),
            CreateAction(ActionType.Travel, hasDestinations, "Actions.Travel.Label", hasDestinations ? null : "Rules.Travel.NoDestinations"),
            CreateAction(ActionType.BuyElectrolytes, IsAtGasStation(state) && ShopRules.CanBuyElectrolytes(state), "Actions.Shop.BuyElectrolytes.Label", GetElectrolytesUnavailableReason(state)),
            CreateAction(ActionType.BuyFuel, IsAtGasStation(state) && ShopRules.CanBuyFuel(state), "Actions.Shop.BuyFuel.Label", GetFuelUnavailableReason(state)),
            CreateAction(ActionType.BuyAlcohol, state.CurrentLocation == Location.Bar && ShopRules.CanBuyAlcohol(state), "Actions.Shop.BuyAlcohol.Label", GetAlcoholUnavailableReason(state)),
            CreateAction(ActionType.PickUpRings, RingRules.CanPickUpRings(state), "Actions.Rings.PickUp.Label", GetRingsUnavailableReason(state)),
            CreateAction(ActionType.EnterChurch, ChurchRules.CanEnterChurch(state), "Actions.Church.Enter.Label", ChurchRules.CanEnterChurch(state) ? null : "Rules.Church.NotAtChurch")
        ];
    }

    public IReadOnlyList<AvailableDestinationDto> GetAvailableDestinations()
    {
        var state = sessionService.GetCurrentState();
        if (state is null)
        {
            return [];
        }

        return TravelMap.GetDestinationsFrom(state.CurrentLocation)
            .Select(destination => CreateDestination(state, destination))
            .ToList();
    }

    public GameStatusDto ToStatusDto(GameState state)
    {
        var stats = state.CharacterStats;

        return new GameStatusDto
        {
            CurrentLocation = state.CurrentLocation,
            CarLocation = state.CarLocation,
            Health = stats.Health,
            Hangover = stats.Hangover,
            Drunkenness = stats.Drunkenness,
            Fuel = stats.Fuel,
            RemainingTime = stats.RemainingTime,
            Money = stats.Money,
            HasRings = state.HasRings,
            CorrectChurchKnown = state.CorrectChurchKnown,
            CorrectChurchToDisplay = state.CorrectChurchKnown ? state.CorrectChurch : null,
            Result = state.Result
        };
    }

    private static AvailableActionDto CreateAction(
        ActionType actionType,
        bool isAvailable,
        string labelKey,
        string? unavailableReasonKey = null) =>
        new()
        {
            ActionType = actionType,
            IsAvailable = isAvailable,
            LabelKey = labelKey,
            UnavailableReasonKey = isAvailable ? null : unavailableReasonKey
        };

    private static AvailableDestinationDto CreateDestination(GameState state, Location destination)
    {
        var walkRoute = TravelMap.GetRouteCost(state.CurrentLocation, destination, TravelMode.Walk);
        var driveRoute = TravelMap.GetRouteCost(state.CurrentLocation, destination, TravelMode.Drive);
        var canWalk = walkRoute is not null && TravelRules.CanTravel(state, TravelMode.Walk, walkRoute.TimeCost, walkRoute.FuelCost);
        var canDrive = driveRoute is not null && TravelRules.CanTravel(state, TravelMode.Drive, driveRoute.TimeCost, driveRoute.FuelCost);

        return new AvailableDestinationDto
        {
            Destination = destination,
            CanWalk = canWalk,
            WalkTimeCost = walkRoute?.TimeCost ?? 0,
            WalkUnavailableReasonKey = canWalk ? null : GetWalkUnavailableReason(state, walkRoute),
            CanDrive = canDrive,
            DriveTimeCost = driveRoute?.TimeCost ?? 0,
            DriveFuelCost = driveRoute?.FuelCost ?? 0,
            DriveUnavailableReasonKey = canDrive ? null : GetDriveUnavailableReason(state, driveRoute)
        };
    }

    private static bool IsAtGasStation(GameState state) =>
        state.CurrentLocation == Location.GasStation;

    private static string? GetElectrolytesUnavailableReason(GameState state)
    {
        if (!IsAtGasStation(state))
        {
            return "Rules.Shop.Electrolytes.WrongLocation";
        }

        return state.CharacterStats.Money >= GameEconomy.ElectrolyteCost ? null : "Rules.Shop.NotEnoughMoney";
    }

    private static string? GetFuelUnavailableReason(GameState state)
    {
        if (!IsAtGasStation(state))
        {
            return "Rules.Shop.Fuel.WrongLocation";
        }

        if (state.CharacterStats.Money < GameEconomy.FuelCost)
        {
            return "Rules.Shop.NotEnoughMoney";
        }

        return state.CharacterStats.Fuel < GameLimits.MaxFuel ? null : "Rules.Shop.Fuel.Full";
    }

    private static string? GetAlcoholUnavailableReason(GameState state)
    {
        if (state.CurrentLocation != Location.Bar)
        {
            return "Rules.Shop.Alcohol.WrongLocation";
        }

        if (state.CharacterStats.Money < GameEconomy.AlcoholCost)
        {
            return "Rules.Shop.NotEnoughMoney";
        }

        if (state.CharacterStats.Health <= GameEconomy.AlcoholHealthCost)
        {
            return "Rules.Shop.Alcohol.HealthTooLow";
        }

        if (state.CharacterStats.RemainingTime >= GameLimits.MaxRemainingTime)
        {
            return "Rules.Shop.Alcohol.TimeLimitReached";
        }

        return state.CharacterStats.Drunkenness < GameLimits.MaxDrunkenness ? null : "Rules.Shop.Alcohol.NotAllowed";
    }

    private static string? GetRingsUnavailableReason(GameState state)
    {
        if (state.CurrentLocation != Location.JewelryStore)
        {
            return "Rules.Rings.WrongLocation";
        }

        if (state.HasRings)
        {
            return "Rules.Rings.AlreadyPickedUp";
        }

        return TravelRules.HasEnoughTime(state.CharacterStats, GameEconomy.PickUpRingsTimeCost)
            ? null
            : "Rules.Rings.NotEnoughTime";
    }

    private static string? GetWalkUnavailableReason(GameState state, RouteCost? route)
    {
        if (route is null)
        {
            return "Rules.Travel.RouteNotFound";
        }

        return TravelRules.HasEnoughTime(state.CharacterStats, route.TimeCost)
            ? null
            : "Rules.Travel.NotEnoughTime";
    }

    private static string? GetDriveUnavailableReason(GameState state, RouteCost? route)
    {
        if (route is null)
        {
            return "Rules.Travel.RouteNotFound";
        }

        if (!TravelRules.CanDriveFromCurrentLocation(state))
        {
            return "Rules.Travel.CarNotHere";
        }

        if (!TravelRules.HasEnoughFuel(state.CharacterStats, route.FuelCost))
        {
            return "Rules.Travel.NotEnoughFuel";
        }

        if (!TravelRules.HasEnoughTime(state.CharacterStats, route.TimeCost))
        {
            return "Rules.Travel.NotEnoughTime";
        }

        return state.CharacterStats.Drunkenness < GameLimits.DrivingDrunkennessLimit
            ? null
            : "Rules.Travel.TooDrunkToDrive";
    }
}
