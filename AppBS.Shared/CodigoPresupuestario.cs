using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppBS.Shared
{
    public class CodigoPresupuestario
    {
        public int Id { get; set; } // ID autoincremental
        public int IdCuenta { get; set; } // Código de cuenta
        public int IdMeta { get; set; } // Código de meta
        public string Descripcion { get; set; } // Descripción del presupuesto
        public decimal Presupuesto { get; set; } // Monto del presupuesto
        public decimal ADisponer { get; set; } // Monto disponible a disponer
        public string IdBienServicio { get; set; } // Relación con SolicitudBienServicio
        public string? Observaciones { get; set; } // Descripción del presupuesto

    }
}
