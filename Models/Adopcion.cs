using System.ComponentModel.DataAnnotations;
    
namespace Proyecto.Models
{
        public class Adopcion
        {
            public int Id { get; set; }

            [Display(Name = "Estado de la Postulación")]
            public string Estado { get; set; }
            public DateTime FechaPostulacion { get; set; }
            public int MascotaId { get; set; }
            public Mascota Mascota { get; set; }

            [Required(ErrorMessage = "Tu nombre es obligatorio")]
            [Display(Name = "Nombre Completo")]
            public string NombrePostulante { get; set; }

            [Required(ErrorMessage = "Tu email es obligatorio")]
            [EmailAddress]
            [Display(Name = "Correo Electrónico")]
            public string EmailPostulante { get; set; }

            [Required(ErrorMessage = "Tu teléfono es obligatorio")]
            [Display(Name = "Teléfono de Contacto")]
            public string TelefonoPostulante { get; set; }

            [Display(Name = "¿Por qué deseas adoptar a esta mascota?")]
            public string Motivo { get; set; }
        }
}
