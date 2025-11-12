using System.ComponentModel.DataAnnotations.Schema;

namespace Proyecto.Models
{
    public class Donacion
    {
        public int Id { get; set; }
        public string NombreDonante { get; set; }
        public string EmailDonante { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal Monto { get; set; }
        public DateTime FechaDonacion { get; set; }
    }
}
