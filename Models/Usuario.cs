using System.ComponentModel.DataAnnotations;
namespace Proyecto.Models
{
    public class Usuario
    {
        public int Id { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Nombre { get; set; }

        [Required]
        public string Rol { get; set; } = "Administrador";

        [Required]
        public string PasswordHash { get; set; }
    }
}
