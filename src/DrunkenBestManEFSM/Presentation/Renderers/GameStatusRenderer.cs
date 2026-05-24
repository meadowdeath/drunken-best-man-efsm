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
        printer.WriteSection(textProvider.GetText("Game.Status"));
        printer.WriteLine($"Location: {GetLocationName(status.CurrentLocation)}");
        printer.WriteLine($"Car Location: {GetLocationName(status.CarLocation)}");
        printer.WriteLine($"Health: {status.Health} / 100");
        printer.WriteLine($"Hangover: {status.Hangover} / 100");
        printer.WriteLine($"Drunkenness: {status.Drunkenness} / 100");
        printer.WriteLine($"Fuel: {status.Fuel} / 100");
        printer.WriteLine($"Remaining Time: {status.RemainingTime} minutes");
        printer.WriteLine($"Money: {status.Money}");
        printer.WriteLine($"Rings: {FormatBoolean(status.HasRings)}");
        printer.WriteLine($"Correct Church Known: {FormatBoolean(status.CorrectChurchKnown)}");

        if (status.CorrectChurchToDisplay is not null)
        {
            printer.WriteLine($"Correct Church: {GetText($"Location.{status.CorrectChurchToDisplay}")}");
        }

        printer.WriteLine($"Result: {status.Result}");
    }

    private string GetLocationName(object location) =>
        GetText($"Location.{location}");

    private string GetText(string key) =>
        textProvider.GetText(key);

    private static string FormatBoolean(bool value) =>
        value ? "Yes" : "No";
}
