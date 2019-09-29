namespace BusinessLayer.Models
{
    public class Location
    {
        private string streetAddress;
        private string city;
        private string state;
        private Coordinates coordinates;

        public string StreetAddress { get => streetAddress; set => streetAddress = value; }
        public string City { get => city; set => city = value; }
        public string State { get => state; set => state = value; }
        //public Coordinates Coordinates { get => coordinates; set => coordinates = value; }

        public Coordinates Coordinates { get; set; }
    }
}