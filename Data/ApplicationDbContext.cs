using Microsoft.EntityFrameworkCore;
using Proyecto.Models;

namespace Proyecto.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Adopcion>()
                .Property(a => a.Id)
                .UseIdentityColumn();
        }

        public DbSet<Producto> Productos { get; set; }
        public DbSet<Mascota> Mascotas { get; set; }
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Donacion> Donaciones { get; set; }
        public DbSet<Adopcion> Adopciones { get; set; }
        public DbSet<Blog> Blogs { get; set; }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<PasswordResetToken> PasswordResetTokens { get; set; }
    }
}
