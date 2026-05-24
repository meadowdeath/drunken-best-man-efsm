using DrunkenBestManEFSM.Application.Contracts;

namespace DrunkenBestManEFSM.Presentation.Console;

/// <summary>
/// Centralizes safe console input.
/// </summary>
public sealed class ConsoleInputReader
{
    private readonly ITextProvider textProvider;

    public ConsoleInputReader(ITextProvider textProvider)
    {
        this.textProvider = textProvider;
    }

    public string ReadRequiredText(string prompt)
    {
        while (true)
        {
            global::System.Console.Write(prompt);
            var input = global::System.Console.ReadLine();

            if (!string.IsNullOrWhiteSpace(input))
            {
                return input.Trim();
            }

            global::System.Console.WriteLine(textProvider.GetText("Menu.InvalidOption"));
        }
    }

    public int ReadIntInRange(string prompt, int min, int max)
    {
        while (true)
        {
            var input = ReadRequiredText(prompt);

            if (TryReadInt(input, out var value) && value >= min && value <= max)
            {
                return value;
            }

            global::System.Console.WriteLine($"{textProvider.GetText("Menu.InvalidOption")} ({min}-{max})");
        }
    }

    public bool TryReadInt(string input, out int value) =>
        int.TryParse(input, out value);

    public void WaitForEnter()
    {
        global::System.Console.WriteLine();
        global::System.Console.Write(textProvider.GetText("Menu.PressEnter"));
        global::System.Console.ReadLine();
    }

    public bool ReadConfirmation(string prompt)
    {
        while (true)
        {
            var input = ReadRequiredText(prompt);
            if (input.Equals("y", StringComparison.OrdinalIgnoreCase) || input.Equals("yes", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            if (input.Equals("n", StringComparison.OrdinalIgnoreCase) || input.Equals("no", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            global::System.Console.WriteLine(textProvider.GetText("Menu.InvalidOption"));
        }
    }
}
