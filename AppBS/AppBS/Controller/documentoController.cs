using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AppBS.DAO;
using AppBS.Shared;
using AppBS.Client.Pages;


namespace AppBS.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class documentoController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public documentoController(ApplicationDbContext context)
        {

            _context = context;
        }

        // Endpoint para eliminar un estudiante y sus documentos
        [HttpDelete("eliminarDocumento")]
        public async Task<IActionResult> EliminarDocumento(int id)
        {

            // Buscar el documento a eliminar
            var documento = await _context.Documento
                .FirstOrDefaultAsync(d => d.Id == id);

            // Si no se encuentra el documento, devolver NotFound
            if (documento == null)
            {
                return NotFound("Documento no encontrado.");
            }

            // Eliminar el documento
            _context.Documento.Remove(documento);

            // Guardar los cambios en la base de datos
            await _context.SaveChangesAsync();

            return NoContent(); // Indica que la eliminación fue exitosa
        }



        [HttpPost("crearDocumento")]
        public async Task<ActionResult<String>> CrearDocumento(Documento documento)
        {
            try
            {

                //Primero se agrega el estudiante y luego se agregan los documentos.
                if (documento == null)
                {
                    return BadRequest("Datos de son documento incompletos.");
                }

                    _context.Documento.Add(documento);
                    await _context.SaveChangesAsync();
                
                return Ok("Documento creado exitosamente");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error al agregar un documento, contacte al administrador. Detalles: " + ex.Message);
            }
        }




        [HttpPost("guardarDocumentos")]
        public async Task<ActionResult<string>> GuardarDocumentos(List<Documento> documentos)
        {
            int idDocumento=0;

            if (documentos == null || !documentos.Any())
                return BadRequest("No se recibieron documentos.");

            try
            {
                foreach (var doc in documentos)
                {
                    if (doc.Id > 0)
                    {
                        idDocumento = doc.IdSolicitudBS;
                        // Buscar el documento existente
                        var existente = await _context.Documento.FindAsync(doc.Id);
                        if (existente != null)
                        {
                            // Modificar campos deseados
                            _context.Entry(existente).CurrentValues.SetValues(doc);
                        }
                
                    }
                    else
                    {
                        // Es un documento nuevo
                        doc.IdSolicitudBS = idDocumento; // Asegurarse de que el IdSolicitudBS esté asignado
                        _context.Documento.Add(doc);
                    }
                }

                await _context.SaveChangesAsync();
                return Ok("Documentos procesados correctamente.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al procesar documentos: {ex.Message}");
            }
        }





        // Endpoint para obtener documentos por IdPersona
        [HttpGet("obtenerDocumentosPorIdPersona")]
        public async Task<ActionResult<List<SolicitudBienServicio>>> ObtenerDocumentosPorIdPersona(int idBienServicio)
        {
            // Obtener los documentos asociados a IdPersona
            var documentos = await _context.Documento
                .Where(d => d.IdSolicitudBS == idBienServicio)
                .ToListAsync();

            // Verificar si se encontraron documentos
            if (documentos == null || !documentos.Any())
            {
                return NotFound("No se encontraron documentos para esta persona.");
            }

            // Devolver la lista de documentos
            return Ok(documentos);
        }


     



    }

}
