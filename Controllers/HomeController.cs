using Kurs.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Kurs.Controllers
{
   
    public class HomeController : Controller
    {
        [Authorize(Roles = "Super_Admin")]
        public ActionResult Index()
        {
            return View();
        }



        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}