using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proyecto.Data;
using Proyecto.Models;

namespace SistemaAdopcion.Controllers
{
    public class AdopcionController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdopcionController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string filtroBusqueda)
        {
            var mascotasQuery = _context.Mascotas
                                    .Where(m => m.EstadoAdopcion == "Disponible")
                                    .AsQueryable();

            if (!String.IsNullOrEmpty(filtroBusqueda))
            {
                mascotasQuery = mascotasQuery.Where(m =>
                    m.Nombre.Contains(filtroBusqueda) ||
                    m.Especie.Contains(filtroBusqueda) ||
                    m.Raza.Contains(filtroBusqueda));
            }

            var mascotas = await mascotasQuery.ToListAsync();
            return View(mascotas);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var mascota = await _context.Mascotas.FindAsync(id);
            if (mascota == null) return NotFound();

            return View(mascota);
        }

        [HttpGet]
        public IActionResult Postular(int? id)
        {
            if (id == null) return NotFound();

            var modeloDePostulacion = new Adopcion
            {
                MascotaId = id.Value
            };
            return View(modeloDePostulacion);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Postular(Adopcion adopcion)
        {
            ModelState.Remove("Mascota");

            if (ModelState.IsValid)
            {
                adopcion.FechaPostulacion = DateTime.Now;
                adopcion.Estado = "Enviado";

                var mascota = await _context.Mascotas.FindAsync(adopcion.MascotaId);
                if (mascota != null && mascota.EstadoAdopcion == "Disponible")
                {
                    mascota.EstadoAdopcion = "En Proceso";
                    _context.Update(mascota);
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Esta mascota ya no está disponible o ya ah sido adoptado.");
                    return View(adopcion);
                }

                _context.Add(adopcion);
                await _context.SaveChangesAsync();

                return RedirectToAction("PostulacionEnviada");
            }

            return View(adopcion);
        }

        public IActionResult PostulacionEnviada()
        {
            return View();
        }
    }
}