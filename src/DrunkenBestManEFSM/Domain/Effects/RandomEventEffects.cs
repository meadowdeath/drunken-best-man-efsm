using DrunkenBestManEFSM.Domain.Enums;
using DrunkenBestManEFSM.Domain.Models;
using DrunkenBestManEFSM.Domain.Rules;

namespace DrunkenBestManEFSM.Domain.Effects;

/// <summary>
/// Applies selected random event effects without generating events.
/// </summary>
public static class RandomEventEffects
{
    public static void ApplyVomit(GameState state)
    {
        StatEffects.AddHealth(state.CharacterStats, -GameEconomy.VomitHealthCost);
        StatEffects.AddHangover(state.CharacterStats, GameEconomy.VomitHangoverIncrease);
        StatEffects.AddDrunkenness(state.CharacterStats, -GameEconomy.VomitDrunkennessReduction);
        StatEffects.AddRemainingTime(state.CharacterStats, -GameEconomy.VomitTimeCost);
    }

    public static void ApplyFindMoney(GameState state, int amount) =>
        StatEffects.AddMoney(state.CharacterStats, amount);

    public static void ApplyRandomEvent(GameState state, RandomEventType eventType, int moneyAmount = 0)
    {
        switch (eventType)
        {
            case RandomEventType.None:
                break;
            case RandomEventType.Vomit:
                ApplyVomit(state);
                break;
            case RandomEventType.FindMoney:
                ApplyFindMoney(state, moneyAmount);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(eventType), eventType, null);
        }
    }
}
