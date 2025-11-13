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

        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> Postulaciones()
        {
            var postulaciones = await _context.Adopciones
                .Include(a => a.Mascota)
                .Where(a => a.Estado == "Enviado")
                .ToListAsync();
            return View(postulaciones);
        }

        [HttpPost]
        [ValidateAntiForgeryToken] // Buena práctica de seguridad
        public async Task<IActionResult> Aprobar(int id)
        {
            var postulacion = await _context.Adopciones
                                      .Include(a => a.Mascota) // ¡Importante incluir la mascota!
                                      .FirstOrDefaultAsync(a => a.Id == id);

            if (postulacion == null)
            {
                return NotFound();
            }

            // 1. Actualiza la postulación
            postulacion.Estado = "Aprobado";

            // 2. Actualiza la mascota
            if (postulacion.Mascota != null)
            {
                postulacion.Mascota.EstadoAdopcion = "Adoptado";
            }

            // 3. Guarda los cambios
            await _context.SaveChangesAsync();

            return RedirectToAction("Postulaciones");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Rechazar(int id)
        {
            var postulacion = await _context.Adopciones
                                      .Include(a => a.Mascota) // ¡Importante incluir la mascota!
                                      .FirstOrDefaultAsync(a => a.Id == id);

            if (postulacion == null)
            {
                return NotFound();
            }

            // 1. Actualiza la postulación
            postulacion.Estado = "Rechazado";

            // 2. Devuelve la mascota a "Disponible"
            if (postulacion.Mascota != null)
            {
                postulacion.Mascota.EstadoAdopcion = "Disponible";
            }

            // 3. Guarda los cambios
            await _context.SaveChangesAsync();

            return RedirectToAction("Postulaciones");
        }
    }
}
