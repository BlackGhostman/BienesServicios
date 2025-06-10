using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppBS.Shared
{
  
        public class BienServicio
    {
      //  [Key]
        public int Id { get; set; } // ID autoincremental

       // [Required]
        public int CodigoClasificacion { get; set; } // Código Clasificación

        //[Required]
       // [StringLength(255)]
        public string NombreClasificacion { get; set; } // Nombre Clasificación

       // [Required]
        public int CodigoIdentificacion { get; set; } // Código de Identificación

        //[Required]
        //[StringLength(255)]
        public string NombreIdentificacion { get; set; } // Nombre Identificación

        //[Required]
       // [StringLength(500)]
        public string DescripcionBienServicio { get; set; } // Descripción del Bien/Servicio

        // Relación con la tabla SolicitudBienServicio
        //[ForeignKey("SolicitudBienServicio")]
        public int IdSolicitudBienServicio { get; set; }

        public decimal Cantidad { get; set; } = 0;
        public decimal MontoUnitario { get; set; } = 0;
        public decimal Subtotal { get; set; } = 0;
        public bool AplicaPresupuesto { get; set; } = true;
        public string Justificacion { get; set; } = string.Empty;

        //  public virtual SolicitudBienServicio SolicitudBienServicio { get; set; }

    }
    }



