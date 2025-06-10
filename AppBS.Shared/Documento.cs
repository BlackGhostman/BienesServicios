
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static AppBS.Shared.EnumExtensions;

namespace AppBS.Shared
{
    [Table("Documento")] // Mapea la clase a la tabla "Documento"
    public class Documento
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

        [Column("Id")] // Especifica el nombre de la columna en la BD
        public int Id { get; set; }

        [Required] // NOT NULL en la base de datos
        [Column("TipoDocumento")] // Especifica el tipo de dato exacto
        public int TipoDocumento { get; set; }

        [Column("TipoPersona")] // Permite NULL en la base de datos
        public int? TipoPersona { get; set; }

        [Required]
        [Column("Nombre")]
        public string Nombre { get; set; }

        [Column("Descripcion")] // Puede ser NULL, tamaño máximo
        public string? Descripcion { get; set; }

        [Required] // NOT NULL en la base de datos
        [Column("Contenido")] // Archivo en binario
        public byte[] Contenido { get; set; }

        [Required]
        [Column("TipoMime")]
        public string TipoMime { get; set; }


        [Column("IdSolicitudBS")] // Llave foránea de BienServicio
        public int IdSolicitudBS { get; set; }
    }
}