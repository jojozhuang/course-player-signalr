﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CoursePlayer.SignalR.Controllers
{
    public class DummyPlayerController : Controller
    {
        // GET: DummyPlayer
        public ActionResult Index()
        {
            return View();
        }
    }
}