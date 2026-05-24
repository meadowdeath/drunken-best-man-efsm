using DrunkenBestManEFSM.Application.Contracts;
using DrunkenBestManEFSM.Application.DTOs;
using DrunkenBestManEFSM.Presentation.Console;

namespace DrunkenBestManEFSM.Presentation.Renderers;

public sealed class AvailableActionsRenderer
{
    private readonly ConsolePrinter printer;
    private readonly ITextProvider textProvider;

    public AvailableActionsRenderer(ConsolePrinter printer, ITextProvider textProvider)
    {
        this.printer = printer;
        this.textProvider = textProvider;
    }

    public void Render(IReadOnlyList<AvailableActionDto> actions)
    {
        printer.WriteSection("Available Actions");

        for (var index = 0; index < actions.Count; index++)
        {
            var action = actions[index];
            var label = textProvider.GetText(action.LabelKey);
            var suffix = action.IsAvailable
                ? string.Empty
                : $" unavailable: {textProvider.GetText(action.UnavailableReasonKey ?? string.Empty)}";

            printer.WriteLine($"{index + 1}. {label}{suffix}");
        }
    }
}
