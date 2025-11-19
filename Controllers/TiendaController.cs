using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proyecto.Data;

namespace Proyecto.Controllers
{
    public class TiendaController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TiendaController(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index(string filtroBusqueda)
        {
            var productos = from p in _context.Productos
                            select p;

            if (!String.IsNullOrEmpty(filtroBusqueda))
            {
                productos = productos.Where(s =>
                    s.Nombre.Contains(filtroBusqueda) ||
                    s.Descripcion.Contains(filtroBusqueda) ||
                    s.TipoProducto.Contains(filtroBusqueda));
            }

            return View(await productos.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var producto = await _context.Productos.FindAsync(id);

            if (producto == null) return NotFound();

            return View(producto);
        }
    }
}
