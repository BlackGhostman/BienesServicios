using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppBS.Shared.DTO
{
    public class SolicitudBienServicioDTO
    {
        // Propiedades de SolicitudBienServicio

        // Propiedades de Producto
        public SolicitudBienServicio BienServicio { get; set; } = new SolicitudBienServicio();


        public List<Producto> Productos { get; set; } = new List<Producto>();

        // Propiedades de Presupuesto
        public List<Presupuesto> Presupuestos { get; set; } = new List<Presupuesto>();

        // Propiedades de Documento
        public List<Documento> Documentos { get; set; } = new List<Documento>();
    }
}
