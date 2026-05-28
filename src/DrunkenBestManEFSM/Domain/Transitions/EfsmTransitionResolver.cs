using DrunkenBestManEFSM.Domain.Effects;
using DrunkenBestManEFSM.Domain.Enums;
using DrunkenBestManEFSM.Domain.Maps;
using DrunkenBestManEFSM.Domain.Models;
using DrunkenBestManEFSM.Domain.Results;
using DrunkenBestManEFSM.Domain.Rules;

namespace DrunkenBestManEFSM.Domain.Transitions;

/// <summary>
/// Coordinates EFSM rule checks and effects for attempted actions.
/// </summary>
public static class EfsmTransitionResolver
{
    public static ActionResult Resolve(GameState state, TransitionRequest request) =>
        request.ActionType switch
        {
            ActionType.Travel => ResolveTravel(state, request),
            ActionType.BuyElectrolytes => ResolveBuyElectrolytes(state, request),
            ActionType.BuyFuel => ResolveBuyFuel(state, request),
            ActionType.BuyAlcohol => ResolveBuyAlcohol(state, request),
            ActionType.RestAtStripClub => ResolveRestAtStripClub(state, request),
            ActionType.PickUpRings => ResolvePickUpRings(state, request),
            ActionType.EnterChurch => ResolveEnterChurch(state, request),
            ActionType.CheckStats => ResolveCheckStats(state),
            _ => CreateFailureResult(state, state.CurrentLocation, "Rules.Action.Unsupported")
        };

    private static ActionResult ResolveTravel(GameState state, TransitionRequest request)
    {
        var previousLocation = state.CurrentLocation;

        if (request.Destination is null)
        {
            return CreateFailureResult(state, previousLocation, "Rules.Travel.InvalidDestination");
        }

        if (request.TravelMode is null)
        {
            return CreateFailureResult(state, previousLocation, "Rules.Travel.InvalidMode");
        }

        var routeCost = TravelMap.GetRouteCost(state.CurrentLocation, request.Destination.Value, request.TravelMode.Value);
        if (routeCost is null)
        {
            return CreateFailureResult(state, previousLocation, "Rules.Travel.RouteNotFound");
        }

        var failureMessageKey = GetTravelFailureMessageKey(state, request.TravelMode.Value, routeCost);
        if (failureMessageKey is not null)
        {
            return CreateFailureResult(state, previousLocation, failureMessageKey);
        }

        if (request.TravelMode == TravelMode.Walk)
        {
            TravelEffects.ApplyWalk(state, request.Destination.Value, routeCost.TimeCost);
        }
        else
        {
            TravelEffects.ApplyDrive(state, request.Destination.Value, routeCost.TimeCost, routeCost.FuelCost);
        }

        return CompleteTurn(state, previousLocation, request.RandomEvent, "Actions.Travel.Success");
    }

    private static ActionResult ResolveBuyElectrolytes(GameState state, TransitionRequest request)
    {
        var previousLocation = state.CurrentLocation;

        if (state.CurrentLocation != Location.GasStation)
        {
            return CreateFailureResult(state, previousLocation, "Rules.Shop.Electrolytes.WrongLocation");
        }

        if (!ShopRules.CanBuyElectrolytes(state))
        {
            return CreateFailureResult(state, previousLocation, "Rules.Shop.NotEnoughMoney");
        }

        ShopEffects.ApplyBuyElectrolytes(state);
        return CompleteTurn(state, previousLocation, request.RandomEvent, "Actions.Shop.BuyElectrolytes.Success");
    }

    private static ActionResult ResolveBuyFuel(GameState state, TransitionRequest request)
    {
        var previousLocation = state.CurrentLocation;

        if (state.CurrentLocation != Location.GasStation)
        {
            return CreateFailureResult(state, previousLocation, "Rules.Shop.Fuel.WrongLocation");
        }

        if (state.CharacterStats.Money < GameEconomy.FuelCost)
        {
            return CreateFailureResult(state, previousLocation, "Rules.Shop.NotEnoughMoney");
        }

        if (state.CharacterStats.Fuel >= GameLimits.MaxFuel)
        {
            return CreateFailureResult(state, previousLocation, "Rules.Shop.Fuel.Full");
        }

        ShopEffects.ApplyBuyFuel(state);
        return CompleteTurn(state, previousLocation, request.RandomEvent, "Actions.Shop.BuyFuel.Success");
    }

