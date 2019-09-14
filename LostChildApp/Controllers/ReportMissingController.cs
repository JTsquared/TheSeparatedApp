using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using LostFamily.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Web;

namespace LostChildApp.Controllers
{
    public class ReportMissingController : Controller
    {
        private readonly IHostingEnvironment hostingEnvironment;

        public ReportMissingController(IHostingEnvironment hostingEnvironment)
        {
            this.hostingEnvironment = hostingEnvironment;
        }

        public IActionResult Index()
        {
            ReportMissingMsg missingReport = new ReportMissingMsg();
            return View(missingReport);
        }

        [HttpPost]
        public IActionResult SendMissingMsg(ReportMissingMsg model)
        {
            if (ModelState.IsValid)
            {
                //TODO:
                //not sure if this code is needed. borrowed it from tutorial on saving images to database but the instructor had a separate viewmodel as the input for this IActionResult which he then used to populate a different model before
                //saving to a database. Since I am using the model directly and not using a viewmodel as an intermediate step I might be able to skip this part and just use client-side and server-side validation to ensure data is there before
                //sending to the queue.
                string fileName = null;
                if (model.DependentImage != null)
                {
                    string uploadFolder = Path.Combine(hostingEnvironment.WebRootPath, "uploadFolder");
                    fileName = Guid.NewGuid().ToString() + "_" + model.DependentImage.FileName;
                    string filePath = Path.Combine(uploadFolder, fileName);
                    model.DependentImage.CopyTo(new FileStream(filePath, FileMode.Create));
                }

                //TODO:
                //here is where I would add call to queue storage and submit ReportMissingMsg
            }

            return View();
        }

        [HttpPost]
        public IActionResult FamilyReport(ReportMissingMsg model, IFormFile imageFile)
        {
            model.DependentImage = imageFile;
            model.DependentImgURL = imageFile.FileName;
            model.Reporter.ContactType = ContactType.Family;
            return View();
        }

        [HttpPost]
        public IActionResult NonFamilyReport(ReportMissingMsg model, IFormFile imageFile, string useMobileLocation)
        {
            model.DependentImage = imageFile;
            model.DependentImgURL = imageFile.FileName;
            model.Reporter.ContactType = ContactType.NonFamily;
            if (useMobileLocation == "on")
            {
                model.Location = GetMobileLocation();
            }
            return View();
        }

        public void UploadImage()
        {
            if (HttpContext.Request.Form.Files[0] != null)
            {
                var file = HttpContext.Request.Form.Files[0];
                string uploadFolder = Path.Combine(hostingEnvironment.WebRootPath, "uploadFolder");
                var fileName = file.FileName;
                string filePath = Path.Combine(uploadFolder, fileName);
                file.CopyTo(new FileStream(filePath, FileMode.Create));
            }
        }

        private Location GetMobileLocation()
        {
            //TODO: return mobile location of mobile GPS coords;
            return new Location();
        }

        private void SendMissingReport(ReportMissingMsg model)
        {
            if (model != null)
            {

                //search for existing reports in msg queue based on user proximity 
                List<ReportMissingMsg> possibleMatches = PollMsgQueue(model.Reporter.ContactType, model.Location);

                if (possibleMatches.Count > 0)
                {
                    int imageMatchIndex = CompareDependentImages(model.DependentImage, possibleMatches.Select(queueResults => queueResults.DependentImage));
                    if (imageMatchIndex >= 0)
                    {
                        ReportMissingMsg matchedReport = possibleMatches[imageMatchIndex];
                        bool canSendReporterLocation = RequestToProvideLocation();
                        SendMatchNotification(model, matchedReport, canSendReporterLocation);
                        //OfferToCallReporter(matchedReport.Reporter.PhoneNumber);
                        //RemoveQueueMsg();
                        return;
                    }
                }

                //if a match is not found add the message to the message queue and notify user that the report has been logged and that they will be notified when a match is found
                //AddReportToQueue();

            }
        }

        private List<ReportMissingMsg> PollMsgQueue(ContactType contactType, Location location)
        {
            List<ReportMissingMsg> queueResults = new List<ReportMissingMsg>();
            ContactType searchContactType = contactType == ContactType.NonFamily ? ContactType.Family : ContactType.NonFamily;
            //queueResults = MissingReportRepository.GetQueueMessages(searchContactType, location);
            return queueResults;
        }

        private int CompareDependentImages(IFormFile dependentImage, IEnumerable<IFormFile> queuedImages)
        {
            //TODO: Send images to facial recognition service to determine if there is a match
            int matchedImageIndex = -1;
            bool isMatchFound = false;

            for (int i = 0; i < queuedImages.Count(); i++)
            {
                //bool isMatchFound = FacialRecognitionService.CompareImages(dependentImage, queuedImages);
                if (isMatchFound)
                {
                    matchedImageIndex = i;
                    break;
                }
            }

            return matchedImageIndex;
        }

        private bool RequestToProvideLocation()
        {
            //TODO: request permission from user to be able to send their location to the family so that they can locate them.
            return false;
        }

        private void SendMatchNotification(ReportMissingMsg model, ReportMissingMsg matchedReport, bool canSendReporterLocation)
        {
            if(model.Reporter.ContactType == ContactType.NonFamily)
            {
                //TODO: use builder class to build notification message
                string messageBody = notificationBuilder.BuildMessage(model, canSendReporterLocation);
                bool notificationSent = SendNotification(messageBody, matchedReport.Reporter.PhoneNumber);
            }
        }
    }
}