using DrunkenBestManEFSM.Application.Contracts;

namespace DrunkenBestManEFSM.Infrastructure.Xml;

/// <summary>
/// Provides text values loaded from XML.
/// </summary>
public sealed class XmlTextProvider : ITextProvider
{
    private readonly IReadOnlyDictionary<string, string> texts;

    public XmlTextProvider(string filePath)
    {
        texts = XmlTextLoader.Load(filePath);
    }

    public XmlTextProvider(IReadOnlyDictionary<string, string> texts)
    {
        this.texts = texts;
    }

    public string GetText(string key) =>
        texts.TryGetValue(key, out var text)
            ? text
            : $"[Missing text: {key}]";
}
