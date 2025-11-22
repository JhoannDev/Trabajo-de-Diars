using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Proyecto.Data;
using Proyecto.Models;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using System.Text.RegularExpressions;

namespace Proyecto.Controllers
{
    public class ProductosController : Controller
    {
        private readonly ApplicationDbContext _context;

        private readonly IWebHostEnvironment _hostEnvironment;

        private readonly List<string> _tiposProducto = new()
        {
            "Alimentos",
            "Salud e Higiene",
            "Accesorios y Otros",
        };

        public ProductosController(ApplicationDbContext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
        }

        // GET: Productos
        public async Task<IActionResult> Index()
        {
            return View(await _context.Productos.ToListAsync());
        }

        // GET: Productos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var producto = await _context.Productos
                .FirstOrDefaultAsync(m => m.Id == id);
            if (producto == null)
            {
                return NotFound();
            }

            return View(producto);
        }

        // GET: Productos/Create
        public IActionResult Create()
        {
            ViewBag.TiposProducto = _tiposProducto;
            return View();
        }

        // POST: Productos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
        [Bind("Nombre,TipoProducto,Descripcion,Precio,Stock,FotoUrl")] Producto producto,
        IFormFile? foto)
        {
            if (ModelState.IsValid)
            {
                string wwwRootPath = _hostEnvironment.WebRootPath;
                string productoPath = Path.Combine(wwwRootPath, "images", "productos");

                if (!Directory.Exists(productoPath))
                    Directory.CreateDirectory(productoPath);

                if (foto != null)
                {
                    // Nombre original del archivo
                    string originalName = Path.GetFileNameWithoutExtension(foto.FileName);
                    string extension = Path.GetExtension(foto.FileName);

                    // Limpieza del nombre
                    originalName = Regex.Replace(originalName, @"[^a-zA-Z0-9_-]", "_");

                    // Nombre final
                    string fileName = originalName + extension;
                    string finalPath = Path.Combine(productoPath, fileName);

                    int counter = 1;
                    while (System.IO.File.Exists(finalPath))
                    {
                        fileName = $"{originalName}_{counter}{extension}";
                        finalPath = Path.Combine(productoPath, fileName);
                        counter++;
                    }

                    // Guardar imagen
                    using (var stream = new FileStream(finalPath, FileMode.Create))
                    {
                        await foto.CopyToAsync(stream);
                    }

                    producto.FotoUrl = "/images/productos/" + fileName;
                }

                _context.Add(producto);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            return View(producto);
        }

        // GET: Productos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var producto = await _context.Productos.FindAsync(id);
            if (producto == null)
                return NotFound();

            ViewBag.TiposProducto = _tiposProducto;
            return View(producto);
        }

        // POST: Productos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,
    [Bind("Id,Nombre,TipoProducto,Descripcion,Precio,Stock,FotoUrl")] Producto producto,
    IFormFile? foto)
        {
            if (id != producto.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                string wwwRootPath = _hostEnvironment.WebRootPath;
                string productosPath = Path.Combine(wwwRootPath, "images", "productos");

                if (!Directory.Exists(productosPath))
                    Directory.CreateDirectory(productosPath);

                // Obtener datos actuales
                var productoOriginal = await _context.Productos.AsNoTracking()
                                         .FirstOrDefaultAsync(p => p.Id == producto.Id);

                if (productoOriginal == null)
                    return NotFound();

                if (foto != null)
                {
                    // Borrar la imagen anterior
                    if (!string.IsNullOrEmpty(productoOriginal.FotoUrl))
                    {
                        string oldPath = Path.Combine(wwwRootPath, productoOriginal.FotoUrl.TrimStart('/'));
                        if (System.IO.File.Exists(oldPath))
                            System.IO.File.Delete(oldPath);
                    }

                    // Usar nombre original
                    string originalName = Path.GetFileNameWithoutExtension(foto.FileName);
                    string extension = Path.GetExtension(foto.FileName);

                    originalName = Regex.Replace(originalName, @"[^a-zA-Z0-9_-]", "_");

                    string fileName = originalName + extension;
                    string finalPath = Path.Combine(productosPath, fileName);

                    int counter = 1;
                    while (System.IO.File.Exists(finalPath))
                    {
                        fileName = $"{originalName}_{counter}{extension}";
                        finalPath = Path.Combine(productosPath, fileName);
                        counter++;
                    }

                    // Guardar archivo
                    using (var stream = new FileStream(finalPath, FileMode.Create))
                    {
                        await foto.CopyToAsync(stream);
                    }

                    producto.FotoUrl = "/images/productos/" + fileName;
                }
                else
                {
                    producto.FotoUrl = productoOriginal.FotoUrl; // Mantener imagen
                }

                // Guardar cambios
                try
                {
                    _context.Update(producto);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Productos.Any(p => p.Id == producto.Id))
                        return NotFound();
                    throw;
                }

                return RedirectToAction(nameof(Index));
            }
            return View(producto);
        }

        // GET: Productos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var producto = await _context.Productos
                .FirstOrDefaultAsync(m => m.Id == id);
            if (producto == null)
            {
                return NotFound();
            }

            return View(producto);
        }

        // POST: Productos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var producto = await _context.Productos.FindAsync(id);
            if (producto != null)
            {
                _context.Productos.Remove(producto);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductoExists(int id)
        {
            return _context.Productos.Any(e => e.Id == id);
        }
    }
}
