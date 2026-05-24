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
        printer.WriteSection("Available Destinations");

        for (var index = 0; index < destinations.Count; index++)
        {
            var destination = destinations[index];
            printer.WriteLine($"{index + 1}. {textProvider.GetText($"Location.{destination.Destination}")}");
            printer.WriteLine($"   Walk: {FormatAvailability(destination.CanWalk)}, {destination.WalkTimeCost} min{FormatReason(destination.WalkUnavailableReasonKey)}");
            printer.WriteLine($"   Drive: {FormatAvailability(destination.CanDrive)}, {destination.DriveTimeCost} min, {destination.DriveFuelCost} fuel{FormatReason(destination.DriveUnavailableReasonKey)}");
        }
    }

    private static string FormatAvailability(bool isAvailable) =>
        isAvailable ? "available" : "unavailable";

    private string FormatReason(string? reasonKey) =>
        string.IsNullOrWhiteSpace(reasonKey)
            ? string.Empty
            : $": {textProvider.GetText(reasonKey)}";
}
