using Microsoft.AspNetCore.Mvc;
using Proyecto.Data;
using Proyecto.Models;
using System.Linq;
using Microsoft.AspNetCore.Http;
using System;

namespace Proyecto.Controllers
{
    public class BlogController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BlogController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Mostrar lista de blogs
        public IActionResult Index()
        {
            var blogs = _context.Blogs
                .OrderByDescending(b => b.FechaPublicacion)
                .ToList();

            return View(blogs);
        }

        // Ver detalle del blog
        public IActionResult Detalle(int id)
        {
            var blog = _context.Blogs.FirstOrDefault(b => b.Id == id);
            if (blog == null)
                return NotFound();

            return View(blog);
        }

        // Mostrar formulario de creación
        public IActionResult Crear()
        {
            var role = HttpContext.Session.GetString("UserRole");

            // Validación de rol
            if (role != "Administrador")
                return RedirectToAction("Index");

            return View();
        }

        // Crear nuevo blog (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Crear(Blog blog)
        {
            var role = HttpContext.Session.GetString("UserRole");

            if (role != "Administrador")
                return RedirectToAction("Index");

            if (ModelState.IsValid)
            {
                blog.FechaPublicacion = DateTime.Now;
                blog.Activo = true; // no se pide en el formulario

                _context.Blogs.Add(blog);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }

            return View(blog);
        }

        // Mostrar formulario de edición
        public IActionResult Editar(int id)
        {
            var role = HttpContext.Session.GetString("UserRole");

            if (role != "Administrador")
                return RedirectToAction("Index");

            var blog = _context.Blogs.Find(id);
            if (blog == null)
                return NotFound();

            return View(blog);
        }

        // Guardar cambios del blog editado
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Editar(Blog blog)
        {
            var role = HttpContext.Session.GetString("UserRole");

            if (role != "Administrador")
                return RedirectToAction("Index");

            if (ModelState.IsValid)
            {
                _context.Blogs.Update(blog);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }

            return View(blog);
        }

        // Eliminar blog
        public IActionResult Eliminar(int id)
        {
            var role = HttpContext.Session.GetString("UserRole");

            if (role != "Administrador")
                return RedirectToAction("Index");

            var blog = _context.Blogs.Find(id);
            if (blog == null)
                return NotFound();

            _context.Blogs.Remove(blog);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }
    }
}
