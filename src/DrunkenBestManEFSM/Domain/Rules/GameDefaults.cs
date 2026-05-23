using DrunkenBestManEFSM.Domain.Enums;

namespace DrunkenBestManEFSM.Domain.Rules;

/// <summary>
/// Defines the default values for a new game.
/// </summary>
public static class GameDefaults
{
    public const Location InitialLocation = Location.StripClub;
    public const Location InitialCarLocation = Location.StripClub;
    public const int InitialHealth = 30;
    public const int InitialHangover = 80;
    public const int InitialDrunkenness = 45;
    public const int InitialFuel = 15;
    public const int InitialRemainingTime = 60;
    public const int InitialMoney = 120;
    public const bool InitialHasRings = false;
    public const bool InitialCorrectChurchKnown = false;
}
