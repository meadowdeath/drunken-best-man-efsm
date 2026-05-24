using DrunkenBestManEFSM.Application.Contracts;
using DrunkenBestManEFSM.Application.DTOs;
using DrunkenBestManEFSM.Presentation.Console;

namespace DrunkenBestManEFSM.Presentation.Renderers;

public sealed class GameStatusRenderer
{
    private readonly ConsolePrinter printer;
    private readonly ITextProvider textProvider;

    public GameStatusRenderer(ConsolePrinter printer, ITextProvider textProvider)
    {
        this.printer = printer;
        this.textProvider = textProvider;
    }

    public void Render(GameStatusDto status)
    {
        printer.WriteSection("LOCATION");
        printer.WriteLine($"{textProvider.GetText("Status.CurrentLocation"),-18} {GetLocationName(status.CurrentLocation)}");
        printer.WriteLine($"{textProvider.GetText("Status.CarLocation"),-18} {GetLocationName(status.CarLocation)}{GetCarWarning(status)}");

        printer.WriteSection("STATS");
        printer.WriteLine($"{textProvider.GetText("Status.Health"),-18} {status.Health} / 100");
        printer.WriteLine($"{textProvider.GetText("Status.Hangover"),-18} {status.Hangover} / 100");
        printer.WriteLine($"{textProvider.GetText("Status.Drunkenness"),-18} {status.Drunkenness} / 100");
        printer.WriteLine($"{textProvider.GetText("Status.Fuel"),-18} {status.Fuel} / 100");
        printer.WriteLine($"{textProvider.GetText("Status.RemainingTime"),-18} {status.RemainingTime} min");
        printer.WriteLine($"{textProvider.GetText("Status.Money"),-18} ${status.Money}");
        printer.WriteLine($"{textProvider.GetText("Status.Rings"),-18} {FormatBoolean(status.HasRings)}");
        printer.WriteLine($"{textProvider.GetText("Status.CorrectChurchKnown"),-18} {FormatBoolean(status.CorrectChurchKnown)}");

        if (status.CorrectChurchToDisplay is not null)
        {
            printer.WriteLine($"{textProvider.GetText("Status.CorrectChurch"),-18} {GetText($"Location.{status.CorrectChurchToDisplay}")}");
        }

        printer.WriteLine($"{"Result:",-18} {status.Result}");
    }

    private string GetLocationName(object location) =>
        GetText($"Location.{location}");

    private string GetText(string key) =>
        textProvider.GetText(key);

    private static string FormatBoolean(bool value) =>
        value ? "Yes" : "No";

    private string GetCarWarning(GameStatusDto status) =>
        status.CarLocation == status.CurrentLocation
            ? string.Empty
            : $" ({textProvider.GetText("Status.CarElsewhere")})";
}
