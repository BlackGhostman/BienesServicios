using Microsoft.AspNetCore.Mvc;
using System.Xml;
using ServiceReferenceFinancieroContable;
using AppBS.Shared;
using Org.BouncyCastle.Tls;

namespace AppBS.Controller
{
    [ApiController]
    [Route("api/rh")]
    public class RecursosHumanosController : ControllerBase
    {
        private readonly ServiceReferenceRecursosHumano.WS_RecursosHumanosSoapClient _recursosHumanosService;

        public RecursosHumanosController(ServiceReferenceRecursosHumano.WS_RecursosHumanosSoapClient recursosHumanosService)
        {
            _recursosHumanosService = recursosHumanosService;
        }

        [HttpGet("dependencia/{cedula}")]
        public async Task<ActionResult<Dependencia>> ObtenerDependenciaPorCedula(string cedula)
        {
            try
            {
                // Llamar al servicio de Recursos Humanos
                var response = await _recursosHumanosService.ObtenerUnidadesAdministrativasXCedulaAsync(cedula);

                // Verificar si la respuesta es nula
                if (response == null || response.Any1 == null)
                {
                    return NotFound("No se encontró la dependencia para la cédula proporcionada.");
                }

                // Convertir la respuesta a XML
                var xmlElement = response.Any1 as XmlElement;

                if (xmlElement == null)
                {
                    return StatusCode(500, "La respuesta no contiene XML válido.");
                }

                // Imprimir el XML para depuración
                Console.WriteLine("XML recibido:");
                Console.WriteLine(xmlElement.OuterXml);

                // Crear un documento XML
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(xmlElement.OuterXml);

                // Seleccionar el nodo de la dependencia (suponiendo que el nodo es 'DT')
                XmlNode dependenciaNode = xmlDoc.SelectSingleNode("//DocumentElement/DT");

                if (dependenciaNode == null)
                {
                    return NotFound("No se encontró la dependencia en el XML.");
                }

                // Extraer los valores de 'ucod' y 'udes'
                var codigoNode = dependenciaNode["codigo"];
                var nombreNode = dependenciaNode["Unidad_Administrativa"];

                if (codigoNode != null && nombreNode != null)
                {
                    string codigo = codigoNode.InnerText.Trim();
                    string nombre = nombreNode.InnerText.Trim();

                    if (!string.IsNullOrEmpty(codigo) && !string.IsNullOrEmpty(nombre))
                    {
                        // Crear el objeto Dependencia y devolverlo
                        var dependencia = new Dependencia
                        {
                            Codigo = codigo,
                            Descripcion = nombre
                        };

                        return Ok(dependencia);
                    }
                }

                return NotFound("No se encontraron datos válidos para la dependencia.");
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error al obtener la dependencia: {ex.Message}");
                return StatusCode(500, "Ocurrió un error al obtener la dependencia.");
            }
        }





    }

}