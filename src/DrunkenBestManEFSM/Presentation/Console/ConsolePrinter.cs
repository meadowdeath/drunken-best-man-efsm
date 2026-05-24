namespace DrunkenBestManEFSM.Presentation.Console;

/// <summary>
/// Centralizes basic console output helpers.
/// </summary>
public sealed class ConsolePrinter
{
    public void WriteTitle(string text)
    {
        WriteLine(text);
        WriteLine(new string('=', text.Length));
    }

    public void WriteSection(string text)
    {
        WriteEmptyLine();
        WriteLine(text);
        WriteLine(new string('-', text.Length));
    }

    public void WriteLine(string text) =>
        global::System.Console.WriteLine(text);

    public void WriteError(string text)
    {
        var previousColor = global::System.Console.ForegroundColor;
        global::System.Console.ForegroundColor = ConsoleColor.Red;
        global::System.Console.WriteLine(text);
        global::System.Console.ForegroundColor = previousColor;
    }

    public void WriteEmptyLine() =>
        global::System.Console.WriteLine();

    public void Clear()
    {
        try
        {
            global::System.Console.Clear();
        }
        catch (IOException)
        {
            WriteEmptyLine();
        }
    }
}
