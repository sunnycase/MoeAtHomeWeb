﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MoeAtHome.Repositories;
using Microsoft.WindowsAzure.Storage.Table;

namespace MoeAtHome.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult _ViewBlog()
        {
            return PartialView();
        }

        public ActionResult _HomePage()
        {
            return PartialView();
        }

        public ActionResult _Login()
        {
            return PartialView();
        }
    }
}