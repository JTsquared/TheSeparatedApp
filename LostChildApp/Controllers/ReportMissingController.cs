﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace LostChildApp.Controllers
{
    public class ReportMissingController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult UploadPicture()
        {
            return View();
        }
    }
}