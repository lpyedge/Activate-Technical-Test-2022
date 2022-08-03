using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Ocsp;

namespace Bakery.Site.Controllers
{
    public class HomeController : Controller
    {

        [HttpGet("/")]
        public IActionResult Index()
        {
            ViewBag.ProductList = BLL.ProductBLL.QueryListByStoreShow();

            if (!string.IsNullOrWhiteSpace(Request.Query["template"]))
            {
                switch (Request.Query["template"])
                {
                    case "1":
                        return View("T1/Index");
                    case "2":
                        return View("T2/Index");
                    default:
                        return View("T1/Index");
                }
            }
            else
            {
                if (SiteContext.Config.SiteTemplate == ESiteTemplate.Template1)
                {
                    return View("T1/Index");
                }
                else if (SiteContext.Config.SiteTemplate == ESiteTemplate.Template2)
                {
                    return View("T2/Index");
                }
                else
                {
                    return View("T1/Index");
                }
            }
        }
    }
}
