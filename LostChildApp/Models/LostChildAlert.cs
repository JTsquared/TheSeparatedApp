namespace LostChild.Models
{
    public class LostChildAlert
    {
        private string childName;
        private string comments;
        private IImage childImage;
        private IContact contact;

        public LostChildAlert(string name, IImage image, IContact reporter)
        {
            childName = name;
            childImage = image;
            contact = reporter;
        }

        public LostChildAlert(string name, IImage image, IContact reporter, string reporterComments)
        {
            childName = name;
            childImage = image;
            contact = reporter;
            comments = reporterComments;
        }

        public string ChildName { get => childName; set => childName = value; }
        public IImage ChildImage { get => childImage; set => childImage = value; }
        public IContact Contact { get => contact; set => contact = value; }
        public string Comments { get => comments; set => comments = value; }
    }
}