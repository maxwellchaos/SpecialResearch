using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SpecialResearch.Data;
using SpecialResearch.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Session;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authorization;

namespace SpecialResearch.Controllers
{
    [Authorize]
    public class RequestsController : Controller
    {
        private readonly SpecialResearchContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public RequestsController(SpecialResearchContext context, IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
            _context = context;
        }

        // GET: Requests
        public async Task<IActionResult> Index()
        {
            var specialResearchContext = _context.Request.Include(r => r.Stage).
                Include(r => r.User).Include(r => r.User1).
                OrderByDescending(r=>r.CreateDate);

            if(User.IsInRole(Startup.RecieverRole))
            {
                return View(specialResearchContext.Where(r => r.StageID == 1));
            }

            if (User.IsInRole(Startup.TesterRole))
            {
                return View(specialResearchContext.Where(r => r.StageID == 2 || r.StageID == 1));
            }
            if (User.IsInRole(Startup.ControllerRole))
            {
                return View(specialResearchContext.Where(r => r.StageID == 3 || r.StageID == 2));
            }

            return View(await specialResearchContext.ToListAsync());
        }

        // GET: Requests/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var request = await _context.Request
                .Include(r => r.Stage)
                .Include(r => r.User)
                .Include(r => r.User1)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (request == null)
            {
                return NotFound();
            }
            

            return View(request);
        }


      
       
       
        public async Task<IActionResult> AddFile(int? id,IFormFile UploadedFile)
        {
            if (id != null && UploadedFile != null)
            {

                string Path = "/files/" + Guid.NewGuid().ToString()+UploadedFile.FileName;
                using (var fileStream = new FileStream(_webHostEnvironment.WebRootPath+Path, FileMode.Create)) 
                {
                    await UploadedFile.CopyToAsync(fileStream);
                }
                Request request = _context.Request.Where(r => r.Id == id).FirstOrDefault();
                request.PhotoCopy = Path;
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View();
        }




        // GET: Requests/Create
        public IActionResult Create()
        {
            //ViewData["StageID"] = new SelectList(_context.Stage, "Id", "StageName");
            //ViewData["UserId"] = new SelectList(_context.User, "Id", "Login");
            //ViewData["User1Id"] = new SelectList(_context.User, "Id", "Login");
            Request request = new Request();
            //Ставим данные по умолчанию для создания заявки
            request.CreateDate = DateTime.Now;//Сейчас
            request.UserId = (int)HttpContext.Session.GetInt32("CurrentUserId");//Залогинившийся юзер - создатель
           ViewBag.UserName = _context.User.Where(u => u.Id == request.UserId).FirstOrDefault().Name;
            return View(request);
        }

        // POST: Requests/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Number,CreateDate,StageID,UserId,User1Id,UseOrder,EndDate,PhotoCopy")] Request request)
        {
            request.User1Id = 1;
            request.StageID = 1;
            //request.UserId = 2;
            if (ModelState.IsValid)
            {
                _context.Add(request);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["StageID"] = new SelectList(_context.Stage, "Id", "StageName", request.StageID);
            ViewData["UserId"] = new SelectList(_context.User, "Id", "Login", request.UserId);
            ViewData["User1Id"] = new SelectList(_context.User, "Id", "Login", request.User1Id);
            return View(request);
        }

        // GET: Requests/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var request = await _context.Request.FindAsync(id);
            if (request == null)
            {
                return NotFound();
            }
            ViewData["StageID"] = new SelectList(_context.Stage, "Id", "StageName", request.StageID);
            ViewData["UserId"] = new SelectList(_context.User, "Id", "Login", request.UserId);
            ViewData["User1Id"] = new SelectList(_context.User, "Id", "Login", request.User1Id);
            
            return View(request);
        }

        //CloseRquest
        public async Task<IActionResult> CloseRquest(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var request = await _context.Request.FindAsync(id);

            if (request == null)
            {
                return NotFound();
            }
            request.StageID = 4;
            request.EndDate = DateTime.Now;

            try
            {
                _context.Update(request);
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                return RedirectToAction(nameof(Index));
            }
            return RedirectToAction(nameof(Index));
        }
        //GetUse
        public async Task<IActionResult> GetUse(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var request = await _context.Request.FindAsync(id);

            if (request == null)
            {
                return NotFound();
            }
            request.StageID = 3;
            request.UseOrder = 1;
            request.User1Id = (int)HttpContext.Session.GetInt32("CurrentUserId");//Залогинившийся юзер - Выдал предписание

            try
            {
                _context.Update(request);
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                return RedirectToAction(nameof(Index));
            }
            return RedirectToAction(nameof(Index));
        }
        // GET: Requests/Edit/5
        public async Task<IActionResult> NextStage(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var request = await _context.Request.FindAsync(id);
            
            if (request == null)
            {
                return NotFound();
            }
            request.StageID++;
            
            if(request.StageID == 4)
            {
                request.EndDate = DateTime.Now;
            }
            try
            {
                _context.Update(request);
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                return RedirectToAction(nameof(Index));
            }
            return RedirectToAction(nameof(Index));
        }

        // POST: Requests/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Number,CreateDate,StageID,UserId,User1Id,UseOrder,EndDate,PhotoCopy")] Request request)
        {
            if (id != request.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(request);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RequestExists(request.Id))
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
            ViewData["StageID"] = new SelectList(_context.Stage, "Id", "StageName", request.StageID);
            ViewData["UserId"] = new SelectList(_context.User, "Id", "Login", request.UserId);
            ViewData["User1Id"] = new SelectList(_context.User, "Id", "Login", request.User1Id);
            return View(request);
        }

        // GET: Requests/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var request = await _context.Request
                .Include(r => r.Stage)
                .Include(r => r.User)
                .Include(r => r.User1)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (request == null)
            {
                return NotFound();
            }

            return View(request);
        }

        // POST: Requests/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var request = await _context.Request.FindAsync(id);
            _context.Request.Remove(request);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RequestExists(int id)
        {
            return _context.Request.Any(e => e.Id == id);
        }
    }
}
