using LostChildApp.Models;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace BusinessLayer.Models
{
    public class ReportMissingMsg
    {
        public string DependentName { get; set; }
        [Required(ErrorMessage ="You must provide a picture of the dependent")]
        public IFormFile DependentImage { get; set; }
        public string DependentImgURL { get; set; }
        [Required]
        public Contact Reporter { get; set; }
        public Coordinates Coordinates { get; set; }
        public bool ShareLocation { get; set; }

        public string PushNotificationKey { get; set; }
        public string PushNotificationEndpoint { get; set; }
        public string PushNotificationAuthSecret { get; set; }

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