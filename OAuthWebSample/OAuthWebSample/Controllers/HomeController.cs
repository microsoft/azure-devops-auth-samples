using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OAuthSample.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.ClientAppId = System.Configuration.ConfigurationManager.AppSettings["ClientAppId"];
            ViewBag.CallbackUrl = System.Configuration.ConfigurationManager.AppSettings["CallbackUrl"];
            ViewBag.Scope = System.Configuration.ConfigurationManager.AppSettings["Scope"];

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
    }
}