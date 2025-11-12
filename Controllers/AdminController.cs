using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proyecto.Data;

namespace Proyecto.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Postulaciones()
        {
            var postulaciones = await _context.Adopciones
                .Include(a => a.Mascota)
                .Where(a => a.Estado == "Postulado")
                .ToListAsync();
            return View(postulaciones);
        }
    }
}
