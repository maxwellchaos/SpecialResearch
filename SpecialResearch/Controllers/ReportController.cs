using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SpecialResearch.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpecialResearch.Controllers
{
    //Контроллер отчетов

    //доступен управленцу и админу
    [Authorize (Roles = "admin,manager")]
    public class ReportController : Controller
    {
        //Клаccы для отчетов
        public class yearCount
        {
            //класс года
            public int Count;//количество в год
            public int Year;//год
            public List<MonthCount> MonthsCount;//список месяцев
            
        }
        public class MonthCount
        {
            //класс месяцев
            public int Count;//количество в месяц
            public string MonthsName;//название месяца
        }

        private readonly SpecialResearchContext _context;//БД

        public ReportController(SpecialResearchContext context)
        {
            _context = context;
        }

        // GET: ReportController
        public ActionResult Index()
        {
            return View();
        }

        //FailTest
        //отчет проваленных испытаний
        public async Task<IActionResult> FailTest()
        {
            //выбрать все испытания
            var EqWithTests = _context.TestResult.Include(t => t.Equipment).Include(e => e.Equipment.Request);
         //Подсчитать количество проваленных испытаний
            foreach (var tr in EqWithTests)//перебрать все испытания
            {
                tr.Equipment.TestResultCount++;//увеличить счетчик испытаний
                if (!tr.TestIsOk)//если испытание провалено
                    tr.Equipment.TestResultFailCount++;//увеличить счетчик проваленных испытианий
            }

            var specialResearchContext = _context.Equipment.Include(e => e.Request);
            //Задать даты испытаний
            ViewBag.EndDate = DateTime.Now;
            ViewBag.StartDate = DateTime.Now.AddYears(-1);

            return View(await specialResearchContext.ToListAsync());
        }


        //Если изменены даты испытаний то обновить страницу отчета. 
        // POST: Requests/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> FailTest(DateTime StartDate, DateTime EndDate)
        {
            //меняем местами если надо
            if (StartDate > EndDate)
            {
                DateTime tmp = StartDate;
                StartDate = EndDate;
                EndDate = tmp;
            }
            ViewBag.EndDate = EndDate;
            ViewBag.StartDate = StartDate;
            var specialResearchContext = _context.TestResult.Include(t => t.Equipment).Include(t => t.Interface).Include(t => t.TestType).Include(t => t.User)
                .Where(t => t.Date > StartDate)
                .Where(t => t.Date < EndDate);
            return View(await specialResearchContext.ToListAsync());
        }

        //Отчет по пройденным испытаниям
        //TestResults
        // GET: TestResults
        public async Task<IActionResult> TestResults()
        {
            //выбор всего из бд
            var specialResearchContext = _context.TestResult.Include(t => t.Equipment).Include(t => t.Interface).Include(t => t.TestType).Include(t => t.User);
            //начальные даты
            ViewBag.EndDate = DateTime.Now;
            ViewBag.StartDate = DateTime.Now.AddYears(-1);
            return View(await specialResearchContext.ToListAsync());
        }


        //Если изменены даты испытаний то обновить страницу отчета. 
        // POST: Requests/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TestResults(DateTime StartDate,DateTime EndDate)
        {
            //меняем местами если надо
            if(StartDate> EndDate)
            {
                DateTime tmp = StartDate;
                StartDate = EndDate;
                EndDate = tmp;
            }
            
            ViewBag.EndDate = EndDate;
            ViewBag.StartDate = StartDate;
            var specialResearchContext = _context.TestResult.Include(t => t.Equipment).Include(t => t.Interface).Include(t => t.TestType).Include(t => t.User)
                .Where(t=>t.Date > StartDate)
                .Where(t => t.Date < EndDate);
            return View(await specialResearchContext.ToListAsync());
        }

        //отчет по закрытым заявкам
        public ActionResult ClosedRequest()
        {
            //создать список лет
            List<yearCount> YearList = new List<yearCount>();
            try//попробовать
            {
               //взять крайние даты из бд по закрытию заявок
                DateTime FirstDate = _context.Request.Where(r => r.StageID == 4).
                    Where(r => r.EndDate != null).Max(r => r.EndDate);
                DateTime LastDate = _context.Request.Where(r => r.StageID == 4).
                    Where(r => r.EndDate != null).Min(r => r.EndDate);

                //перебрать годы
                for (int year = FirstDate.Year; year <= LastDate.Year; year++)
                {
                    //для каждого года
                    yearCount year1 = new yearCount();
                    year1.Year = year;
                    //задать крайние даты года
                    DateTime firstYearDate = new DateTime(year, 1, 1, 0, 0, 0);
                    DateTime lastYearDate = firstYearDate.AddYears(1).AddMilliseconds(-1);
                    //выбрать все из бд между крайними датами
                    year1.Count = _context.Request.Where(r => r.StageID == 4).Where(r => r.EndDate != null)
                        .Where(r => r.EndDate > firstYearDate)
                        .Where(r => r.EndDate < lastYearDate)
                        .Count();

                    //создать список месяцев для года
                    year1.MonthsCount = new List<MonthCount>();
                    //перебрать месяцы
                    for (int month = 1; month <= 12; month++)
                    {
                        //для каждого месяца
                        MonthCount mc = new MonthCount();
                        //взять имя месяца
                        mc.MonthsName = (new DateTime(year, month, 1)).ToString("MMMM");
                        //выделить крайние даты месяца
                        DateTime firstMonthDate = new DateTime(year, month, 1, 0, 0, 0);
                        DateTime lastMonthDate = firstMonthDate.AddMonths(1).AddMilliseconds(-1);
                        //взять все заявки между крайними датами из БД
                        mc.Count = _context.Request.Where(r => r.StageID == 4).Where(r => r.EndDate != null)
                        .Where(r => r.EndDate > firstMonthDate)
                        .Where(r => r.EndDate < lastMonthDate)
                        .Count();
                        year1.MonthsCount.Add(mc);//добавить месяц в список месяцев
                    }
                    YearList.Add(year1);//добавить год в список лет

                }
            }
            catch(Exception e)//если что-то пошло не так
            {
                YearList.Clear();//очистить список лет
                //у меня оно упало когда небыло ни одной заявки. поэтому так чтобы не падало
            }
            ViewBag.y = YearList;

            return View();//показать
        }

        //отчет по созданным заявкам (то же, что и выше но по другому полю в бд)
        public ActionResult OpenedRequest()
        {
            List<yearCount> YearList = new List<yearCount>();
            DateTime FirstDate = _context.Request.
                Where(r => r.CreateDate != null).Max(r => r.CreateDate);
            DateTime LastDate = _context.Request.
                Where(r => r.CreateDate != null).Min(r => r.CreateDate);

            //перебрать годы
            for (int year = FirstDate.Year; year <= LastDate.Year; year++)
            {
                yearCount year1 = new yearCount();
                year1.Year = year;
                DateTime firstYearDate = new DateTime(year, 1, 1, 0, 0, 0);
                DateTime lastYearDate = firstYearDate.AddYears(1).AddMilliseconds(-1);

                year1.Count = _context.Request.Where(r => r.CreateDate != null)
                    .Where(r => r.CreateDate > firstYearDate)
                    .Where(r => r.CreateDate < lastYearDate)
                    .Count();

                year1.MonthsCount = new List<MonthCount>();
                //перебрать месяцы
                for (int month = 1; month <= 12; month++)
                {
                    MonthCount mc = new MonthCount();
                    mc.MonthsName = (new DateTime(year, month, 1)).ToString("MMMM");
                    DateTime firstMonthDate = new DateTime(year, month, 1, 0, 0, 0);
                    DateTime lastMonthDate = firstMonthDate.AddMonths(1).AddMilliseconds(-1);
                    mc.Count = _context.Request.Where(r => r.CreateDate != null)
                    .Where(r => r.CreateDate > firstMonthDate)
                    .Where(r => r.CreateDate < lastMonthDate)
                    .Count();
                    year1.MonthsCount.Add(mc);
                }
                YearList.Add(year1);

            }
            ViewBag.y = YearList;

            return View();
        }



        //дальше идут автосгенерированные методы. пусть будут, хоть и не обращаюсь к ним.



        // GET: ReportController/Details/5
        public ActionResult BaseMetod()
        {

            return View();
        }

        // GET: ReportController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ReportController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: ReportController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: ReportController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: ReportController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: ReportController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
