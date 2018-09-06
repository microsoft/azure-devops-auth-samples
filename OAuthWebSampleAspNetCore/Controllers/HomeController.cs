using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using OAuthWebSampleAspNetCore.Models;

namespace OAuthWebSampleAspNetCore.Controllers
{
    public class HomeController : Controller
    {
        private IConfiguration Configuration;

        public HomeController(IConfiguration configuration)
        {
            this.Configuration = configuration;   
        }
        
        public IActionResult Index()
        {
            OAuthSettings settings = new OAuthSettings();
            this.Configuration.GetSection("oauth").Bind(settings);
            ViewData["settings"] = settings;

            return View(settings);
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
