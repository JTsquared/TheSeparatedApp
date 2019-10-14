using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Web;
using LostChildApp.Models;
using BusinessLayer.Models;

namespace LostChildApp.Controllers
{
    public class ReportMissingController : Controller
    {
        private readonly IHostingEnvironment hostingEnvironment;
        private readonly IMessageRepository mq;
        private readonly IImageService imageService;
        private readonly IMobileHelper mobileHelper;

        private const double SEARCH_RADIUS_IN_MILES = 5.0d;

        public ReportMissingController(IHostingEnvironment hostingEnvironment, IMessageRepository mq, IImageService imageService, IMobileHelper mobileHelper)
        {
            this.hostingEnvironment = hostingEnvironment;
            this.mq = mq;
            this.imageService = imageService;
            this.mobileHelper = mobileHelper;

        }

        public IActionResult Index()
        {
            ReportMissingMsg missingReport = new ReportMissingMsg();
            //return View(missingReport);
            return View(missingReport);
        }

        [HttpPost]
        public IActionResult FamilyReport(ReportMissingMsg model, IFormFile imageFile2, string useMobileLocation)
        {
            bool canSendReporterLocation = false;
            model.Reporter.ContactType = ContactType.Family;

            //the reporter's mobile location is required but if the below checkbox on the post form is checked they they have given permission to send their location to the other party
            if (useMobileLocation == "on")
            {
                canSendReporterLocation = true;
            }

            model.DependentImgURL = imageService.SaveImageToStorage(imageFile2);

            ReportMissingMsgAdaptor matchedReport = GetMissingReportMatch(model, canSendReporterLocation);

            if (matchedReport != null)
            {
                return View("ReportMatch", matchedReport);
            }
            else
            {
                return View("ReportAdded");
            }
        }

        [HttpPost]
        public IActionResult NonFamilyReport(ReportMissingMsg model, IFormFile imageFile, string useMobileLocation)
        {
            bool canSendReporterLocation = false;
            model.Reporter.ContactType = ContactType.NonFamily;

            //the reporter's mobile location is required but if the below checkbox on the post form is checked they they have given permission to send their location to the other party
            if (useMobileLocation == "on")
            {
                canSendReporterLocation = true;
            }

            model.DependentImgURL = imageService.SaveImageToStorage(imageFile);

            ReportMissingMsgAdaptor matchedReport = GetMissingReportMatch(model, canSendReporterLocation);

            if (matchedReport != null)
            {
                return View("ReportMatch", matchedReport);
            }
            else
            {
                return View("ReportAdded");
            }
        }

        private ReportMissingMsgAdaptor GetMissingReportMatch(ReportMissingMsg model, bool canSendReporterLocation)
        {
            ReportMissingMsgAdaptor matchedReport = null;

            if (model != null)
            {
                try
                {
                    //search for existing reports in msg queue based on user proximity
                    List<ReportMissingMsgAdaptor> possibleMatches = mq.GetMessagesByLocation(model.Reporter.ContactType, model.Coordinates, SEARCH_RADIUS_IN_MILES);

                    if (possibleMatches.Count > 0)
                    {
                        //TODO: after application is in MVP state I plan on redoing this part so that rather than using imgURLs for matching requests the possible matches will actually return a person ID for better accuracy. 
                        //Will have to redo CompareImagesFromURI method and all called methods within to handle a personID instead of FaceID

                        //Compare image supplied by reporter against all images returned from possible matches and return index of first match with expected confidence level.
                        int imageMatchIndex = imageService.CompareImagesFromURI(model.DependentImgURL, possibleMatches.Select(queueResults => queueResults.DependentImgURL).ToArray());
                        if (imageMatchIndex >= 0)
                        {
                            //if match found, use the index of the matched report to retrieve the entire report object
                            matchedReport = possibleMatches[imageMatchIndex];
                            mobileHelper.SendMatchNotification(model, matchedReport, canSendReporterLocation);
                            //TODO: rather than have an "OfferToCallReporter" method instead change to "CallReporter" method and move method call into another controller and/or action from with which the user is redirected in order to offer to connect them to the other party
                            mobileHelper.OfferToCallReporter(matchedReport.ReporterPhoneNumber);
                            mq.RemoveMessageFromStorage(matchedReport);
                            return matchedReport;
                        }
                    }

                    //if a match is not found add the message to the message queue and notify user that the report has been logged and that they will be notified when a match is found
                    mq.AddMessageToStorage(new ReportMissingMsgAdaptor(model));
                }

                catch(Exception ex)
                {
                    //TODO: create logger to log exception messages and redirect to user-friendly error page.
                    return matchedReport;
                }
            }
            return matchedReport;
        }
    }
}