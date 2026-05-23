namespace DrunkenBestManEFSM.Domain.Rules;

/// <summary>
/// Defines economy and item configuration values.
/// </summary>
public static class GameEconomy
{
    public const int ElectrolyteCost = 30;
    public const int ElectrolyteHealthGain = 20;
    public const int ElectrolyteHangoverReduction = 30;
    public const int ElectrolyteDrunkennessReduction = 5;
    public const int ElectrolyteTimeCost = 2;

    public const int FuelCost = 20;
    public const int FuelGain = 15;
    public const int FuelPurchaseTimeCost = 1;

    public const int AlcoholCost = 25;
    public const int AlcoholHealthCost = 10;
    public const int AlcoholHangoverReduction = 15;
    public const int AlcoholDrunkennessIncrease = 25;
    public const int AlcoholTimeGain = 5;

    public const int PickUpRingsTimeCost = 5;
    public const int WrongChurchTimePenalty = 5;

    public const int VomitHealthCost = 10;
    public const int VomitHangoverIncrease = 5;
    public const int VomitDrunkennessReduction = 10;
    public const int VomitTimeCost = 3;

    public const int MoneyFoundMin = 10;
    public const int MoneyFoundMax = 40;

    public const int PassiveHangoverIncrease = 3;
    public const int PassiveDrunkennessReduction = 5;
}
