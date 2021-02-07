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
using OfficeOpenXml;
using System.Security.Claims;

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
        public async Task<IActionResult> Index(string template = null)
        {
            if (template != null)
                ViewBag.Template = template;
           
            var specialResearchContext = _context.Request.Include(r => r.Stage).
                Include(r => r.Creator).Include(r => r.Controler).
                OrderByDescending(r=>r.CreateDate);

            if(User.IsInRole(Startup.RecieverRole))
            {
                if (template == null)
                {
                    return View(specialResearchContext.Where(r => r.StageID == 1));
                }
                else
                {
                    return View(specialResearchContext.Where(r => r.StageID == 1 && r.Number.Contains(template)));
                }
                
            }

            if (User.IsInRole(Startup.TesterRole))
            {
                if (template == null)
                {
                    return View(specialResearchContext.Where(r => r.StageID == 2 || r.StageID == 1));
                }
                else
                {
                    return View(specialResearchContext.Where(r => (r.StageID == 2 || r.StageID == 1) && r.Number.Contains(template)));
                }

               
            }
            if (User.IsInRole(Startup.ControllerRole))
            {
                if (template == null)
                {
                    return View(specialResearchContext.Where(r => r.StageID == 2 || r.StageID == 3));
                }
                else
                {
                    return View(specialResearchContext.Where(r => (r.StageID == 2 || r.StageID == 3) && r.Number.Contains(template)));
                }
               
            }
            if (template == null)
            {
                return View(await specialResearchContext.ToListAsync());
            }
            else
            {
                return View(specialResearchContext.Where(r => r.Number.Contains(template)));
            }
            
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
                .Include(r => r.Creator)
                .Include(r => r.Controler)
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
            Request request = new Request();
            //Ставим данные по умолчанию для создания заявки
            request.CreateDate = DateTime.Now;//Сейчас
            int id = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value);
            request.CreatorId = id;
            //request.CreatorId = (int)HttpContext.Session.GetInt32("CurrentUserId");//Залогинившийся юзер - создатель
            int lastId = 0;
            try
            {
                lastId = _context.Request.Max(r => r.Id);
            }
            catch(Exception e)
            {

            }
            request.Number = "УК-" + (lastId + 1).ToString()+"-" + DateTime.Now.ToString("yyyy");
            ViewBag.UserName = _context.User.Where(u => u.Id == request.CreatorId).FirstOrDefault().Name;

            return View(request);
        }

        // POST: Requests/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Number,CreateDate,StageID,CreatorId,ControllerId,UseOrder,EndDate,PhotoCopy")] Request request)
        {
            request.ControlerId = 1;
            request.StageID = 1;
            //request.CreatorId = 2;
            if (ModelState.IsValid)
            {
                _context.Add(request);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["StageID"] = new SelectList(_context.Stage, "Id", "StageName", request.StageID);
            ViewData["CreatorId"] = new SelectList(_context.User, "Id", "Login", request.CreatorId);
            ViewData["ControlerId"] = new SelectList(_context.User, "Id", "Login", request.ControlerId);
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
            ViewData["CreatorId"] = new SelectList(_context.User, "Id", "Login", request.CreatorId);
            ViewData["ControlerId"] = new SelectList(_context.User, "Id", "Login", request.ControlerId);
            
            return View(request);
        }

        //UploadRequest
        public async Task<IActionResult> UploadRequest( IFormFile UploadedFile)
        {

            if (UploadedFile != null)
            {
                //разбираем файл


                string Path = "/files/" + UploadedFile.FileName;

                using (var fileStream = new FileStream(_webHostEnvironment.WebRootPath + Path, FileMode.Create))
                {
                    UploadedFile.CopyTo(fileStream);
                }
                FileInfo fi = new FileInfo(_webHostEnvironment.WebRootPath + Path);
                //Чтобы работал импорт из экселя
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                using (ExcelPackage excelPackage = new ExcelPackage(fi))
                {
                    if (excelPackage.Workbook.Worksheets.Count == 0)
                    {
                        throw new Exception("Ошибка открытия файла " + UploadedFile.FileName);
                    }
                    ExcelWorksheet firstWorksheet = excelPackage.Workbook.Worksheets[0];
                    Request newRequest = new Request();
                    newRequest.CreateDate = DateTime.Now;
                    newRequest.Number = firstWorksheet.Cells["E3"].Value.ToString();
                    newRequest.StageID = 1;
                    newRequest.ControlerId = 1;
                    int id = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value);
                    newRequest.CreatorId = id;
                   // newRequest.CreatorId = (int)HttpContext.Session.GetInt32("CurrentUserId");//Залогинившийся юзер - создатель
                    _context.Add(newRequest);
                    _context.SaveChanges();

                    //Берем количество оборудования из нужной ячейки
                    double count = (double)firstWorksheet.Cells["C4"].Value;
                    double count1 = (double)firstWorksheet.Cells[4,3].Value;
                    for (int i = 0;i<count;i++)
                    {
                        Equipment eq = new Equipment();
                        eq.Name = firstWorksheet.Cells[5 + i, 4].Value.ToString();
                        if(firstWorksheet.Cells[5 + i, 5].Value != null)
                            eq.Manufacturer = firstWorksheet.Cells[5 + i, 5].Value.ToString();
                        if (firstWorksheet.Cells[5 + i, 6].Value != null)
                            eq.Model = firstWorksheet.Cells[5 + i, 6].Value.ToString();
                        if (firstWorksheet.Cells[5 + i, 7].Value != null)
                            eq.SerialNumber = firstWorksheet.Cells[5 + i, 7].Value.ToString();
                        if (firstWorksheet.Cells[5 + i, 8].Value != null)
                            eq.OperatingMode = firstWorksheet.Cells[5 + i, 8].Value.ToString();
                        eq.Request = newRequest;
                        _context.Add(eq);
                    }
                    // создание и добавление моделей
                    //Team t1 = new Team { Name = "Барселона" };
                    //Team t2 = new Team { Name = "Реал Мадрид" };
                    //db.Teams.Add(t1);
                    //db.Teams.Add(t2);
                    //db.SaveChanges();
                    //Player pl1 = new Player { Name = "Роналду", Age = 31, Position = "Нападающий", Team = t2 };
                    //Player pl2 = new Player { Name = "Месси", Age = 28, Position = "Нападающий", Team = t1 };
                    //Player pl3 = new Player { Name = "Хави", Age = 34, Position = "Полузащитник", Team = t1 };
                    //db.Players.AddRange(new List<Player> { pl1, pl2, pl3 });
                    //db.SaveChanges();

                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View();
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
           
           
            return View();
        }

        //GetUse
        [HttpPost]
        public async Task<IActionResult> GetUse(int? id,string UseOrder)
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

            request.UseOrder = UseOrder;
            int ControlerId = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value);
            request.ControlerId = ControlerId;//Залогинившийся юзер - Выдал предписание

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
        public async Task<IActionResult> Edit(int id, [Bind("Id,Number,CreateDate,StageID,CreatorId,ControllerId,UseOrder,EndDate,PhotoCopy")] Request request)
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
            ViewData["CreatorId"] = new SelectList(_context.User, "Id", "Login", request.CreatorId);
            ViewData["ControlerId"] = new SelectList(_context.User, "Id", "Login", request.ControlerId);
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
                .Include(r => r.Creator)
                .Include(r => r.Controler)
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