    private static ActionResult ResolveBuyAlcohol(GameState state, TransitionRequest request)
    {
        var previousLocation = state.CurrentLocation;

        if (state.CurrentLocation != Location.Bar)
        {
            return CreateFailureResult(state, previousLocation, "Rules.Shop.Alcohol.WrongLocation");
        }

        if (state.CharacterStats.Money < GameEconomy.AlcoholCost)
        {
            return CreateFailureResult(state, previousLocation, "Rules.Shop.NotEnoughMoney");
        }

        if (state.CharacterStats.Health <= GameEconomy.AlcoholHealthCost)
        {
            return CreateFailureResult(state, previousLocation, "Rules.Shop.Alcohol.HealthTooLow");
        }

        if (state.CharacterStats.RemainingTime >= GameLimits.MaxRemainingTime)
        {
            return CreateFailureResult(state, previousLocation, "Rules.Shop.Alcohol.TimeLimitReached");
        }

        if (!ShopRules.CanBuyAlcohol(state))
        {
            return CreateFailureResult(state, previousLocation, "Rules.Shop.Alcohol.NotAllowed");
        }

        ShopEffects.ApplyBuyAlcohol(state);
        return CompleteTurn(state, previousLocation, request.RandomEvent, "Actions.Shop.BuyAlcohol.Success");
    }

    private static ActionResult ResolveRestAtStripClub(GameState state, TransitionRequest request)
    {
        var previousLocation = state.CurrentLocation;

        if (state.CurrentLocation != Location.StripClub)
        {
            return CreateFailureResult(state, previousLocation, "Rules.StripClub.WrongLocation");
        }

        if (state.CharacterStats.Money < GameEconomy.StripClubServiceCost)
        {
            return CreateFailureResult(state, previousLocation, "Rules.StripClub.NotEnoughMoney");
        }

        if (state.CharacterStats.Health >= GameLimits.MaxHealth)
        {
            return CreateFailureResult(state, previousLocation, "Rules.StripClub.HealthFull");
        }

        if (state.CharacterStats.RemainingTime <= GameEconomy.StripClubServiceTimeCost)
        {
            return CreateFailureResult(state, previousLocation, "Rules.StripClub.NotEnoughTime");
        }

        if (request.HealthGain is null || !ShopRules.IsValidStripClubHealthGain(request.HealthGain.Value))
        {
            return CreateFailureResult(state, previousLocation, "Rules.StripClub.InvalidHealthGain");
        }

        ShopEffects.ApplyRestAtStripClub(state, request.HealthGain.Value);
        return CompleteTurn(state, previousLocation, request.RandomEvent, "Actions.StripClub.Rest.Success");
    }

    private static ActionResult ResolvePickUpRings(GameState state, TransitionRequest request)
    {
        var previousLocation = state.CurrentLocation;

        if (state.CurrentLocation != Location.JewelryStore)
        {
            return CreateFailureResult(state, previousLocation, "Rules.Rings.WrongLocation");
        }

        if (state.HasRings)
        {
            return CreateFailureResult(state, previousLocation, "Rules.Rings.AlreadyPickedUp");
        }

        if (!RingRules.CanPickUpRings(state))
        {
            return CreateFailureResult(state, previousLocation, "Rules.Rings.NotEnoughTime");
        }

        RingEffects.ApplyPickUpRings(state);
        return CompleteTurn(state, previousLocation, request.RandomEvent, "Actions.Rings.PickUp.Success");
    }

