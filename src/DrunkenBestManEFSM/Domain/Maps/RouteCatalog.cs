using DrunkenBestManEFSM.Domain.Enums;
using DrunkenBestManEFSM.Domain.Models;

namespace DrunkenBestManEFSM.Domain.Maps;

/// <summary>
/// Defines route costs between connected locations.
/// </summary>
public static class RouteCatalog
{
    public const int NearWalkTime = 8;
    public const int NearDriveTime = 4;
    public const int NearDriveFuel = 8;

    public const int MediumWalkTime = 14;
    public const int MediumDriveTime = 7;
    public const int MediumDriveFuel = 15;

    public const int FarWalkTime = 22;
    public const int FarDriveTime = 11;
    public const int FarDriveFuel = 25;

    private static readonly IReadOnlyList<RouteCost> Routes = CreateRoutes();

    public static IReadOnlyList<RouteCost> GetAllRoutes() => Routes;

    private static IReadOnlyList<RouteCost> CreateRoutes()
    {
        var routes = new List<RouteCost>();

        AddMediumRoute(routes, Location.StripClub, Location.GasStation);
        AddNearRoute(routes, Location.StripClub, Location.Bar);
        AddMediumRoute(routes, Location.GasStation, Location.JewelryStore);
        AddNearRoute(routes, Location.GasStation, Location.Bar);
        AddNearRoute(routes, Location.GasStation, Location.Casino);
        AddNearRoute(routes, Location.Bar, Location.Casino);
        AddMediumRoute(routes, Location.JewelryStore, Location.Casino);
        AddMediumRoute(routes, Location.JewelryStore, Location.LostLoveParish);
        AddMediumRoute(routes, Location.JewelryStore, Location.ForbiddenRoseChapel);
        AddMediumRoute(routes, Location.JewelryStore, Location.LastGoodbyeSanctuary);
        AddFarRoute(routes, Location.JewelryStore, Location.SecretsOfTheSoulChurch);
        AddFarRoute(routes, Location.JewelryStore, Location.FinalDestinyCathedral);
        AddNearRoute(routes, Location.LostLoveParish, Location.ForbiddenRoseChapel);
        AddNearRoute(routes, Location.ForbiddenRoseChapel, Location.LastGoodbyeSanctuary);
        AddNearRoute(routes, Location.LastGoodbyeSanctuary, Location.SecretsOfTheSoulChurch);
        AddNearRoute(routes, Location.SecretsOfTheSoulChurch, Location.FinalDestinyCathedral);
        AddMediumRoute(routes, Location.FinalDestinyCathedral, Location.LostLoveParish);

        return routes;
    }

    private static void AddNearRoute(List<RouteCost> routes, Location from, Location to) =>
        AddBidirectionalRoute(routes, from, to, NearWalkTime, NearDriveTime, NearDriveFuel);

    private static void AddMediumRoute(List<RouteCost> routes, Location from, Location to) =>
        AddBidirectionalRoute(routes, from, to, MediumWalkTime, MediumDriveTime, MediumDriveFuel);

    private static void AddFarRoute(List<RouteCost> routes, Location from, Location to) =>
        AddBidirectionalRoute(routes, from, to, FarWalkTime, FarDriveTime, FarDriveFuel);

    private static void AddBidirectionalRoute(
        List<RouteCost> routes,
        Location from,
        Location to,
        int walkTime,
        int driveTime,
        int driveFuel)
    {
        AddRoute(routes, from, to, TravelMode.Walk, walkTime, fuelCost: 0);
        AddRoute(routes, from, to, TravelMode.Drive, driveTime, driveFuel);
        AddRoute(routes, to, from, TravelMode.Walk, walkTime, fuelCost: 0);
        AddRoute(routes, to, from, TravelMode.Drive, driveTime, driveFuel);
    }

    private static void AddRoute(
        List<RouteCost> routes,
        Location from,
        Location to,
        TravelMode travelMode,
        int timeCost,
        int fuelCost)
    {
        routes.Add(new RouteCost
        {
            From = from,
            To = to,
            TravelMode = travelMode,
            TimeCost = timeCost,
            FuelCost = fuelCost
        });
    }
}
