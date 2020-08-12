using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IntroMVCSite.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult CRUDOperations()
        {
            ViewBag.Message = "This message is being passed via the \"ViewBag.Message\" property in ~/Controllers/HomeController.CRUDOpertaions";
            ViewBag.Title = "CRUD Operations";

            return View();
        }
    }
}