using Microsoft.AspNetCore.Mvc;
using Proyecto.Data;
using Proyecto.Models;
using Proyecto.Extensions;

namespace Proyecto.Controllers
{
    public class CarritoController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CarritoController(ApplicationDbContext context)
        {
            _context = context;
        }

        // 1. VER EL CARRITO
        public IActionResult Index()
        {
            var carrito = HttpContext.Session.GetObject<List<CarritoItem>>("Carrito") ?? new List<CarritoItem>();
            return View(carrito);
        }

        // 2. AGREGAR PRODUCTO AL CARRITO
        public async Task<IActionResult> Agregar(int id, int cantidad = 1)
        {
            var producto = await _context.Productos.FindAsync(id);
            if (producto == null) return NotFound();

            var carrito = HttpContext.Session.GetObject<List<CarritoItem>>("Carrito") ?? new List<CarritoItem>();

            var itemExistente = carrito.FirstOrDefault(c => c.ProductoId == id);

            if (itemExistente != null)
            {
                itemExistente.Cantidad += cantidad;
            }
            else
            {
                carrito.Add(new CarritoItem
                {
                    ProductoId = producto.Id,
                    Nombre = producto.Nombre,
                    Precio = producto.Precio,
                    Cantidad = cantidad,
                    FotoUrl = producto.FotoUrl
                });
            }

            HttpContext.Session.SetObject("Carrito", carrito);

            return RedirectToAction("Index");
        }

        // 3. ELIMINAR PRODUCTO DEL CARRITO
        public IActionResult Eliminar(int id)
        {
            var carrito = HttpContext.Session.GetObject<List<CarritoItem>>("Carrito");

            if (carrito != null)
            {
                var item = carrito.FirstOrDefault(c => c.ProductoId == id);
                if (item != null)
                {
                    carrito.Remove(item);
                    HttpContext.Session.SetObject("Carrito", carrito);
                }
            }

            return RedirectToAction("Index");
        }
    }
}
