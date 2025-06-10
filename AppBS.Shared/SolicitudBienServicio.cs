using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppBS.Shared
{
  
        public class SolicitudBienServicio
    {
            // Identificador único del bien o servicio
        public int Id { get; set; }

            // Número consecutivo generado automáticamente desde la Aplicación
        public int NumeroConsecutivo { get; set; }

            // Fecha de emisión del bien o servicio (por defecto la fecha actual)
        public DateTime FechaEmision { get; set; } = DateTime.Now;

            
        // Número de ficha asociada al bien o servicio
        public string? NumeroFicha { get; set; }

        // Dependencia o unidad organizativa re0sponsable
        [NotMapped]
        public Dependencia? Dependencia { get; set; }
        public String CodDependencia { get; set; }

        public String NomDependencia { get; set; }

        // Meta asociada al bien o servicio dentro del plan de trabajo
        [NotMapped]
        public String? Meta { get; set; }
        [NotMapped]
        public string? NomMeta { get; set; }
        // Código de la cuenta presupuestaria utilizada
        [NotMapped]
        public string? Cuenta { get; set; }

         


        // Descripción detallada del requerimiento se necesita una columna
            public string DescripcionRequerimiento { get; set; }

        // Objetivo o finalidad del requerimiento se necesita una columna
        public string FinalidadPerseguida { get; set; }

        // Estimación total del costo del bien o servicio se necesita una columna
        public decimal EstimacionTotal { get; set; }

        // Presupuesto disponible para cubrir el costo del bien o servicio se necesita una columna
        public decimal PresupuestoDisponible { get; set; }

        // Indica si la partida específica ha sido asignada se necesita una columna
        public bool PartidaEspecifica { get; set; }


        // Montos disponibles dentro del presupuesto se necesita una columna
         //public decimal MontosDisponibles { get; set; }

        // Recursos de infraestructura asociados al bien o servicio se necesita una columna
        public string RecursosInfraestructura { get; set; }

            // Indica si el bien o servicio corresponde a una obra pública
            public bool ObraPublica { get; set; }

       
            // Encargados de la inspección del bien o servicio
            public string EncargadosInspeccion { get; set; }

            // Comentarios o aclaraciones adicionales
            public string? Observaciones { get; set; }

      
            /* Sección de atributos de firmas */

            // Indica si el solicitante ha firmado la solicitud
            public bool? FirmaSolicitante { get; set; }

            // Indica si el departamento de presupuesto ha firmado
            public bool? FirmaPresupuesto { get; set; }

            // Indica si el área financiera ha firmado
            public bool? FirmaFinanciero { get; set; }


        public int Estado { get; set; }
      
        public int TipoBienServicio { get; set; }

        public string? UsuarioRegistro { get; set; }

        public DateTime? FechaRegistro { get; set; } = DateTime.Now;



    }
}



