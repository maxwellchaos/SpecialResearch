using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SpecialResearch.Data;
using SpecialResearch.Models;

namespace SpecialResearch.Controllers
{
    //доступен для испытателя и админа
    //автосгенерированный код. не редактировался.
    //типы испытаний(тестов)
    [Authorize(Roles = "admin,tester")]
    public class TestTypesController : Controller
    {
        private readonly SpecialResearchContext _context;

        public TestTypesController(SpecialResearchContext context)
        {
            _context = context;
        }

        //список всех типов тестов
        // GET: TestTypes
        public async Task<IActionResult> Index()
        {
            return View(await _context.TestType.ToListAsync());
        }
        //детально по кадому типу теста
        // GET: TestTypes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var testType = await _context.TestType
                .FirstOrDefaultAsync(m => m.Id == id);
            if (testType == null)
            {
                return NotFound();
            }

            return View(testType);
        }


        //страница добавления типа теста
        // GET: TestTypes/Create
        public IActionResult Create()
        {
            return View();
        }


        //добавить тест в БД
        // POST: TestTypes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,TestName,TestDescription")] TestType testType)
        {
            if (ModelState.IsValid)
            {
                _context.Add(testType);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(testType);
        }

        //отредактировать тип испытания
        // GET: TestTypes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var testType = await _context.TestType.FindAsync(id);
            if (testType == null)
            {
                return NotFound();
            }
            return View(testType);
        }

        //поменять данные в бд
        // POST: TestTypes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,TestName,TestDescription")] TestType testType)
        {
            if (id != testType.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(testType);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TestTypeExists(testType.Id))
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
            return View(testType);
        }

        // GET: TestTypes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var testType = await _context.TestType
                .FirstOrDefaultAsync(m => m.Id == id);
            if (testType == null)
            {
                return NotFound();
            }

            return View(testType);
        }

        // POST: TestTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var testType = await _context.TestType.FindAsync(id);
            _context.TestType.Remove(testType);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TestTypeExists(int id)
        {
            return _context.TestType.Any(e => e.Id == id);
        }
    }
}
