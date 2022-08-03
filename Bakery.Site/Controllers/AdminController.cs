using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Bakery.Site.Controllers
{
    [Route("{controller}/{action}")]
    public class AdminController : Controller
    {
        [Route("/admin")]
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Login()
        {
            return View();
        }
        
        public IActionResult Product_List()
        {
            return View();
        }
    }
}
