namespace BusinessLayer.Models
{
    public interface IGeolocator
    {
        BoundingBox GetBoundingBox(Coordinates coordinates, double radiusInMiles);
    }
}