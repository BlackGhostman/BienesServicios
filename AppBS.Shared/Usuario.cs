using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppBS.Shared
{
    public class Usuario
    {
        public int Id { get; set; }
        public int IdPersona { get; set; }
        public int TipoPersona { get; set; }  // "Profesor" o "Estudiante"
        public string Correo { get; set; }
        public int? Rol { get; set; }
        public string Clave { get; set; }
        public string? Cedula { get; set; }
    }
}
