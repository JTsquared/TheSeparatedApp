using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using BusinessLayer.Models;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Net;

namespace LostChildApp.Controllers
{
    public class ReportMissingController : Controller
    {
        private readonly IHostingEnvironment hostingEnvironment;
        private readonly IMessageRepository mq;
        private readonly IImageService imageService;
        private readonly IVapidSettings vapidSettings;
        private readonly IPushNotificationService pushNotificationService;
        private readonly SmtpSettings smtpSettings;

        private const double SEARCH_RADIUS_IN_MILES = 5.0d;

        public ReportMissingController(IHostingEnvironment hostingEnvironment, IMessageRepository mq, IImageService imageService, IOptions<VapidSettings> vapidSettings, IPushSubscription pushSubscription, IOptions<SmtpSettings> smtpSettings)//, IPushNotificationService pushNotificationService)
        {
            this.hostingEnvironment = hostingEnvironment;
            this.mq = mq;
            this.imageService = imageService;
            //this.mobileHelper = mobileHelper;
            this.vapidSettings = vapidSettings.Value;
            this.pushNotificationService = new PushNotificationService(this.vapidSettings, pushSubscription);
            this.smtpSettings = smtpSettings.Value;
        }

        public IActionResult Index()
        {
            //ReportMissingMsg missingReport = new ReportMissingMsg();
            ViewBag.VapidPublicKey = vapidSettings.PublicKey;

            //return View(missingReport);
            return View();
        }

        //public IActionResult Index(ReportMissingMsg model)
        //{
        //    if (model == null)
        //    {
        //        ReportMissingMsg msg = new ReportMissingMsg();
        //        model = msg;
        //    }
        //    return View(model);
        //}

        [HttpPost]
        public IActionResult FamilyReport(ReportMissingMsg model, IFormFile imageFile2, string useMobileLocation)
        {
            ////BEGIN TEST DATA------------------------------------------------------------------

            //string smtpServer = smtpSettings.SMTPServer;
            //string emailUsername = smtpSettings.SMTPEmailAddress;
            //string emailPassword = smtpSettings.SMTPEmailPassword;
            //string jwtSecret = smtpSettings.JWTSecret;
            //INotificationService notificationService = new EmailNotificationService(smtpServer, emailUsername, emailPassword, "turner.e.jonathan@gmail.com", emailUsername, jwtSecret);
            //string returnURL = $"{this.Request.Scheme}://{this.Request.Host}/ReportMatch/Confirm";


            //JWTTokenService temp = new JWTTokenService(smtpSettings.JWTSecret);
            //var temp2 = JsonConvert.SerializeObject(model);
            //var temp3 = temp.GetSecurityToken(temp2);
            //return Redirect(returnURL + "?jwt=" + temp3);
            ////return Redirect(returnURL + "/" + temp3);


            //notificationService.SendReportMatchNotification(new Notification("testSubject", "testMessage", model, returnURL));

            ////END TEST DATA--------------------------------------------------------------------

            model.Reporter.ContactType = ContactType.Family;

            //the reporter's mobile location is required but if the below checkbox on the post form is checked they they have given permission to send their location to the other party
            model.ShareLocation = useMobileLocation == "on" ? true : false;
            model.DependentImgURL = imageService.SaveImageToStorage(imageFile2);

            ReportMissingMsgAdaptor matchedReport = GetMissingReportMatch(model);

            if (matchedReport != null)
            {
                bool matchNotificationSent = SendMatchNotification(model, matchedReport);
                if (matchNotificationSent)
                {
                    ViewBag.SentNotificationStatus = "Success";
                }
                else
                {
                    ViewBag.SentNotificationStatus = "Failure";
                }
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
            model.Reporter.ContactType = ContactType.NonFamily;

            //the reporter's mobile location is required but if the below checkbox on the post form is checked they they have given permission to send their location to the other party
            model.ShareLocation = useMobileLocation == "on" ? true : false;

            model.DependentImgURL = imageService.SaveImageToStorage(imageFile);

            ReportMissingMsgAdaptor matchedReport = GetMissingReportMatch(model);

            if (matchedReport != null)
            {
                bool matchNotificationSent = SendMatchNotification(model, matchedReport);
                if (matchNotificationSent)
                {
                    ViewBag.SentNotificationStatus = "Success";
                }
                else
                {
                    ViewBag.SentNotificationStatus = "Failure";
                }
                return View("ReportMatch", matchedReport);
            }
            else
            {
                return View("ReportAdded");
            }
        }

        public IActionResult GetImageFromURI(string uri)
        {
            var img = imageService.GetImageFromStorage(uri);
            return File(img, "image/png");
        }

        private ReportMissingMsgAdaptor GetMissingReportMatch(ReportMissingMsg model)
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

                            //********************************************************************************************
                            //TODO: rather than removing the image and report immediately move this call to action within ReportMatch controller after user has confirmed match is correct.
                            //imageService.RemoveImagesFromStorage(model.DependentImgURL, matchedReport.DependentImgURL);
                            //mq.RemoveMessageFromStorage(matchedReport);
                            //********************************************************************************************

                            return matchedReport;
                        }
                    }

