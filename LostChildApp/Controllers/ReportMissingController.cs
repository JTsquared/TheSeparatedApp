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
            return View(missingReport);
        }

        //[HttpPost]
        //public IActionResult SendMissingMsg(ReportMissingMsg model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        //TODO:
        //        //not sure if this code is needed. borrowed it from tutorial on saving images to database but the instructor had a separate viewmodel as the input for this IActionResult which he then used to populate a different model before
        //        //saving to a database. Since I am using the model directly and not using a viewmodel as an intermediate step I might be able to skip this part and just use client-side and server-side validation to ensure data is there before
        //        //sending to the queue.
        //        string fileName = null;
        //        if (model.DependentImage != null)
        //        {
        //            string uploadFolder = Path.Combine(hostingEnvironment.WebRootPath, "uploadFolder");
        //            fileName = Guid.NewGuid().ToString() + "_" + model.DependentImage.FileName;
        //            string filePath = Path.Combine(uploadFolder, fileName);
        //            model.DependentImage.CopyTo(new FileStream(filePath, FileMode.Create));
        //        }

        //        //TODO:
        //        //here is where I would add call to queue storage and submit ReportMissingMsg
        //    }

        //    return View();
        //}

        [HttpPost]
        public IActionResult FamilyReport(ReportMissingMsg model, IFormFile imageFile2, string useMobileLocation)
        {
            model.Reporter.ContactType = ContactType.Family;
            //var imageUri = imageService.SaveImageToStorageAsync(imageFile2);
            //model.DependentImgURL = imageUri.Result;
            model.DependentImgURL = imageService.SaveImageToStorage(imageFile2);
            if (useMobileLocation == "on")
            {
                model.Location = GetMobileLocation();
            }

            SendMissingReport(model);

            return View();
        }

        [HttpPost]
        public IActionResult NonFamilyReport(ReportMissingMsg model, IFormFile imageFile, string useMobileLocation)
        {
            //model.DependentImgURL = imageFile.FileName;
            model.Reporter.ContactType = ContactType.NonFamily;
            //var imageUri = imageService.SaveImageToStorageAsync(imageFile);
            //model.DependentImgURL = imageUri.Result;
            model.DependentImgURL = imageService.SaveImageToStorage(imageFile);
            if (useMobileLocation == "on")
            {
                model.Location = GetMobileLocation();
            }

            SendMissingReport(model);

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
            Coordinates coordinates = new Coordinates(7, 8);
            Location location = new Location();
            location.Coordinates = coordinates;
            return location;
        }

        private void SendMissingReport(ReportMissingMsg model)
        {
            if (model != null)
            {
                //search for existing reports in msg queue based on user proximity
                //TODO: move magic number for searchRadius into a constant
                List<ReportMissingMsgAdaptor> possibleMatches = mq.GetMessagesByLocation(model.Reporter.ContactType, model.Location, 5);

                if (possibleMatches.Count > 0)
                {
                    int imageMatchIndex = imageService.CompareImagesFromURI(model.DependentImgURL, possibleMatches.Select(queueResults => queueResults.DependentImgURL).ToArray());
                    if (imageMatchIndex >= 0)
                    {
                        ReportMissingMsgAdaptor matchedReport = possibleMatches[imageMatchIndex];
                        bool canSendReporterLocation = mobileHelper.RequestToProvideLocation();
                        mobileHelper.SendMatchNotification(model, matchedReport, canSendReporterLocation);
                        //TODO: rather than have an "OfferToCallReporter" method instead change to "CallReporter" method and move method call into another controller from with which the user is redirected in order to offer to connect them to the other part
                        mobileHelper.OfferToCallReporter(matchedReport.ReporterPhoneNumber);
                        mq.RemoveMessageFromStorage(new ReportMissingMsgAdaptor(model));
                        return;
                    }
                }

                //if a match is not found add the message to the message queue and notify user that the report has been logged and that they will be notified when a match is found
                mq.AddMessageToStorage(new ReportMissingMsgAdaptor(model));
                
            }
        }
    }
}