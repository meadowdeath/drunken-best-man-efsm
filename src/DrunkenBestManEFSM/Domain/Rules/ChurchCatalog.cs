using DrunkenBestManEFSM.Domain.Enums;

namespace DrunkenBestManEFSM.Domain.Rules;

/// <summary>
/// Defines the church locations available in the game.
/// </summary>
public static class ChurchCatalog
{
    private static readonly ChurchLocation[] Churches =
    [
        ChurchLocation.LostLoveParish,
        ChurchLocation.ForbiddenRoseChapel,
        ChurchLocation.LastGoodbyeSanctuary,
        ChurchLocation.SecretsOfTheSoulChurch,
        ChurchLocation.FinalDestinyCathedral
    ];

    public static IReadOnlyList<ChurchLocation> GetChurches() => Churches;

    public static Location ToLocation(ChurchLocation churchLocation) =>
        churchLocation switch
        {
            ChurchLocation.LostLoveParish => Location.LostLoveParish,
            ChurchLocation.ForbiddenRoseChapel => Location.ForbiddenRoseChapel,
            ChurchLocation.LastGoodbyeSanctuary => Location.LastGoodbyeSanctuary,
            ChurchLocation.SecretsOfTheSoulChurch => Location.SecretsOfTheSoulChurch,
            ChurchLocation.FinalDestinyCathedral => Location.FinalDestinyCathedral,
            _ => throw new ArgumentOutOfRangeException(nameof(churchLocation), churchLocation, null)
        };

    public static bool IsChurch(Location location) =>
        location is Location.LostLoveParish
            or Location.ForbiddenRoseChapel
            or Location.LastGoodbyeSanctuary
            or Location.SecretsOfTheSoulChurch
            or Location.FinalDestinyCathedral;
}
