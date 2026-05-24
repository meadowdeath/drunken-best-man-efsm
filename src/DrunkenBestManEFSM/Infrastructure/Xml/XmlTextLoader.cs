using System.Xml.Linq;

namespace DrunkenBestManEFSM.Infrastructure.Xml;

/// <summary>
/// Loads text key/value pairs from the game text XML file.
/// </summary>
public static class XmlTextLoader
{
    public static IReadOnlyDictionary<string, string> Load(string filePath)
    {
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException("Game text XML file was not found.", filePath);
        }

        var document = XDocument.Load(filePath);
        var root = document.Root;

        if (root is null || root.Name != "GameTexts")
        {
            throw new InvalidOperationException("Game text XML must have a GameTexts root element.");
        }

        var texts = new Dictionary<string, string>();

        foreach (var textElement in root.Elements("Text"))
        {
            var key = (string?)textElement.Attribute("key");
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new InvalidOperationException("Each Text element must include a non-empty key attribute.");
            }

            texts[key] = textElement.Value.Trim();
        }

        return texts;
    }
}
