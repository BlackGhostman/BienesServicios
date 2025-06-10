using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppBS.Shared
{
    public class Cuenta
    {
        public int Id { get; set; } //Identificar el presupuesto seleccionado
        public decimal Tnum { get; set; } // Número de presupuesto
        public string C2cta { get; set; } // Código de cuenta 2
        public string C3cta { get; set; } // Código de cuenta 3
        public string C4cta { get; set; } // Código de cuenta 4
        public string C5cta { get; set; } // Código de cuenta 5
        public string C6cta { get; set; } // Código de cuenta 6
        public string Mdet { get; set; }  // Descripción de la meta
    }
}
