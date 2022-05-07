using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SpecialResearch.Data;
using SpecialResearch.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpecialResearch.Controllers
{
    public class AuthorizationController : Controller
    {
        private readonly SpecialResearchContext _context;
      
        public AuthorizationController(SpecialResearchContext context)
        {
            _context = context;
            
        }
        
        public IActionResult Index()
        {
            return View();
        }
            
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult>  Index(string login,string password)
        {
            //AQAAAAEAACcQAAAAEKAhzVvGDVM3CmYt1a/sElNYA3PhGxOsdsMsqtjdQ+5vfINsGKD5NAkfp4BcmhJKvA==
            //AQAAAAEAACcQAAAAEAEmc0ICNnJCFWz1RTCr3mfPgybwsJ2f0/LFytaBgSxvXd06fh5PAcripI2ohg7OUQ==
            //AQAAAAEAACcQAAAAEDPnSCbywWsrjxLuj/RGHG4tH6ajkuLkdnbZ37eB2bLkRd1UAJDjpk3LVywFaFE5Nw==
            string HashPass = new PasswordHasher<User>().HashPassword(null, password);
            var UserLogin = _context.User.Where(u => u.Login == login && u.Password == HashPass).SingleOrDefault();
            if (UserLogin !=null)
            {
                //Залогинились
                HttpContext.Session.SetInt32("CurrentUserId", UserLogin.Id);
                HttpContext.Session.SetInt32("CurrentUserRole", UserLogin.RoleId);
                HttpContext.Session.SetString("CurrentUserName", UserLogin.Name);
                return RedirectToAction("Request");
            }
            ViewBag.Err = "Неверный логин или пароль!";
            return View();
        }
    }
}
