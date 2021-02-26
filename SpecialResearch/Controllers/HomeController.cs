using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Session;
using Microsoft.Extensions.Logging;
using SpecialResearch.Data;
using SpecialResearch.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace SpecialResearch.Controllers
{
    //контроллер использовалася на ранних этапах. сейчас не используется
    public class HomeController : Controller
    {
      //  private readonly ILogger<HomeController> _logger;
        private readonly SpecialResearchContext _context;

     
        //public HomeController(ILogger<HomeController> logger)
        public HomeController(SpecialResearchContext context)
        {
           // _logger = logger;
            _context = context;

        }

        public IActionResult Index()
        {
            
            return RedirectToAction("index","Requests" );
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
