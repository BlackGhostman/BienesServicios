using Microsoft.AspNetCore.Mvc;
using System.Xml;
using ServiceReferenceFinancieroContable;
using AppBS.Shared;

namespace AppBS.Controller
{
    [ApiController]
    [Route("api/finanzas")]
    public class FinanzasController : ControllerBase
    {
        private readonly ServiceReferenceFinancieroContable.WSSoapClient _wsClient;

        public FinanzasController(ServiceReferenceFinancieroContable.WSSoapClient wsClient)
        {
            _wsClient = wsClient;
        }


        [HttpGet("unidades")]
        public async Task<ActionResult<List<Unidad>>> GetUnidadesAdministrativas()
        {
            try
            {
                var response = await _wsClient.ConsultaUnidadesAdministrativasAsync();

                // Verificar que la respuesta no es nula
                if (response.Any1 == null)
                {
                    return NotFound("No se encontraron datos.");
                }

                var xmlElement = response.Any1 as XmlElement;

                if (xmlElement == null)
                {
                    return StatusCode(500, "La respuesta no contiene XML válido.");
                }

                // Imprimir el XML recibido para depuración
                Console.WriteLine("XML recibido:");
                Console.WriteLine(xmlElement.OuterXml);

                var unidades = new List<Unidad>();

                // Cargar el XML de la respuesta
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(xmlElement.OuterXml);

                // Seleccionar nodos 'DT' dentro de 'DocumentElement'
                XmlNodeList dtNodes = xmlDoc.SelectNodes("//DocumentElement/DT");

                if (dtNodes == null || dtNodes.Count == 0)
                {
                    return NotFound("No se encontraron unidades administrativas.");
                }

                // Recorrer los nodos 'DT' y extraer información
                foreach (XmlNode dtNode in dtNodes)
                {
                    var codigoNode = dtNode["ucod"];
                    var nombreNode = dtNode["udes"];

                    if (codigoNode != null && nombreNode != null)
                    {
                        string codigo = codigoNode.InnerText.Trim();
                        string nombre = nombreNode.InnerText.Trim();

                        // Asegurarse de que ambos valores no estén vacíos
                        if (!string.IsNullOrEmpty(codigo) && !string.IsNullOrEmpty(nombre))
                        {
                            unidades.Add(new Unidad
                            {
                                Ucod = codigo,
                                Udes = nombre
                            });
                        }
                    }
                }

                return Ok(unidades);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error al consultar las unidades administrativas: {ex.Message}");
                return StatusCode(500, "Ocurrió un error al obtener las unidades administrativas.");
            }
        }



        [HttpGet("metas/{unidad}")]
        public async Task<ActionResult<List<Meta>>> GetMetasPorUnidad(string unidad)
        {
            try
            {
                var response = await _wsClient.ConsultaMetasxUnidadAsync(unidad);
                if (response.Any1 == null) return NotFound("No se encontraron metas para la unidad especificada.");

                var xmlElement = response.Any1 as XmlElement;
                if (xmlElement == null) return StatusCode(500, "La respuesta no contiene XML válido.");

                var metas = new List<Meta>();
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(xmlElement.OuterXml);
                XmlNodeList metaNodes = xmlDoc.SelectNodes("//DocumentElement/DT");


                if (metaNodes == null || metaNodes.Count == 0)
                {
                    return NotFound("No se encontraron unidades administrativas.");
                }

                // Recorrer los nodos 'DT' y extraer información
                foreach (XmlNode dtNode in metaNodes)
                {
                    var codigoNode = dtNode["mnum"];
                    var descripcionNode = dtNode["mdes"];

                    if (codigoNode != null && descripcionNode != null)
                    {
                        string codigo = codigoNode.InnerText.Trim();
                        string descripcion = descripcionNode.InnerText.Trim();

                        // Asegurarse de que ambos valores no estén vacíos
                        if (!string.IsNullOrEmpty(codigo) && !string.IsNullOrEmpty(descripcion))
                        {
                            metas.Add(new Meta { Codigo = codigo, Descripcion = descripcion });
                        }
                    }
                }
                return Ok(metas);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error al consultar las metas por unidad: {ex.Message}");
                return StatusCode(500, "Ocurrió un error al obtener las metas.");
            }
        }



