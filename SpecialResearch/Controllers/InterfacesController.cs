using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SpecialResearch.Data;
using SpecialResearch.Models;

namespace SpecialResearch.Controllers
{
    public class InterfacesController : Controller
    {
        private readonly SpecialResearchContext _context;

        public InterfacesController(SpecialResearchContext context)
        {
            _context = context;
        }

        // GET: Interfaces
        public async Task<IActionResult> Index()
        {
            return View(await _context.Interface.ToListAsync());
        }

        // GET: Interfaces/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @interface = await _context.Interface
                .FirstOrDefaultAsync(m => m.Id == id);
            if (@interface == null)
            {
                return NotFound();
            }

            return View(@interface);
        }

        // GET: Interfaces/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Interfaces/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,NormalState")] Interface @interface)
        {
            if (ModelState.IsValid)
            {
                _context.Add(@interface);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(@interface);
        }

        // GET: Interfaces/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @interface = await _context.Interface.FindAsync(id);
            if (@interface == null)
            {
                return NotFound();
            }
            return View(@interface);
        }

        // POST: Interfaces/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,NormalState")] Interface @interface)
        {
            if (id != @interface.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(@interface);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!InterfaceExists(@interface.Id))
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
            return View(@interface);
        }

        // GET: Interfaces/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @interface = await _context.Interface
                .FirstOrDefaultAsync(m => m.Id == id);
            if (@interface == null)
            {
                return NotFound();
            }

            return View(@interface);
        }

        // POST: Interfaces/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var @interface = await _context.Interface.FindAsync(id);
            _context.Interface.Remove(@interface);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool InterfaceExists(int id)
        {
            return _context.Interface.Any(e => e.Id == id);
        }
    }
}
