using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppBS.Shared
{
    public class Producto
    {

        public int Id { get; set; } // Identificador del producto

        public string Prod_id { get; set; } // Identificador del producto
        public string Prod_nm { get; set; } = string.Empty; // Nombre del producto

        public int? Linea { get; set; } // linea del producto

        public int Cantidad { get; set; } = 0;
        public decimal MontoUnitario { get; set; } = 0;
        public decimal Subtotal { get; set; } = 0;
        public bool AplicaPresupuesto { get; set; }
        public string? Justificacion { get; set; } = string.Empty;

        [NotMapped]
        public bool Seleccionado { get; set; } // Nueva propiedad para manejar selección
        [NotMapped]
        public bool? Use_yn { get; set; } // Indica si está en uso (Sí/No)
        [NotMapped]
        public DateTime? Reg_dt { get; set; } // Fecha de registro
                                              //public DateTime? ModDt { get; set; } // Fecha de modificación (puede ser nula)

        public int? IdSolicitudBS { get; set; } // Relación con SolicitudBienServicio

        public int TipoPresupuesto { get; set; }

    }
}
