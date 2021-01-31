using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SpecialResearch.Data;
using SpecialResearch.Models;

namespace SpecialResearch.Controllers
{
    [Authorize(Roles ="admin")]
    public class UsersController : Controller
    {
        private readonly SpecialResearchContext _context;

        public UsersController(SpecialResearchContext context)
        {
            _context = context;
        }


        // GET: Users
        [AllowAnonymous]
        public async Task<IActionResult> LogOff()
        {
            await HttpContext.SignOutAsync("Cookie");
            HttpContext.Session.SetInt32("CurrentUserId", 0);
            return Redirect("/");

        }

        // GET: Users
        [AllowAnonymous]
        public IActionResult AccessDenied()
        {

            return View();

        }

        [AllowAnonymous]
        // GET: Users
        public  IActionResult Login(string ReturnUrl)
        {
            //var specialResearchContext = _context.User.Include(u => u.Role);
            return View();
     
        }
        [HttpPost]
        [AllowAnonymous]
        // GET: Users
        public async Task<IActionResult> Login( User model)
        {
            if(!ModelState.IsValid)
            {
                ViewBag.Err = "Неверный логин или пароль";
                return View(model);
            }
            string hash = SpecialResearchContext.GetHashString(model.Password);
            User UserLogin = await _context.User.Include(u=>u.Role)
                .Where(u => u.Login == model.Login && u.Password == hash).SingleOrDefaultAsync();
            if (UserLogin == null )
            {
                ViewBag.Err = "Неверный логин или пароль";
                return View(model);
            }
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name,UserLogin.Name),
                new Claim(ClaimTypes.Role,UserLogin.Role.Name),
            };
            var claimeIdentity = new ClaimsIdentity(claims, "Cookie");
            var claimePrincipal = new ClaimsPrincipal(claimeIdentity);
            await HttpContext.SignInAsync("Cookie", claimePrincipal);

            HttpContext.Session.SetInt32("CurrentUserId", UserLogin.Id);


            return Redirect(model.ReturnUrl);
        }

      

        // GET: Users
        public async Task<IActionResult> Index()
        {
            var specialResearchContext = _context.User.Include(u => u.Role);
            return View(await specialResearchContext.ToListAsync());
        }

        // GET: Users/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.User
                .Include(u => u.Role)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // GET: Users/Create
        public IActionResult Create()
        {
            ViewData["RoleId"] = new SelectList(_context.Role, "Id", "Name");
            return View();
        }

        // POST: Users/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Login,RoleId,Name,Password")] User user)
        {
            if (ModelState.IsValid)
            {
                user.Password = SpecialResearchContext.GetHashString(user.Password);
                _context.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["RoleId"] = new SelectList(_context.Role, "Id", "Name", user.RoleId);
            return View(user);
        }

        // GET: Users/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.User.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            user.Password = "";
            ViewData["RoleId"] = new SelectList(_context.Role, "Id", "Name", user.RoleId);
            return View(user);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Login,RoleId,Name,Password")] User user)
        {
            if (id != user.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    user.Password = SpecialResearchContext.GetHashString(user.Password);
                    _context.Update(user);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["RoleId"] = new SelectList(_context.Role, "Id", "Name", user.RoleId);
            return View(user);
        }

        // GET: Users/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.User
                .Include(u => u.Role)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await _context.User.FindAsync(id);
            _context.User.Remove(user);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserExists(int id)
        {
            return _context.User.Any(e => e.Id == id);
        }
    }
}
