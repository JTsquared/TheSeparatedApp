using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BusinessLayer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace LostChildApp.Controllers
{
    public class ReportMatchController : Controller
    {
        private readonly IVapidSettings _vapidSettings;
        private readonly SmtpSettings _smtpSettings;
        private readonly JWTTokenService _jwtTokenService;
        private readonly IImageService _imageService;
        public ReportMatchController(IOptions<SmtpSettings> smtpSettings, IOptions<VapidSettings> vapidSettings, IImageService imageService)
        {
            _vapidSettings = vapidSettings.Value;
            _smtpSettings = smtpSettings.Value;
            _jwtTokenService = new JWTTokenService(_smtpSettings.JWTSecret);
            _imageService = imageService;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [AutoValidateAntiforgeryToken]
        public IActionResult Confirm(string jwt)
        {
            var jsonObj = _jwtTokenService.ReadJWTToken(jwt);

            ReportMissingMsg model = null;
            model = JsonConvert.DeserializeObject<ReportMissingMsg>(jsonObj);
            return View("Confirm", new ReportMissingMsgAdaptor(model));
        }

        [HttpPost]
        public IActionResult Confirm()
        {
            return View("Confirm");
        }

        public IActionResult GetImageFromURI(string uri)
        {
            var img = _imageService.GetImageFromStorage(uri);
            return File(img, "image/png");
        }
    }
}