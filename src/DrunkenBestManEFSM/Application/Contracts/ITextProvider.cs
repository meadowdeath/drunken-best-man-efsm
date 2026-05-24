namespace DrunkenBestManEFSM.Application.Contracts;

/// <summary>
/// Provides narrative or UI text by message key.
/// </summary>
public interface ITextProvider
{
    string GetText(string key);
}
