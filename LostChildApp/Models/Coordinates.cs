namespace LostFamily.Models
{
    public class Coordinates
    {
        private double longitude;
        private double latitude;

        public double Longitude { get => longitude; }
        public double Latitude { get => latitude; }

        public Coordinates(double longitude, double latitude)
        {
            this.longitude = longitude;
            this.latitude = latitude;
        }

        public void ChangeCoordinates(double longitude, double latitude)
        {
            this.longitude = longitude;
            this.latitude = latitude;
        }
    }
}