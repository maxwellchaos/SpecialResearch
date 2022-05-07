using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SpecialResearch.Data;
using SpecialResearch.Models;

namespace SpecialResearch.Controllers
{
    [Authorize]
    public class EquipmentsController : Controller
    {
        private readonly SpecialResearchContext _context;//для доступа к БД
        private readonly IWebHostEnvironment _webHostEnvironment;//Для доступа к файловой системе

        public EquipmentsController(SpecialResearchContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        //не использую - автоматически сгенерированный код

        // GET: Equipments
        public async Task<IActionResult> Index()
        {
            var specialResearchContext = _context.Equipment.Include(e => e.Request);
            var EqWithTests = _context.TestResult.Include(t=>t.Equipment).Include(e => e.Equipment.Request);
            foreach(var tr in EqWithTests)
            {
                tr.Equipment.TestResultCount++;
                if(!tr.TestIsOk)
                    tr.Equipment.TestResultFailCount++;
            }
            return View(await specialResearchContext.ToListAsync());
        }


        //Загрузить файл картинки
        public async Task<IActionResult> AddFile(int? id, IFormFile UploadedFile)
        {
            if (id != null && UploadedFile != null)//если указан id и файл 
            {
                //запишем файл в папку
                string Path = "/files/" + Guid.NewGuid().ToString() + UploadedFile.FileName;
                using (var fileStream = new FileStream(_webHostEnvironment.WebRootPath + Path, FileMode.Create))
                {
                    await UploadedFile.CopyToAsync(fileStream);
                }
                //добудем оборудование из БД по id
                Equipment equipment = _context.Equipment.Where(r => r.Id == id).FirstOrDefault();
                //запишем путь к файлу
                equipment.PhotoCopy = Path;
                await _context.SaveChangesAsync();//сохраним
                //перенапрамим на заявку
                return RedirectToAction(nameof(List), new { id = equipment.RequestId });
            }
            return View();
        }


        //Откыть список оборудования в форме заявки
        //// GET: Equipments
        public async Task<IActionResult> List(int? id)
        {
            //данные по оборудованию
            var specialResearchContext = _context.Equipment
                .Include(e => e.Request)
                .Where(p => p.RequestId == id);
            //данные по заявке
            var ReqContext = _context.Request
                .Where(p => p.Id == id)
                .Include(u => u.Stage)
                .Include(u => u.Creator)
                .Include(u => u.Controler).ToList();
            Request rq = _context.Request.Where(p => p.Id == id).FirstOrDefault();
            ViewBag.rq = rq;
            return View(await specialResearchContext.ToListAsync());
        }

        //не использую - автоматически сгенерированный код
        // GET: Equipments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var equipment = await _context.Equipment
                .Include(e => e.Request)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (equipment == null)
            {
                return NotFound();
            }

            return View(equipment);
        }

        //создание оборудования - открыть страницу создания
        //на вход подается id заявки
        // GET: Equipments/Create
        public IActionResult Create(int? id)
        {
            //ViewData["RequestId"] = new SelectList(_context.Request, "Id", "Number");
            // ViewBag.RequestId = id;
            Equipment equipmentData = new Equipment();
            equipmentData.RequestId = (int)id;
            ViewBag.RequestName = _context.Request.Where(r => r.Id == id).FirstOrDefault().Number;
            return View(equipmentData);
        }

        //создание оборудования
        //получили данные со страницы и записываем в БД
        // POST: Equipments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,RequestId,Manufacturer,Model,SerialNumber,OperatingMode")] Equipment equipment)
        {
          //  equipment.RequestId = equipmentData.RequestId;
            if (ModelState.IsValid)
            {
                _context.Add(equipment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(List),new { id = equipment.RequestId });
            }
            ViewData["RequestId"] = new SelectList(_context.Request, "Id", "Number", equipment.RequestId);
            return View(equipment);
        }


        //Изменить - вызываем страницу изменения и заполняем ее данными
        // GET: Equipments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var equipment = await _context.Equipment.FindAsync(id);
            if (equipment == null)
            {
                return NotFound();
            }
            //ViewData["RequestId"] = new SelectList(_context.Request, "Id", "Number", equipment.RequestId);
            int RequestId = _context.Equipment.Where(e => e.Id == id).FirstOrDefault().RequestId;
            ViewBag.RequestName = _context.Request.Where(r => r.Id == RequestId).FirstOrDefault().Number;
            return View(equipment);
        }

        //сохраняем измененные данные
        // POST: Equipments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,RequestId,Manufacturer,Model,SerialNumber,OperatingMode")] Equipment equipment)
        {
            if (id != equipment.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(equipment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EquipmentExists(equipment.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(List), new { id = equipment.RequestId });
            }
            ViewData["RequestId"] = new SelectList(_context.Request, "Id", "Number", equipment.RequestId);
            return View(equipment);
        }

        //Удалить
        // GET: Equipments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var equipment = await _context.Equipment
                .Include(e => e.Request)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (equipment == null)
            {
                return NotFound();
            }

            return View(equipment);
        }

        //Подтвердить удаление и стереть запись из бд
        // POST: Equipments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var equipment = await _context.Equipment.FindAsync(id);
            _context.Equipment.Remove(equipment);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        //проверка есть ли такое оборудование
        private bool EquipmentExists(int id)
        {
            return _context.Equipment.Any(e => e.Id == id);
        }
    }
}
