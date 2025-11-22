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
        public async Task<IActionResult> Create(
        [Bind("Nombre,Especie,Raza,Edad,EstadoAdopcion,FotoUrl")] Mascota mascota,
        IFormFile? foto)
        {
            if (ModelState.IsValid)
            {
                string wwwRootPath = _hostEnvironment.WebRootPath;
                string mascotaPath = Path.Combine(wwwRootPath, "images", "mascotas");

                if (!Directory.Exists(mascotaPath))
                    Directory.CreateDirectory(mascotaPath);

                if (foto != null)
                {
                    // Nombre original
                    string originalName = Path.GetFileNameWithoutExtension(foto.FileName);
                    string extension = Path.GetExtension(foto.FileName);

                    // Limpieza
                    originalName = Regex.Replace(originalName, @"[^a-zA-Z0-9_-]", "_");

                    // Nombre final
                    string fileName = originalName + extension;
                    string finalPath = Path.Combine(mascotaPath, fileName);

                    int counter = 1;
                    while (System.IO.File.Exists(finalPath))
                    {
                        fileName = $"{originalName}_{counter}{extension}";
                        finalPath = Path.Combine(mascotaPath, fileName);
                        counter++;
                    }

                    // Guardar archivo
                    using (var stream = new FileStream(finalPath, FileMode.Create))
                    {
                        await foto.CopyToAsync(stream);
                    }

                    mascota.FotoUrl = "/images/mascotas/" + fileName;
                }

                // Normalización
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
        public async Task<IActionResult> Edit(int id,
        [Bind("Id,Nombre,Especie,Raza,Edad,EstadoAdopcion,FotoUrl")] Mascota mascota,
        IFormFile? foto)
        {
            if (id != mascota.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                string wwwRootPath = _hostEnvironment.WebRootPath;
                string mascotaPath = Path.Combine(wwwRootPath, "images", "mascotas");

                if (!Directory.Exists(mascotaPath))
                    Directory.CreateDirectory(mascotaPath);

                // Cargar el registro actual
                var mascotaOriginal = await _context.Mascotas.AsNoTracking()
                                     .FirstOrDefaultAsync(m => m.Id == mascota.Id);

                if (mascotaOriginal == null)
                    return NotFound();

                if (foto != null)
                {
                    // 1. Eliminar la imagen anterior
                    if (!string.IsNullOrEmpty(mascotaOriginal.FotoUrl))
                    {
                        string oldFullPath = Path.Combine(wwwRootPath, mascotaOriginal.FotoUrl.TrimStart('/'));

                        if (System.IO.File.Exists(oldFullPath))
                            System.IO.File.Delete(oldFullPath);
                    }

                    // 2. Usar el nombre original limpio
                    string originalName = Path.GetFileNameWithoutExtension(foto.FileName);
                    string extension = Path.GetExtension(foto.FileName);

                    originalName = Regex.Replace(originalName, @"[^a-zA-Z0-9_-]", "_");

                    string fileName = originalName + extension;
                    string finalPath = Path.Combine(mascotaPath, fileName);

                    int counter = 1;
                    while (System.IO.File.Exists(finalPath))
                    {
                        fileName = $"{originalName}_{counter}{extension}";
                        finalPath = Path.Combine(mascotaPath, fileName);
                        counter++;
                    }

                    // 3. Guardar imagen nueva
                    using (var stream = new FileStream(finalPath, FileMode.Create))
                    {
                        await foto.CopyToAsync(stream);
                    }

                    mascota.FotoUrl = "/images/mascotas/" + fileName;
                }
                else
                {
                    // Mantener imagen anterior
                    mascota.FotoUrl = mascotaOriginal.FotoUrl;
                }

                // NORMALIZAR ESTADO
                mascota.EstadoAdopcion = Regex.Replace(
                    mascota.EstadoAdopcion?.Trim() ?? "SIN ADOPTAR",
                    @"\s+",
                    " "
                ).ToUpperInvariant();

                // Guardar
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
