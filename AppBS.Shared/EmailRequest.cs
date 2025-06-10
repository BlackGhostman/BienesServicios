using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppBS.Shared
{
    public class EmailRequest
    {
        public string Destinatario { get; set; }
        public string Titulo { get; set; }
        public string Cuerpo { get; set; }
    }
}
