using SmartStore.Web.Models.ReflectedXSS;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.DirectoryServices;
using System.DirectoryServices.Protocols;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace SmartStore.Web.Controllers
{
    public class ReflectedXSSController : Controller
    {
        // GET: ReflectedXSS
        public ActionResult Index()
        {
            return View();
        }


        [HttpPost]
        public ActionResult Index(DummyMessage message)
        {
            ViewBag.ErrorMessage = message.Description;
            return View("Error");
        }
    }
}