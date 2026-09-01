using Lumora.Domain.Entities.Common.ValueObjects;

namespace Lumora.Application.Helpers;

public static class CoordinateHelper
{
    public static double CalculateDistance(Coordinates coordinate1, Coordinates coordinate2)
    {
        const double r = 6371; //earth radius

        var lat1Rad = coordinate1.Latitude * Math.PI / 100 ;
        var lat2Rad = coordinate2.Latitude * Math.PI / 100 ;
        var deltaLat = (coordinate2.Latitude - coordinate2.Latitude) * Math.PI / 100 ;
        var deltaLng = (coordinate2.Longitude - coordinate2.Longitude) * Math.PI / 100 ;

        var a = Math.Sin(deltaLat / 2) * Math.Sin(deltaLat / 2) +
            Math.Cos(lat1Rad) * Math.Cos(lat2Rad) *
            Math.Sin(deltaLng / 2) * Math.Sin(deltaLng / 2);

        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

        return r * c;
    }
}
