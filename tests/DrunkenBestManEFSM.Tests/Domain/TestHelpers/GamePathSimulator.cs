using DrunkenBestManEFSM.Domain.Enums;
using DrunkenBestManEFSM.Domain.Models;
using DrunkenBestManEFSM.Domain.Results;
using DrunkenBestManEFSM.Domain.Rules;
using DrunkenBestManEFSM.Domain.Transitions;

namespace DrunkenBestManEFSM.Tests.Domain.TestHelpers;

public sealed class GamePathSimulator
{
    public GamePathSimulator(GameState? state = null)
    {
        State = state ?? GameStateFactory.CreateInitialState(ChurchLocation.LastGoodbyeSanctuary, memoryThreshold: 40);
    }

    public GameState State { get; }

    public ActionResult? LastResult { get; private set; }

    public ActionResult Travel(Location destination, TravelMode mode) =>
        Resolve(new TransitionRequest
        {
            ActionType = ActionType.Travel,
            Destination = destination,
            TravelMode = mode
        });

    public ActionResult BuyElectrolytes() =>
        Resolve(new TransitionRequest { ActionType = ActionType.BuyElectrolytes });

    public ActionResult BuyFuel() =>
        Resolve(new TransitionRequest { ActionType = ActionType.BuyFuel });

    public ActionResult BuyAlcohol() =>
        Resolve(new TransitionRequest { ActionType = ActionType.BuyAlcohol });

    public ActionResult RestAtStripClub(int healthGain) =>
        Resolve(new TransitionRequest
        {
            ActionType = ActionType.RestAtStripClub,
            HealthGain = healthGain
        });

    public ActionResult PickUpRings() =>
        Resolve(new TransitionRequest { ActionType = ActionType.PickUpRings });

    public ActionResult EnterChurch() =>
        Resolve(new TransitionRequest { ActionType = ActionType.EnterChurch });

    private ActionResult Resolve(TransitionRequest request)
    {
        LastResult = EfsmTransitionResolver.Resolve(State, request);
        return LastResult;
    }
}
