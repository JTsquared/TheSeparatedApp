using LostChildApp.Models;
using Microsoft.AspNetCore.Http;

namespace BusinessLayer.Models
{
    public class ReportMissingMsg
    {
        public string DependentName { get; set; }
        //public IFormFile DependentImage { get; set; }
        public string DependentImgURL { get; set; }
        public Contact Reporter { get; set; }
        public Coordinates Coordinates { get; set; }

        public ReportMissingMsg() { }

        public ReportMissingMsg(string dependentImgURL, Contact reporter)
        {
            DependentImgURL = dependentImgURL;
            Reporter = reporter;
        }

        public ReportMissingMsg(string childName, string dependentImgURL, Contact reporter)
        {
            DependentName = childName;
            DependentImgURL = dependentImgURL;
            Reporter = reporter;
        }

        public ReportMissingMsg(string childName, string dependentImgURL, Contact reporter, Coordinates coordinates)
        {
            DependentName = childName;
            DependentImgURL = dependentImgURL;
            Reporter = reporter;
            Coordinates = coordinates;
        }
    }
}