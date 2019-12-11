using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using LostChildApp.Models;
using Microsoft.Extensions.Configuration;

namespace LostChildApp.Controllers
{
    public class HomeController : Controller
    {
        IConfiguration _iconfiguration;

        public HomeController(IConfiguration configuration)
        {
            _iconfiguration = configuration;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Title"] = "Contact The Separated App Team";
            ViewData["AddressLn1"] = _iconfiguration["AddressLn1"];
            ViewData["AddressLn2"] = _iconfiguration["AddressLn2"];
            ViewData["SupportPhoneNumber"] = _iconfiguration["SupportPhoneNumber"];
            ViewData["SupportEmailAddress"] = _iconfiguration["SupportEmailAddress"];

            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
