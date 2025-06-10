using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppBS.Shared
{
    public class Trazabilidad
    {
        public string Observacion { get; set; } = string.Empty;
        public DateTime Fecha { get; set; }
        public string Departamento { get; set; } = string.Empty;
    }
}
