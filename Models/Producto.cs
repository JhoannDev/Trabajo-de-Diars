using System.ComponentModel.DataAnnotations.Schema;

namespace Proyecto.Models
{
    public class Producto
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string TipoProducto { get; set; }
        public string Descripcion {  get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal Precio { get; set; }
        public int Stock { get; set; }
        public string? FotoUrl { get; set; }
    }
}
