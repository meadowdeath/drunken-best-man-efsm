using DrunkenBestManEFSM.Application.DTOs;
using DrunkenBestManEFSM.Application.Results;
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

    public UseCaseResult<GameStatusDto> GetStatus()
    {
        var state = sessionService.GetCurrentState();
        return state is null
            ? UseCaseResult<GameStatusDto>.Fail("UseCase.Game.NoActiveGame")
            : UseCaseResult<GameStatusDto>.Ok(ToStatusDto(state), "UseCase.Action.Completed");
    }

    public UseCaseResult<IReadOnlyList<AvailableActionDto>> GetAvailableActions()
    {
        var state = sessionService.GetCurrentState();
        if (state is null)
        {
            return UseCaseResult<IReadOnlyList<AvailableActionDto>>.Fail("UseCase.Game.NoActiveGame");
        }

        var hasDestinations = TravelMap.GetDestinationsFrom(state.CurrentLocation).Count > 0;

        IReadOnlyList<AvailableActionDto> actions =
        [
            CreateAction(ActionType.CheckStats, isAvailable: true, "Actions.CheckStats.Label"),
            CreateAction(ActionType.Travel, hasDestinations, "Actions.Travel.Label", hasDestinations ? null : "Rules.Travel.NoDestinations"),
            CreateAction(ActionType.BuyElectrolytes, IsAtGasStation(state) && ShopRules.CanBuyElectrolytes(state), "Actions.Shop.BuyElectrolytes.Label", GetElectrolytesUnavailableReason(state)),
            CreateAction(ActionType.BuyFuel, IsAtGasStation(state) && ShopRules.CanBuyFuel(state), "Actions.Shop.BuyFuel.Label", GetFuelUnavailableReason(state)),
            CreateAction(ActionType.BuyAlcohol, state.CurrentLocation == Location.Bar && ShopRules.CanBuyAlcohol(state), "Actions.Shop.BuyAlcohol.Label", GetAlcoholUnavailableReason(state)),
            CreateAction(ActionType.RestAtStripClub, state.CurrentLocation == Location.StripClub && ShopRules.CanRestAtStripClub(state), "Actions.StripClub.Rest.Label", GetStripClubRestUnavailableReason(state)),
            CreateAction(ActionType.PickUpRings, RingRules.CanPickUpRings(state), "Actions.Rings.PickUp.Label", GetRingsUnavailableReason(state)),
            CreateAction(ActionType.EnterChurch, ChurchRules.CanEnterChurch(state), "Actions.Church.Enter.Label", ChurchRules.CanEnterChurch(state) ? null : "Rules.Church.NotAtChurch")
        ];

        return UseCaseResult<IReadOnlyList<AvailableActionDto>>.Ok(actions, "UseCase.Action.Completed");
    }

    public UseCaseResult<IReadOnlyList<AvailableDestinationDto>> GetAvailableDestinations()
    {
        var state = sessionService.GetCurrentState();
        if (state is null)
        {
            return UseCaseResult<IReadOnlyList<AvailableDestinationDto>>.Fail("UseCase.Game.NoActiveGame");
        }

        var destinations = TravelMap.GetDestinationsFrom(state.CurrentLocation)
            .Select(destination => CreateDestination(state, destination))
            .ToList();

        return UseCaseResult<IReadOnlyList<AvailableDestinationDto>>.Ok(destinations, "UseCase.Action.Completed");
    }

    public UseCaseResult<ShopActionSummaryDto> GetShopActionSummary(ActionType actionType)
    {
        var summary = actionType switch
        {
            ActionType.BuyElectrolytes => new ShopActionSummaryDto
            {
                ActionType = actionType,
                TitleKey = "Shop.Electrolytes.Summary",
                Cost = GameEconomy.ElectrolyteCost,
                HealthChange = GameEconomy.ElectrolyteHealthGain,
                HangoverChange = -GameEconomy.ElectrolyteHangoverReduction,
                DrunkennessChange = -GameEconomy.ElectrolyteDrunkennessReduction,
                RemainingTimeChange = -GameEconomy.ElectrolyteTimeCost
            },
            ActionType.BuyFuel => new ShopActionSummaryDto
            {
                ActionType = actionType,
                TitleKey = "Shop.Fuel.Summary",
                Cost = GameEconomy.FuelCost,
                FuelChange = GameEconomy.FuelGain,
                RemainingTimeChange = -GameEconomy.FuelPurchaseTimeCost
            },
            ActionType.BuyAlcohol => new ShopActionSummaryDto
            {
                ActionType = actionType,
                TitleKey = "Shop.Alcohol.Summary",
                Cost = GameEconomy.AlcoholCost,
                HealthChange = -GameEconomy.AlcoholHealthCost,
                HangoverChange = -GameEconomy.AlcoholHangoverReduction,
                DrunkennessChange = GameEconomy.AlcoholDrunkennessIncrease,
                RemainingTimeChange = GameEconomy.AlcoholTimeGain,
                RemainingTimeLimit = GameLimits.MaxRemainingTime
            },
            ActionType.RestAtStripClub => new ShopActionSummaryDto
            {
                ActionType = actionType,
                TitleKey = "Shop.StripClubRest.Summary",
                Cost = GameEconomy.StripClubServiceCost,
                MinHealthChange = GameEconomy.StripClubServiceMinHealthGain,
                MaxHealthChange = GameEconomy.StripClubServiceMaxHealthGain,
                RemainingTimeChange = -GameEconomy.StripClubServiceTimeCost
            },
            _ => null
        };

        return summary is null
            ? UseCaseResult<ShopActionSummaryDto>.Fail("UseCase.Action.Failed")
            : UseCaseResult<ShopActionSummaryDto>.Ok(summary, "UseCase.Action.Completed");
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

    private static string? GetStripClubRestUnavailableReason(GameState state)
    {
        if (state.CurrentLocation != Location.StripClub)
        {
            return "Rules.StripClub.WrongLocation";
        }

        if (state.CharacterStats.Money < GameEconomy.StripClubServiceCost)
        {
            return "Rules.StripClub.NotEnoughMoney";
        }

        if (state.CharacterStats.Health >= GameLimits.MaxHealth)
        {
            return "Rules.StripClub.HealthFull";
        }

        return state.CharacterStats.RemainingTime > GameEconomy.StripClubServiceTimeCost
            ? null
            : "Rules.StripClub.NotEnoughTime";
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
