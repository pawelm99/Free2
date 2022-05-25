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
            var computers = await db.GetComputersAsync(filter.Year, filter.ComputerName);
            var model = new ComputerList { Computers = computers, Filter = filter };

            return View(model);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Computers computers)
        {
            if (computers != null)
            {
                await db.Create(computers);
                return RedirectToAction("Index");
            }
            return View(computers);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


        public async Task<IActionResult> Edit(string id)
        {
            Computers computer = await db.GetComputers(id);
            if (computer == null)
            {
                throw new Exception("Not find PC");

            }
            else return View(computer);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Computers computers)
        {
            if (computers != null)
            {
                await db.Update(computers);
                return RedirectToAction("Index");
            }
            else return View(computers);
        }

        public async Task<IActionResult> Delete(string id)
        {
            await db.Remove(id);
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> AttachImage(string id)
        {
            Computers computers = await db.GetComputers(id);
            if (computers == null)
            {
                throw new Exception("Computers is null");

            }
            return View(computers);
        }

        [HttpPost]
        public async Task<IActionResult> AttachImage([FromForm(Name = "file")] IFormFile file, string id)
        {
            if (file != null)
            {




                await db.StoreImage(id, file.OpenReadStream(), file.FileName);

            }

            return RedirectToAction("Index");
        }

        public async Task<ActionResult> GetImage(string id)
        {
            var image = await db.GetImage(id);
            if (image == null)
            {
                return NotFound();
            }
            return File(image, "image/png");

        }
    }
}