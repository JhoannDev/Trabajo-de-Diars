using System;
using System.ComponentModel.DataAnnotations;

namespace Proyecto.Models
{
    public class Blog
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "El título es obligatorio")]
        [StringLength(150)]
        public string Titulo { get; set; }

        [Required(ErrorMessage = "El contenido es obligatorio")]
        public string Contenido { get; set; }

        [StringLength(355)]
        public string ImagenUrl { get; set; }

        [Required]
        public DateTime FechaPublicacion { get; set; } = DateTime.Now;

        [StringLength(120)]
        public string Autor { get; set; }

        // Estado del blog (Activo o Inactivo)
        public bool Activo { get; set; } = true;
    }
}

