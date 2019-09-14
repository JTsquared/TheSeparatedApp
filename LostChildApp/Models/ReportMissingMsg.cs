﻿using LostChildApp.Models;
using Microsoft.AspNetCore.Http;

namespace LostFamily.Models
{
    public class ReportMissingMsg
    {
        public string DependentName { get; set; }
        public IFormFile DependentImage { get; set; }
        public string DependentImgURL { get; set; }
        public Contact Reporter { get; set; }
        public Location Location { get; set; }

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

        public ReportMissingMsg(string childName, string dependentImgURL, Contact reporter, Location location)
        {
            DependentName = childName;
            DependentImgURL = dependentImgURL;
            Reporter = reporter;
            Location = location;
        }
    }
}