namespace Lumora.Domain.Entities.Common.ValueObjects;

public class Coordinates
{
    public double Latitude { get; init; }
    public double Longitude { get; init; }
    public string LocationName { get; init; }

    public Coordinates(double latitude, double longitude, string locationName)
    {
        if (latitude < -90 || latitude > 90)
            throw new ArgumentException("Latitude must be between -90 and 90");
        if (longitude < -180 || longitude > 180)
            throw new ArgumentException("Longitude must be between -180 and 180");

        Latitude = latitude;
        Longitude = longitude;
        LocationName = locationName;
    }

    public override bool Equals(object? obj) => 
        obj is Coordinates other && Latitude == other.Latitude && Longitude == other.Longitude;

    public override int GetHashCode() =>
        HashCode.Combine(Latitude, Longitude);

    public override string ToString()
    {
        return $"{Latitude}, {Longitude}";
    }
}
