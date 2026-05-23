using DrunkenBestManEFSM.Domain.Enums;
using DrunkenBestManEFSM.Domain.Models;

namespace DrunkenBestManEFSM.Domain.Maps;

/// <summary>
/// Exposes travel route lookups without evaluating game-state availability.
/// </summary>
public static class TravelMap
{
    public static IReadOnlyList<RouteCost> GetAllRoutes() =>
        RouteCatalog.GetAllRoutes();

    public static IReadOnlyList<RouteCost> GetRoutesFrom(Location location) =>
        RouteCatalog.GetAllRoutes()
            .Where(route => route.From == location)
            .ToList();

    public static IReadOnlyList<Location> GetDestinationsFrom(Location location) =>
        GetRoutesFrom(location)
            .Select(route => route.To)
            .Distinct()
            .ToList();

    public static RouteCost? GetRouteCost(Location from, Location to, TravelMode travelMode) =>
        RouteCatalog.GetAllRoutes()
            .FirstOrDefault(route =>
                route.From == from
                && route.To == to
                && route.TravelMode == travelMode);

    public static bool RouteExists(Location from, Location to, TravelMode travelMode) =>
        GetRouteCost(from, to, travelMode) is not null;
}
