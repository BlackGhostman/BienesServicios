using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppBS.Shared
{
  
        public class ControlAprobaciones
    {
        [Key]
        public int Id { get; set; }

        public int IdSolicitudBS { get; set; }

        public string? Observaciones { get; set; }

        public DateTime FechaRegistro { get; set; }

        public string? UsuarioRegistro { get; set; }

        public string? Departamento { get; set; }

        public int Estado { get; set; }

        [NotMapped]
        public int DestinoRechazo { get; set; } 



    }
}



