using Microsoft.AspNetCore.Mvc;
using Proyecto.Models;
using Proyecto.Data;

namespace Proyecto.Controllers
{
    public class DonacionController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DonacionController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Procesar(Donacion donacion)
        {
            if (ModelState.IsValid)
            {
                donacion.FechaDonacion = DateTime.Now;
                _context.Add(donacion);
                await _context.SaveChangesAsync();

                return RedirectToAction("Gracias");
            }
            return View("Index", donacion);
        }

        public IActionResult Gracias()
        {
            return View();
        }
    }
}
