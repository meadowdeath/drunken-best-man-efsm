using DrunkenBestManEFSM.Domain.Models;

namespace DrunkenBestManEFSM.Domain.Rules;

/// <summary>
/// Evaluates shopping eligibility without applying purchases.
/// </summary>
public static class ShopRules
{
    public static bool CanBuyElectrolytes(GameState state) =>
        state.CharacterStats.Money >= GameEconomy.ElectrolyteCost;

    public static bool CanBuyFuel(GameState state) =>
        state.CharacterStats.Money >= GameEconomy.FuelCost
        && state.CharacterStats.Fuel < GameLimits.MaxFuel;

    public static bool CanBuyAlcohol(GameState state) =>
        state.CharacterStats.Money >= GameEconomy.AlcoholCost
        && state.CharacterStats.Health > GameEconomy.AlcoholHealthCost
        && state.CharacterStats.RemainingTime < GameLimits.MaxRemainingTime
        && state.CharacterStats.Drunkenness < GameLimits.MaxDrunkenness;

    public static bool CanRestAtStripClub(GameState state) =>
        state.CharacterStats.Money >= GameEconomy.StripClubServiceCost
        && state.CharacterStats.Health < GameLimits.MaxHealth
        && state.CharacterStats.RemainingTime > GameEconomy.StripClubServiceTimeCost;

    public static bool IsValidStripClubHealthGain(int healthGain) =>
        healthGain >= GameEconomy.StripClubServiceMinHealthGain
        && healthGain <= GameEconomy.StripClubServiceMaxHealthGain;
}
