using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Proyecto.Data;
using Proyecto.Models;
using System.Text.RegularExpressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace Proyecto.Controllers
{
    public class MascotasController : Controller
    {
        private readonly ApplicationDbContext _context;

        private readonly IWebHostEnvironment _hostEnvironment;

        public MascotasController(ApplicationDbContext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
        }
        // GET: Mascotas
        public async Task<IActionResult> Index()
        {
            return View(await _context.Mascotas.ToListAsync());
        }
        // GET: Mascotas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var mascota = await _context.Mascotas
                .FirstOrDefaultAsync(m => m.Id == id);
            if (mascota == null)
            {
                return NotFound();
            }

            return View(mascota);
        }

        // GET: Mascotas/Create
        public IActionResult Create()
        {
            return View();
        }
        // POST: Mascotas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Nombre,Especie,Raza,Edad,EstadoAdopcion")] Mascota mascota, IFormFile? foto)
        {
            if (ModelState.IsValid)
            {
                if (foto != null)
                {
                    string wwwRootPath = _hostEnvironment.WebRootPath;
                    string imagesPath = Path.Combine(wwwRootPath, "images");

                    if (!Directory.Exists(imagesPath))
                        Directory.CreateDirectory(imagesPath);

                    // 1) Obtener nombre ORIGINAL del archivo
                    string originalName = Path.GetFileNameWithoutExtension(foto.FileName);
                    string extension = Path.GetExtension(foto.FileName);

                    // 2) Limpiar nombre (sin espacios raros ni caracteres inválidos)
                    originalName = Regex.Replace(originalName, @"[^a-zA-Z0-9_-]", "_");

                    // 3) Crear un nombre final evitando reemplazar archivos existentes
                    string fileName = originalName + extension;
                    string finalPath = Path.Combine(imagesPath, fileName);

                    int counter = 1;
                    while (System.IO.File.Exists(finalPath))
                    {
                        fileName = $"{originalName}_{counter}{extension}";
                        finalPath = Path.Combine(imagesPath, fileName);
                        counter++;
                    }

                    // 4) Copiar archivo
                    using (var stream = new FileStream(finalPath, FileMode.Create))
                    {
                        await foto.CopyToAsync(stream);
                    }

                    // 5) Guardar URL accesible
                    mascota.FotoUrl = "/images/" + fileName;
                }

                // Normalización de estado
                mascota.EstadoAdopcion = string.IsNullOrWhiteSpace(mascota.EstadoAdopcion)
                                       ? "SIN ADOPTAR"
                                       : Regex.Replace(mascota.EstadoAdopcion.Trim(), @"\s+", " ").ToUpperInvariant();

                _context.Add(mascota);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return View(mascota);
        }
        // GET: Mascotas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var mascota = await _context.Mascotas.FindAsync(id);
            if (mascota == null)
            {
                return NotFound();
            }
            return View(mascota);
        }
        // POST: Mascotas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nombre,Especie,Raza,Edad,EstadoAdopcion,FotoUrl")] Mascota mascota, IFormFile? foto)
        {
            if (id != mascota.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                if (foto != null)
                {
                    string wwwRootPath = _hostEnvironment.WebRootPath;
                    string imagesPath = Path.Combine(wwwRootPath, "images");

                    if (!Directory.Exists(imagesPath))
                        Directory.CreateDirectory(imagesPath);

                    // 1. Borrar imagen anterior si existe
                    if (!string.IsNullOrEmpty(mascota.FotoUrl))
                    {
                        string fullOldPath = Path.Combine(wwwRootPath, mascota.FotoUrl.TrimStart('/'));
                        if (System.IO.File.Exists(fullOldPath))
                            System.IO.File.Delete(fullOldPath);
                    }

                    // 2. Obtener nombre original
                    string originalName = Path.GetFileNameWithoutExtension(foto.FileName);
                    string extension = Path.GetExtension(foto.FileName);

                    originalName = Regex.Replace(originalName, @"[^a-zA-Z0-9_-]", "_");

                    string fileName = originalName + extension;
                    string finalPath = Path.Combine(imagesPath, fileName);

                    int counter = 1;
                    while (System.IO.File.Exists(finalPath))
                    {
                        fileName = $"{originalName}_{counter}{extension}";
                        finalPath = Path.Combine(imagesPath, fileName);
                        counter++;
                    }

                    // 3. Guardar la nueva imagen
                    using (var stream = new FileStream(finalPath, FileMode.Create))
                    {
                        await foto.CopyToAsync(stream);
                    }

                    mascota.FotoUrl = "/images/" + fileName;
                }

                // Obtener registro original desde la BD
                var mascotaOriginal = await _context.Mascotas.AsNoTracking()
                                                             .FirstOrDefaultAsync(m => m.Id == mascota.Id);

                if (mascotaOriginal == null)
                    return NotFound();

                // ESTADO (MUY IMPORTANTE)
                if (!string.IsNullOrWhiteSpace(mascota.EstadoAdopcion))
                {
                    mascotaOriginal.EstadoAdopcion = Regex.Replace(
                        mascota.EstadoAdopcion.Trim(), @"\s+", " "
                    ).ToUpperInvariant();
                }

                _context.Update(mascota);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return View(mascota);
        }

        // GET: Mascotas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var mascota = await _context.Mascotas
                .FirstOrDefaultAsync(m => m.Id == id);
            if (mascota == null)
            {
                return NotFound();
            }

            return View(mascota);
        }
        // POST: Mascotas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var mascota = await _context.Mascotas.FindAsync(id);
            if (mascota != null)
            {
                _context.Mascotas.Remove(mascota);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        private bool MascotaExists(int id)
        {
            return _context.Mascotas.Any(e => e.Id == id);
        }

        public async Task<IActionResult> Adoptadas()
        {
            var adoptadas = await _context.Mascotas
                .Where(m => m.EstadoAdopcion == "ADOPTADO")
                .ToListAsync();

            return View(adoptadas);
        }

        public async Task<IActionResult> DetallesAdopcion(int id)
        {
            var adopcion = await _context.Adopciones
                .Include(a => a.Mascota)
                .Where(a => a.MascotaId == id && a.Estado == "APROBADO")
                .OrderByDescending(a => a.FechaAdopcion)
                .FirstOrDefaultAsync();

            if (adopcion == null)
                return NotFound();

            return View("DetalleAdopcion", adopcion);
        }
    }
}
