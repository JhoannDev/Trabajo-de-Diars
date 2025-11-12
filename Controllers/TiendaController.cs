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
            var productosQuery = _context.Productos.AsQueryable();

            if (!String.IsNullOrEmpty(filtroBusqueda))
            {
                productosQuery = productosQuery.Where(p =>
                    p.Nombre.Contains(filtroBusqueda) ||
                    p.Descripcion.Contains(filtroBusqueda));
            }

            var productos = await productosQuery.ToListAsync();
            return View(productos);
        }
    }
}
