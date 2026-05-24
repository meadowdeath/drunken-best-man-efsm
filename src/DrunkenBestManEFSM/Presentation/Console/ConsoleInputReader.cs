namespace DrunkenBestManEFSM.Presentation.Console;

/// <summary>
/// Centralizes safe console input.
/// </summary>
public sealed class ConsoleInputReader
{
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

            global::System.Console.WriteLine("Please enter a value.");
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

            global::System.Console.WriteLine($"Please enter a number between {min} and {max}.");
        }
    }

    public bool TryReadInt(string input, out int value) =>
        int.TryParse(input, out value);

    public void WaitForEnter()
    {
        global::System.Console.WriteLine();
        global::System.Console.Write("Press Enter to continue...");
        global::System.Console.ReadLine();
    }
}
