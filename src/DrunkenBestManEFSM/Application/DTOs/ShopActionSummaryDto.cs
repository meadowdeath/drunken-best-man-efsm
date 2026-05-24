using DrunkenBestManEFSM.Domain.Enums;

namespace DrunkenBestManEFSM.Application.DTOs;

public sealed class ShopActionSummaryDto
{
    public ActionType ActionType { get; set; }

    public string TitleKey { get; set; } = string.Empty;

    public int Cost { get; set; }

    public int HealthChange { get; set; }

    public int HangoverChange { get; set; }

    public int DrunkennessChange { get; set; }

    public int FuelChange { get; set; }

    public int RemainingTimeChange { get; set; }

    public int? RemainingTimeLimit { get; set; }
}
