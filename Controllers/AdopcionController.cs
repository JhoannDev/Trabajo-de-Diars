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
            // Mostramos solo SIN ADOPTAR y EN PROCESO
            var query = _context.Mascotas
                .Where(m => m.EstadoAdopcion != "ADOPTADO")
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(filtroBusqueda))
            {
                filtroBusqueda = filtroBusqueda.Trim().ToUpper();

                query = query.Where(m =>
                    m.Nombre.ToUpper().Contains(filtroBusqueda) ||
                    m.Especie.ToUpper().Contains(filtroBusqueda) ||
                    m.Raza.ToUpper().Contains(filtroBusqueda));
            }

            var lista = await query.ToListAsync();

            return View(lista);
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
            // Quitamos los campos que no deben ser validados
            ModelState.Remove("Mascota");
            ModelState.Remove("Estado");

            if (ModelState.IsValid)
            {
                // --- INICIO DE LA NUEVA SOLUCIÓN: TRANSACCIÓN y SQL crudo ---

                // Usamos una transacción para asegurar que ambas tablas 
                // se actualicen (Mascotas y Adopciones) o ninguna lo haga.
                using (var transaction = await _context.Database.BeginTransactionAsync())
                {
                    try
                    {
                        // 1. BUSCAR Y ACTUALIZAR LA MASCOTA (Esto sí lo hace bien EF)
                        var mascota = await _context.Mascotas.FindAsync(adopcion.MascotaId);

                        // Verificamos el estado (con el .ToUpper() que ya arreglamos)
                        if (mascota == null || mascota.EstadoAdopcion.ToUpper() != "SIN ADOPTAR")
                        {
                            ModelState.AddModelError(string.Empty, "Esta mascota ya no está disponible.");
                            return View(adopcion);
                        }

                        // Actualizamos la mascota a "En Proceso"
                        mascota.EstadoAdopcion = "EN PROCESO";
                        _context.Update(mascota);
                        await _context.SaveChangesAsync(); // Guardamos el cambio de la mascota


                        // 2. INSERTAR LA ADOPCIÓN (El bypass de EF)
                        adopcion.FechaPostulacion = DateTime.Now;
                        adopcion.Estado = "Enviado";

                        // ¡Aquí está la magia!
                        // Creamos un comando INSERT manual y (lo más importante)
                        // OMITIMOS la columna [Id]
                        string sql = @"
                    INSERT INTO [Adopciones] 
                    (EmailPostulante, Estado, FechaPostulacion, MascotaId, Motivo, NombrePostulante, TelefonoPostulante)
                    VALUES 
                    (@p1, @p2, @p3, @p4, @p5, @p6, @p7)";

                        // Usamos ExecuteSqlRawAsync para ejecutar el comando
                        // pasando los valores como parámetros para evitar Inyección SQL.
                        await _context.Database.ExecuteSqlRawAsync(sql,
                            new Microsoft.Data.SqlClient.SqlParameter("@p1", adopcion.EmailPostulante),
                            new Microsoft.Data.SqlClient.SqlParameter("@p2", adopcion.Estado),
                            new Microsoft.Data.SqlClient.SqlParameter("@p3", adopcion.FechaPostulacion),
                            new Microsoft.Data.SqlClient.SqlParameter("@p4", adopcion.MascotaId),
                            new Microsoft.Data.SqlClient.SqlParameter("@p5", adopcion.Motivo),
                            new Microsoft.Data.SqlClient.SqlParameter("@p6", adopcion.NombrePostulante),
                            new Microsoft.Data.SqlClient.SqlParameter("@p7", adopcion.TelefonoPostulante)
                        );

                        // 3. Si ambos comandos (Update y Insert) funcionaron,
                        // guardamos la transacción.
                        await transaction.CommitAsync();

                        return RedirectToAction("PostulacionEnviada");
                    }
                    catch (Exception ex)
                    {
                        // Si algo falló, revertimos todo
                        await transaction.RollbackAsync();
                        ModelState.AddModelError(string.Empty, $"Ocurrió un error al guardar: {ex.Message}");
                        return View(adopcion);
                    }
                }
                // --- FIN DE SOLUCIÓN DE POSTULACION---
            }

            // Si el ModelState no fue válido, regresa a la vista
            return View(adopcion);
        }
        public IActionResult PostulacionEnviada()
        {
            return View();
        }
    }
}