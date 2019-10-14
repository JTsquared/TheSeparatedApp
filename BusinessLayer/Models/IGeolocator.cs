namespace BusinessLayer.Models
{
    public interface IGeolocator
    {
        BoundingBox CreateBoundingBox(Coordinates coordinates, double radiusInMiles);
    }
}