    private static ActionResult ResolveEnterChurch(GameState state, TransitionRequest request)
    {
        var previousLocation = state.CurrentLocation;

        if (!ChurchRules.CanEnterChurch(state))
        {
            return CreateFailureResult(state, previousLocation, "Rules.Church.NotAtChurch");
        }

        if (ChurchRules.IsCorrectChurch(state, state.CurrentLocation) && state.HasRings)
        {
            ChurchEffects.ApplyVictory(state);
            return CreateSuccessResult(state, previousLocation, "Actions.Church.Enter.Victory", randomEvent: null);
        }

        if (ChurchRules.IsCorrectChurch(state, state.CurrentLocation) && !state.HasRings)
        {
            return CompleteTurn(state, previousLocation, request.RandomEvent, "Actions.Church.Enter.CorrectChurchMissingRings");
        }

        ChurchEffects.ApplyWrongChurchPenalty(state);
        return CompleteTurn(state, previousLocation, request.RandomEvent, "Actions.Church.Enter.WrongChurch");
    }

    private static ActionResult ResolveCheckStats(GameState state) =>
        CreateSuccessResult(state, state.CurrentLocation, "Actions.CheckStats.Success", randomEvent: null);

    private static string? GetTravelFailureMessageKey(GameState state, TravelMode travelMode, RouteCost routeCost)
    {
        if (!TravelRules.HasEnoughTime(state.CharacterStats, routeCost.TimeCost))
        {
            return "Rules.Travel.NotEnoughTime";
        }

        if (travelMode == TravelMode.Walk)
        {
            return TravelRules.CanWalk(state) ? null : "Rules.Travel.CannotWalk";
        }

        if (!TravelRules.CanDriveFromCurrentLocation(state))
        {
            return "Rules.Travel.CarNotHere";
        }

        if (!TravelRules.HasEnoughFuel(state.CharacterStats, routeCost.FuelCost))
        {
            return "Rules.Travel.NotEnoughFuel";
        }

        if (state.CharacterStats.Drunkenness >= GameLimits.DrivingDrunkennessLimit)
        {
            return "Rules.Travel.TooDrunkToDrive";
        }

        return TravelRules.CanTravel(state, travelMode, routeCost.TimeCost, routeCost.FuelCost)
            ? null
            : "Rules.Travel.CannotDrive";
    }

    private static ActionResult CompleteTurn(
        GameState state,
        Location previousLocation,
        RandomEventResult? randomEvent,
        string successMessageKey)
    {
        TurnEffects.ApplyPassiveTurnEffects(state);
        ApplyProvidedRandomEvent(state, randomEvent);
        UpdateMemoryIfEligible(state);
        UpdateGameOutcome(state);

        return CreateSuccessResult(state, previousLocation, successMessageKey, randomEvent);
    }

    private static void ApplyProvidedRandomEvent(GameState state, RandomEventResult? randomEvent)
    {
        if (randomEvent is null || !randomEvent.Occurred)
        {
            return;
        }

        switch (randomEvent.EventType)
        {
            case RandomEventType.None:
                break;
            case RandomEventType.Vomit:
                RandomEventEffects.ApplyVomit(state);
                break;
            case RandomEventType.FindMoney:
                RandomEventEffects.ApplyFindMoney(state, randomEvent.MoneyChange);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(randomEvent), randomEvent.EventType, null);
        }
    }

    private static void UpdateMemoryIfEligible(GameState state)
    {
        if (!state.CorrectChurchKnown && MemoryRules.CanRememberCorrectChurch(state))
        {
            state.CorrectChurchKnown = true;
        }
    }

    private static void UpdateGameOutcome(GameState state)
    {
        var result = GameOutcomeRules.GetGameResult(state);
        state.Result = result;

        if (result == GameResult.Victory)
        {
            state.CurrentLocation = Location.Victory;
        }
        else if (result == GameResult.Defeat)
        {
            state.CurrentLocation = Location.Defeat;
        }
    }

    private static ActionResult CreateFailureResult(GameState state, Location previousLocation, string messageKey) =>
        new()
        {
            Success = false,
            PreviousLocation = previousLocation,
            CurrentLocation = state.CurrentLocation,
            GameResult = state.Result,
            MessageKey = messageKey
        };

    private static ActionResult CreateSuccessResult(
        GameState state,
        Location previousLocation,
        string messageKey,
        RandomEventResult? randomEvent) =>
        new()
        {
            Success = true,
            PreviousLocation = previousLocation,
            CurrentLocation = state.CurrentLocation,
            GameResult = state.Result,
            MessageKey = messageKey,
            RandomEvent = randomEvent is { Occurred: true } ? randomEvent : null
        };
}
