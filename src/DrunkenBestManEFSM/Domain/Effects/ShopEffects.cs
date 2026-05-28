using DrunkenBestManEFSM.Domain.Models;
using DrunkenBestManEFSM.Domain.Rules;

namespace DrunkenBestManEFSM.Domain.Effects;

/// <summary>
/// Applies item purchase effects without deciding whether a purchase is allowed.
/// </summary>
public static class ShopEffects
{
    public static void ApplyBuyElectrolytes(GameState state)
    {
        StatEffects.AddMoney(state.CharacterStats, -GameEconomy.ElectrolyteCost);
        StatEffects.AddHealth(state.CharacterStats, GameEconomy.ElectrolyteHealthGain);
        StatEffects.AddHangover(state.CharacterStats, -GameEconomy.ElectrolyteHangoverReduction);
        StatEffects.AddDrunkenness(state.CharacterStats, -GameEconomy.ElectrolyteDrunkennessReduction);
        StatEffects.AddRemainingTime(state.CharacterStats, -GameEconomy.ElectrolyteTimeCost);
    }

    public static void ApplyBuyFuel(GameState state)
    {
        StatEffects.AddMoney(state.CharacterStats, -GameEconomy.FuelCost);
        StatEffects.AddFuel(state.CharacterStats, GameEconomy.FuelGain);
        StatEffects.AddRemainingTime(state.CharacterStats, -GameEconomy.FuelPurchaseTimeCost);
    }

    public static void ApplyBuyAlcohol(GameState state)
    {
        StatEffects.AddMoney(state.CharacterStats, -GameEconomy.AlcoholCost);
        StatEffects.AddHealth(state.CharacterStats, -GameEconomy.AlcoholHealthCost);
        StatEffects.AddHangover(state.CharacterStats, -GameEconomy.AlcoholHangoverReduction);
        StatEffects.AddDrunkenness(state.CharacterStats, GameEconomy.AlcoholDrunkennessIncrease);
        StatEffects.AddRemainingTime(state.CharacterStats, GameEconomy.AlcoholTimeGain);
    }

    public static void ApplyRestAtStripClub(GameState state, int healthGain)
    {
        StatEffects.AddMoney(state.CharacterStats, -GameEconomy.StripClubServiceCost);
        StatEffects.AddHealth(state.CharacterStats, healthGain);
        StatEffects.AddRemainingTime(state.CharacterStats, -GameEconomy.StripClubServiceTimeCost);
    }
}
