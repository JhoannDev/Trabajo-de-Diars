using Microsoft.AspNetCore.Razor.Hosting;

namespace Proyecto.Models
{
    public class Mascota
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Especie {  get; set; }
        public string Raza {  get; set; }
        public int Edad {  get; set; }
        public string EstadoAdopcion {  get; set; }
        public string? FotoUrl { get; set; }
    }
}
