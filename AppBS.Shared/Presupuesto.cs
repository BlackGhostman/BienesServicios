using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppBS.Shared
{

    [Table("Presupuesto")]
    public class Presupuesto
    {
        [Key]
        public int Id { get; set; } // Identificador del presupuesto

        public string? Mnum { get; set; } // Número de meta
        public decimal? Tnum { get; set; } // Número de transacción
        public string C2cta { get; set; } // Cuenta 2
        public string C3cta { get; set; } // Cuenta 3
        public string C4cta { get; set; } // Cuenta 4
        public string C5cta { get; set; } // Cuenta 5
        public string C6cta { get; set; } // Cuenta 6

        [NotMapped]
        public string? C7cta { get; set; } // Cuenta 7 (puede ser null)
        [NotMapped]
        public string? C8cta { get; set; } // Cuenta 8 (puede ser null)
        [NotMapped]
        public string?   C9cta { get; set; } // Cuenta 9 (puede ser null)
        [NotMapped]
        public string? C10cta { get; set; } // Cuenta 10 (puede ser null)
        

        public string? Mdet { get; set; } // Descripción del presupuesto
        [NotMapped]
        public decimal? Mvori { get; set; } // Monto original
        [NotMapped]
        public decimal? Maume { get; set; } // Aumento
        [NotMapped]
        public decimal? Mdism { get; set; } // Disminución
        [NotMapped]
        public decimal? Mtot { get; set; } // Total
        [NotMapped]
        public decimal? Mejec { get; set; } // Ejecutado
        [NotMapped]
        public decimal? Mcomp { get; set; } // Comprometido
        [NotMapped]
        public decimal? Msalop { get; set; } // Saldo operativo
        public decimal Msalfi { get; set; } // Saldo final

        [NotMapped]
        public Meta? Meta { get; set; } // Meta


        public String? MetaCodigo { get; set; } // Meta

        public decimal ADisponer { get; set; } // Saldo a dispone

        public string? IdBienServicio { get; set; } // Relación con SolicitudBienServicio

        public int IdProducto { get; set; } // Relación con Productos


    }

}
