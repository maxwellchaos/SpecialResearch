using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SpecialResearch.Data;
using SpecialResearch.Models;

namespace SpecialResearch.Controllers
{
    [Authorize(Roles = Startup.AdminRole + "," +Startup.ControllerRole + "," +Startup.ManagerRole + "," +Startup.TesterRole)]
    public class TestResultsController : Controller
    {
        private readonly SpecialResearchContext _context;

        public TestResultsController(SpecialResearchContext context)
        {
            _context = context;
        }

        // GET: TestResults
        public async Task<IActionResult> Index()
        {
            var specialResearchContext = _context.TestResult.Include(t => t.Equipment).Include(t => t.Interface).Include(t => t.TestType).Include(t => t.User);
            return View(await specialResearchContext.ToListAsync());
        }



        //// GET: Equipments
        public async Task<IActionResult> List(int? id)
        {
            var specialResearchContext = _context.TestResult
                .Include(t => t.Equipment)
                .Include(t=>t.User)
                .Include(t=>t.Interface)
                .Include(t=>t.TestType)
                .Where(e => e.EquipmentId == id);
            Equipment eq = _context.Equipment.Where(e => e.Id == id).FirstOrDefault();
            ViewBag.eq = eq;
            String ReqNum = _context.Request.Where(r => r.Id == eq.RequestId).FirstOrDefault().Number;
            ViewBag.ReqNum = ReqNum;
            return View(await specialResearchContext.ToListAsync());
        }

        // GET: TestResults/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var testResult = await _context.TestResult
                .Include(t => t.Equipment)
                .Include(t => t.Interface)
                .Include(t => t.TestType)
                .Include(t => t.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (testResult == null)
            {
                return NotFound();
            }

            return View(testResult);
        }

       
        // GET: TestResults/Create
        public IActionResult Create(int? id)//equipment id
        {
            //ViewData["EquipmentId"] = new SelectList(_context.Equipment, "Id", "Name");
            ViewBag.eq = _context.Equipment.Where(e => e.Id == id).FirstOrDefault();
            ViewData["InterfaceId"] = new SelectList(_context.Interface, "Id", "Name");
            ViewData["TestTypeId"] = new SelectList(_context.TestType, "Id", "TestName");
            // ViewData["UserId"] = new SelectList(_context.User, "Id", "Login");
            TestResult testResult = new TestResult();
            testResult.UserId = (int)HttpContext.Session.GetInt32("CurrentUserId");//Залогинившийся юзер
            testResult.EquipmentId = (int)id;
            testResult.Date = DateTime.Now;
            return View(testResult);
        }

        // POST: TestResults/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EquipmentId,Result,InterfaceId,SignalFound,TestIsOk,Date,frequency,TestTypeId,UserId")] TestResult testResult)
        {
            if (ModelState.IsValid)
            {
                _context.Add(testResult);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(List), new { id = testResult.EquipmentId });
            }
            ViewData["EquipmentId"] = new SelectList(_context.Equipment, "Id", "Name", testResult.EquipmentId);
            ViewData["InterfaceId"] = new SelectList(_context.Interface, "Id", "Name", testResult.InterfaceId);
            ViewData["TestTypeId"] = new SelectList(_context.TestType, "Id", "TestName", testResult.TestTypeId);
            ViewData["UserId"] = new SelectList(_context.User, "Id", "Login", testResult.UserId);
            return View(testResult);
        }

        // GET: TestResults/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var testResult = await _context.TestResult.FindAsync(id);
            if (testResult == null)
            {
                return NotFound();
            }
            ViewBag.eq = _context.Equipment.Where(e => e.Id == id).FirstOrDefault();

            ViewData["InterfaceId"] = new SelectList(_context.Interface, "Id", "Name", testResult.InterfaceId);
            ViewData["TestTypeId"] = new SelectList(_context.TestType, "Id", "TestName", testResult.TestTypeId);
            return View(testResult);
        }

        // POST: TestResults/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,EquipmentId,Result,InterfaceId,SignalFound,TestIsOk,Date,frequency,TestTypeId,UserId")] TestResult testResult)
        {
            if (id != testResult.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(testResult);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TestResultExists(testResult.Id))
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
            ViewData["EquipmentId"] = new SelectList(_context.Equipment, "Id", "Name", testResult.EquipmentId);
            ViewData["InterfaceId"] = new SelectList(_context.Interface, "Id", "Name", testResult.InterfaceId);
            ViewData["TestTypeId"] = new SelectList(_context.TestType, "Id", "TestName", testResult.TestTypeId);
            ViewData["UserId"] = new SelectList(_context.User, "Id", "Login", testResult.UserId);
            return View(testResult);
        }

        // GET: TestResults/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var testResult = await _context.TestResult
                .Include(t => t.Equipment)
                .Include(t => t.Interface)
                .Include(t => t.TestType)
                .Include(t => t.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (testResult == null)
            {
                return NotFound();
            }

            return View(testResult);
        }

        // POST: TestResults/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var testResult = await _context.TestResult.FindAsync(id);
            _context.TestResult.Remove(testResult);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TestResultExists(int id)
        {
            return _context.TestResult.Any(e => e.Id == id);
        }
    }
}
