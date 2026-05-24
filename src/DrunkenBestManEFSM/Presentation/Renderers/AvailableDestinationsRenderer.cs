using DrunkenBestManEFSM.Application.Contracts;
using DrunkenBestManEFSM.Application.DTOs;
using DrunkenBestManEFSM.Presentation.Console;

namespace DrunkenBestManEFSM.Presentation.Renderers;

public sealed class AvailableDestinationsRenderer
{
    private readonly ConsolePrinter printer;
    private readonly ITextProvider textProvider;

    public AvailableDestinationsRenderer(ConsolePrinter printer, ITextProvider textProvider)
    {
        this.printer = printer;
        this.textProvider = textProvider;
    }

    public void Render(IReadOnlyList<AvailableDestinationDto> destinations)
    {
        printer.WriteSection("AVAILABLE DESTINATIONS");

        for (var index = 0; index < destinations.Count; index++)
        {
            var destination = destinations[index];
            printer.WriteLine($"{index + 1}. {textProvider.GetText($"Location.{destination.Destination}")}");
            printer.WriteLine($"   {textProvider.GetText("TravelMode.Walk")}:  {FormatTravel(destination.CanWalk, destination.WalkTimeCost, 0, destination.WalkUnavailableReasonKey)}");
            printer.WriteLine($"   {textProvider.GetText("TravelMode.Drive")}: {FormatTravel(destination.CanDrive, destination.DriveTimeCost, destination.DriveFuelCost, destination.DriveUnavailableReasonKey)}");
        }

        printer.WriteLine($"0. {textProvider.GetText("Menu.Cancel")}");
    }

    private string FormatTravel(bool isAvailable, int timeCost, int fuelCost, string? reasonKey)
    {
        var costText = $"{timeCost} min, {fuelCost} fuel";
        return isAvailable
            ? costText
            : $"unavailable - {textProvider.GetText(reasonKey ?? "Menu.InvalidOption")} ({costText})";
    }

    private string FormatReason(string? reasonKey) =>
        string.IsNullOrWhiteSpace(reasonKey)
            ? string.Empty
            : $": {textProvider.GetText(reasonKey)}";
}