        [HttpGet("presupuesto/{meta}")]
        public async Task<ActionResult<List<Cuenta>>> GetPresupuestoPorMeta(string meta)
        {
            try
            {
                var response = await _wsClient.ConsultarPresupuestoXMetaAsync(meta);
                if (response.Any1 == null) return NotFound("No se encontró presupuesto para la meta especificada.");

                var xmlElement = response.Any1 as XmlElement;
                if (xmlElement == null) return StatusCode(500, "La respuesta no contiene XML válido.");

                var presupuestos = new List<Cuenta>();
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(xmlElement.OuterXml);
                XmlNodeList presupuestoNodes = xmlDoc.SelectNodes("//DocumentElement/DT");

                if (presupuestoNodes == null || presupuestoNodes.Count == 0)
                {
                    return NotFound("No se encontraron registros de presupuesto.");
                }

                int contador = 1; // Inicializa el contador

                // Recorrer los nodos 'DT' y extraer información
                foreach (XmlNode dtNode in presupuestoNodes)
                {
                    var tnumNode = dtNode["tnum"];
                    var c2ctaNode = dtNode["c2cta"];
                    var c3ctaNode = dtNode["c3cta"];
                    var c4ctaNode = dtNode["c4cta"];
                    var c5ctaNode = dtNode["c5cta"];
                    var c6ctaNode = dtNode["c6cta"];
                    var mdetNode = dtNode["mdet"];

                    decimal tnum = tnumNode != null && decimal.TryParse(tnumNode.InnerText.Trim(), out var parsedTnum) ? parsedTnum : 0;
                    string c2cta = c2ctaNode?.InnerText.Trim() ?? string.Empty;
                    string c3cta = c3ctaNode?.InnerText.Trim() ?? string.Empty;
                    string c4cta = c4ctaNode?.InnerText.Trim() ?? string.Empty;
                    string c5cta = c5ctaNode?.InnerText.Trim() ?? string.Empty;
                    string c6cta = c6ctaNode?.InnerText.Trim() ?? string.Empty;
                    string mdet = mdetNode?.InnerText.Trim() ?? string.Empty;

                    // Asegurarse de que los valores sean válidos antes de agregarlos
                  /*  presupuestos.Add(new Presupuesto
                    {
                        Tnum = tnum,
                        C2cta = c2cta,
                        C3cta = c3cta,
                        C4cta = c4cta,
                        C5cta = c5cta,
                        C6cta = c6cta,
                        Mdet = mdet
                    });*/

                    presupuestos.Add(new Cuenta
                    {
                        Id = contador++, // Asigna el valor y luego lo incrementa
                        Tnum = tnum,
                        C2cta = c2cta,
                        C3cta = c3cta,
                        C4cta = c4cta,
                        C5cta = c5cta,
                        C6cta = c6cta,
                        Mdet = mdet
                    });
                }
                return Ok(presupuestos);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error al consultar el presupuesto por meta: {ex.Message}");
                return StatusCode(500, "Ocurrió un error al obtener el presupuesto.");
            }
        }


        [HttpGet("presupuestoPorMetaYPresupuesto/{meta}/{cuenta}")]
        public async Task<ActionResult<List<Presupuesto>>> GetPresupuestoPorMetaYPresupuesto(string meta, string cuenta)
        {
            try
            {
                // Dividir el código de presupuesto en partes
                var presupuestoParts = cuenta.Split('-');
                if (presupuestoParts.Length < 5) return BadRequest("El código de presupuesto es inválido.");

                string C2CTA = presupuestoParts[0];
                string C3CTA = presupuestoParts[1];
                string C4CTA = presupuestoParts[2];
                string C5CTA = presupuestoParts[3];
                string C6CTA = presupuestoParts[4];
                string C7CTA = string.Empty;
                string C8CTA = string.Empty;
                string C9CTA = string.Empty;
                string C10CTA = string.Empty;

                // Llamar al WS con los parámetros desglosados
                var response = await _wsClient.ConsultarPresupuestoXCodigoPresupuestarioAsync(
                    meta, C2CTA, C3CTA, C4CTA, C5CTA, C6CTA, C7CTA, C8CTA, C9CTA, C10CTA
                );

                if (response.Any1 == null) return NotFound("No se encontró presupuesto para los parámetros especificados.");

                var xmlElement = response.Any1 as XmlElement;
                if (xmlElement == null) return StatusCode(500, "La respuesta no contiene XML válido.");

                var presupuestos = new List<Presupuesto>();
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(xmlElement.OuterXml);
                XmlNodeList presupuestoNodes = xmlDoc.SelectNodes("//DocumentElement/DT");

                if (presupuestoNodes == null || presupuestoNodes.Count == 0)
                {
                    return NotFound("No se encontraron registros de presupuesto.");
                }

                // Recorrer los nodos 'DT' y extraer información
                foreach (XmlNode dtNode in presupuestoNodes)
                {
                    var tnumNode = dtNode["tnum"];
                    var mdetNode = dtNode["mdet"];
                    var MsalfiNode = dtNode["msalfi"];
                    

                    decimal tnum = tnumNode != null && decimal.TryParse(tnumNode.InnerText.Trim(), out var parsedTnum) ? parsedTnum : 0;
                    string mdet = mdetNode?.InnerText.Trim() ?? string.Empty;
                    decimal Msalfi = MsalfiNode != null && decimal.TryParse(MsalfiNode.InnerText.Trim(), out var parsedMsalfiNode) ? parsedMsalfiNode : 0;

                    // Agregar el presupuesto a la lista
                    presupuestos.Add(new Presupuesto
                    {
                        Tnum = tnum,
                        C2cta = C2CTA,
                        C3cta = C3CTA,
                        C4cta = C4CTA,
                        C5cta = C5CTA,
                        C6cta = C6CTA,
                        Mdet = mdet,
                        Msalfi = Msalfi
                    });
                }
                return Ok(presupuestos);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error al consultar el presupuesto por meta y presupuesto: {ex.Message}");
                return StatusCode(500, "Ocurrió un error al obtener el presupuesto.");
            }
        }




    }
}
