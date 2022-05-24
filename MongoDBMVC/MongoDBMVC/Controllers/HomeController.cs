using Infrastructure.Model;
using Microsoft.AspNetCore.Mvc;
using MongoDBMVC.Context;
using MongoDBMVC.Models;
using System.Diagnostics;

namespace MongoDBMVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ComputerContext db;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
            db = new ComputerContext();
        }

        public async Task<IActionResult> Index(ComputerFilter filter)
        {
            var computers = await db.GetComputersAsync(filter.Year,filter.ComputerName);
            var model = new ComputerList { Computers = computers, Filter= filter };

            return View(model);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Computers computers)
        {
            
                await db.Create(computers);
                return RedirectToAction("Index");
            
            return View(computers);
        }

            [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}