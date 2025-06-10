using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppBS.Shared
{
    public class GenerarPDFRequest
    {
        public SolicitudBienServicio NuevoBien { get; set; }
        public List<Producto> Productos { get; set; }
        public List<Presupuesto> Presupuestos { get; set; }
    }
}
