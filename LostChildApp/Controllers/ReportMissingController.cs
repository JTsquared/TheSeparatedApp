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
        private readonly IMessageQueue mq;
        private readonly IImageService imageService;
        private readonly IMobileHelper mobileHelper;

        public ReportMissingController(IHostingEnvironment hostingEnvironment, IMessageQueue mq, IImageService imageService, IMobileHelper mobileHelper)
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
                List<ReportMissingMsg> possibleMatches = mq.PollMsgQueue(model.Reporter.ContactType, model.Location);

                if (possibleMatches.Count > 0)
                {
                    int imageMatchIndex = imageService.CompareDependentImages(model.DependentImage, possibleMatches.Select(queueResults => queueResults.DependentImage));
                    if (imageMatchIndex >= 0)
                    {
                        ReportMissingMsg matchedReport = possibleMatches[imageMatchIndex];
                        bool canSendReporterLocation = mobileHelper.RequestToProvideLocation();
                        mobileHelper.SendMatchNotification(model, matchedReport, canSendReporterLocation);
                        mobileHelper.OfferToCallReporter(matchedReport.Reporter.PhoneNumber);
                        mq.RemoveQueueMsg(model, matchedReport);
                        return;
                    }
                }

                //if a match is not found add the message to the message queue and notify user that the report has been logged and that they will be notified when a match is found
                //AddReportToQueue();
            }
        }
    }
}