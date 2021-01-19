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


namespace SpecialResearch.Controllers
{
    public class RequestsController : Controller
    {
        private readonly SpecialResearchContext _context;

        public RequestsController(SpecialResearchContext context)
        {
            _context = context;
        }

        // GET: Requests
        public async Task<IActionResult> Index()
        {
            var specialResearchContext = _context.Request.Include(r => r.Stage).Include(r => r.User).Include(r => r.User1);
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

        // GET: Requests/Create
        public IActionResult Create()
        {
            ViewData["StageID"] = new SelectList(_context.Stage, "Id", "StageName");
            ViewData["UserId"] = new SelectList(_context.User, "Id", "Login");
            ViewData["User1Id"] = new SelectList(_context.User, "Id", "Login");
            Request request = new Request();
            //Ставим данные по умолчанию для создания заявки
            request.CreateDate = DateTime.Now;//Сейчас
            request.User1Id = 1;//Никто не выдвал предписания
            request.UserId = (int)HttpContext.Session.GetInt32("CurrentUserId");//Залогинившийся юзер - создатель
            
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
            request.UserId = 2;
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
