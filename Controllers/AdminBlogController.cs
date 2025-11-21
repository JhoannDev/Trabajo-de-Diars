using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proyecto.Data;
using Proyecto.Models;

public class AdminBlogController : Controller
{
    private readonly ApplicationDbContext _context;

    public AdminBlogController(ApplicationDbContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        ViewData["Title"] = "Gestión de Blog";
        var blogs = _context.Blogs.ToList();
        return View(blogs);
    }

    public IActionResult Create()
    {
        ViewData["Title"] = "Nueva Entrada de Blog";
        return View();
    }

    [HttpPost]
    public IActionResult Create(Blog blog)
    {
        if (ModelState.IsValid)
        {
            _context.Blogs.Add(blog);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
        return View(blog);
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
            return NotFound();

        var blog = await _context.Blogs
            .FirstOrDefaultAsync(m => m.Id == id);

        if (blog == null)
            return NotFound();

        return View(blog);
    }

    public IActionResult Edit(int id)
    {
        var blog = _context.Blogs.Find(id);
        if (blog == null) return NotFound();

        return View(blog);
    }

    [HttpPost]
    public IActionResult Edit(Blog blog)
    {
        if (ModelState.IsValid)
        {
            _context.Blogs.Update(blog);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        return View(blog);
    }

    public IActionResult Delete(int id)
    {
        var blog = _context.Blogs.Find(id);
        if (blog == null) return NotFound();

        return View(blog);
    }

    [HttpPost, ActionName("Delete")]
    public IActionResult DeleteConfirmed(int id)
    {
        var blog = _context.Blogs.Find(id);
        if (blog != null)
        {
            _context.Blogs.Remove(blog);
            _context.SaveChanges();
        }
        return RedirectToAction("Index");
    }
}