                    //if a match is not found add the message to the message queue and notify user that the report has been logged and that they will be notified when a match is found
                    mq.AddMessageToStorage(new ReportMissingMsgAdaptor(model));
                }

                catch (Exception ex)
                {
                    //TODO: create logger to log exception messages and redirect to user-friendly error page.
                    return matchedReport;
                }
            }
            return matchedReport;
        }

        private bool SendMatchNotification(ReportMissingMsg model, ReportMissingMsgAdaptor matchedReport)
        {
            INotificationService notificationService;
            bool wasNotificationSent = false;

            try
            {
                //TODO: after MVP consider updating reportMissing UI to give user options to select which type of notification service they prefer
                //configure push notification service
                if (matchedReport.NotificationTypePreference == NotificationTypePreference.PushNotification.ToString())
                {
                    pushNotificationService.SetVapidSettings(vapidSettings);
                    pushNotificationService.SetPushSubscription(matchedReport.ReporterEndpoint, matchedReport.ReporterKey, matchedReport.ReporterAuthSecret);
                    notificationService = new NotificationService(pushNotificationService);
                }
                else
                {
                    string smtpServer = smtpSettings.SMTPServer;
                    string emailUsername = smtpSettings.SMTPEmailAddress;
                    string emailPassword = smtpSettings.SMTPEmailPassword;
                    string jwtSecret = smtpSettings.JWTSecret;
                    notificationService = new EmailNotificationService(smtpServer, emailUsername, emailPassword, matchedReport.ReporterEmailAddress, emailUsername, jwtSecret);
                }

                //TODO: INotificationService preferredNotificationService = NotificationServiceFactory.Create(model.PreferredNotificationServiceType);
                //preferredNotificationService.SendReportMatchNotification(x => new { "test, model.DependentImgURL" });

                //register push notification service by default
                //notificationService = new NotificationService(pushNotificationService);

                //register additional notification serivce types in case push notification service fails.
                //TODO: create additional notification service types below and have the view manage when to collect info to create these types (get permission for text and collect email address)
                //notificationService.AddNotificationService(textNotificationService);
                //notificationService.AddNotificationService(emailNotificationService);

                //notificationService iterates through each registered service type to attempt to send notification until the notification is successfully sent 
                wasNotificationSent = notificationService.SendReportMatchNotification(new Notification("We found a match!", "Please click the link to view the report and confirm the match", model, $"{this.Request.Scheme}://{this.Request.Host}/ReportMatch/Confirm?jwt="));
            }
            catch(Exception e)
            {
                //TODO: create logger to log push notification failure
            }

            return wasNotificationSent;
        }

        //[Route("")]
        //public IActionResult ReportMatch(string notificationTag)
        //{
        //    ReportMissingMsg model = null;
        //    //var decodedModel = WebUtility.UrlDecode(notificationTag);
        //    //model = JsonConvert.DeserializeObject<ReportMissingMsg>(decodedModel);
        //    model = JsonConvert.DeserializeObject<ReportMissingMsg>(notificationTag);
        //    return View("ReportMatch", new ReportMissingMsgAdaptor(model));
        //}

        public IActionResult ReportMatch(string notificationTag)
        {
            ReportMissingMsg model = null;
            //var decodedModel = WebUtility.UrlDecode(notificationTag);
            //model = JsonConvert.DeserializeObject<ReportMissingMsg>(decodedModel);
            model = JsonConvert.DeserializeObject<ReportMissingMsg>(notificationTag);
            return View("ReportMatch", new ReportMissingMsgAdaptor(model));
        }

        [HttpPost]
        public IActionResult ReportMatch()
        {
            return View("ReportMatch");
        }


        //[HttpPost]
        //public IActionResult RegisterUserForPushNotifications(ReportMissingMsg msg, string key, string endpoint, string authSecret)
        //{
        //    //ReportMissingMsg msg = new ReportMissingMsg();
        //    msg.PushNotificationKey = key;
        //    msg.PushNotificationEndpoint = endpoint;
        //    msg.PushNotificationAuthSecret = authSecret;
        //    //return View(msg);

        //    TryUpdateModelAsync(msg);
        //    return RedirectToAction("Index", msg);

        //    //return RedirectToAction("Index", msg);
        //}
    }
}