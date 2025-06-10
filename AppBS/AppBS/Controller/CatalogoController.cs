using Microsoft.AspNetCore.Mvc;
using ServiceReferenceSICOP;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.IO;
using AppBS.Shared;
using System.Xml;
using System.Net.Http;
using System.Xml.Linq;
using System.Dynamic;
using AppBS.SOAP;

namespace AppBS.Controller
{
  
    
    
    
    [Route("api/catalogo")]
    [ApiController]
    public class CatalogoController : ControllerBase
    {
        private readonly CataWebServiceClient _sicopClient;
        private readonly HttpClient _httpClient;


        public CatalogoController(HttpClient httpClient)
        {
            _httpClient = httpClient;

            _sicopClient = new CataWebServiceClient(CataWebServiceClient.EndpointConfiguration.CataWebServiceSoapPort);
        }




        private string EjecutarCurl()
        {
            try
            {
                string soapRequest = @"<soapenv:Envelope xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/""
                  xmlns:cata=""http://www.cata.co.cr""
                  xmlns:java=""java:webservice.vo""
                  xmlns:java1=""java:ws.cata.bean"">
    <soapenv:Header>
        <cata:authHeader>
            <java:UserName>MERLINKUSER02</java:UserName>
            <java:Password>cata!wlsrnr</java:Password>
        </cata:authHeader>
    </soapenv:Header>
    <soapenv:Body>
        <cata:ListadoProductos>
            <cata:cataInfo>
                <java1:Code_id>0</java1:Code_id>
                <java1:Code_type>0</java1:Code_type>
                <java1:From_date>2000-01-01T00:00:00</java1:From_date>
                <java1:To_date>2024-12-01T00:00:00</java1:To_date>
            </cata:cataInfo>
        </cata:ListadoProductos>
    </soapenv:Body>
</soapenv:Envelope>";

                // Guardar en un archivo temporal
                string tempFilePath = Path.Combine(Path.GetTempPath(), "soap_request.xml");
                System.IO.File.WriteAllText(tempFilePath, soapRequest, Encoding.UTF8);

                if (!System.IO.File.Exists(tempFilePath))
                {
                    Console.WriteLine("Archivo SOAP no encontrado: " + tempFilePath);
                    return "Error: Archivo SOAP no encontrado.";
                }

                // Configurar cURL
                var startInfo = new ProcessStartInfo
                {
                    FileName = "curl",
                    Arguments = $"-X POST \"https://www.SICOP.go.cr:8080/CataItemInfoWS/CataItemInfoWService\" " +
                                "-H \"Content-Type: text/xml;charset=UTF-8\" " +
                                "-H \"SOAPAction: ListadoProductos\" " +
                                $"-d @{tempFilePath}",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using (var process = new Process { StartInfo = startInfo })
                {
                    process.Start();
                    string output = process.StandardOutput.ReadToEnd();
                    string error = process.StandardError.ReadToEnd();
                    process.WaitForExit();

                    // Mostrar código de salida
                    int exitCode = process.ExitCode;
                    Console.WriteLine($"Código de salida: {exitCode}");

                    // Eliminar archivo temporal
                    System.IO.File.Delete(tempFilePath);

                    if (exitCode != 0)
                    {
                        Console.WriteLine($"Error en cURL (Código {exitCode}): {error}");
                        return $"Error en cURL (Código {exitCode}): {error}";
                    }

                    // Guardar la respuesta para analizar
                    System.IO.File.WriteAllText("response.xml", output);
                    Console.WriteLine("Respuesta guardada en response.xml");

                    return output;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al ejecutar cURL: {ex.Message}");
                return $"Error al ejecutar cURL: {ex.Message}";
            }
        }



        // Endpoint para obtener productos
        [HttpPost("obtener-productos")]
        public async Task<IActionResult> ObtenerProductos([FromBody] CataProductRequest request)
        {
            // Configura el encabezado de autenticación
            var authHeader = new AuthHeader
            {
                UserName = "MERLINKUSER02", // Reemplaza con tu usuario
                Password = "cata!wlsrnr" // Reemplaza con tu contraseña
            };

            // Crea la solicitud para obtener los productos
            var listadoProductosRequest = new ListadoProductosRequest
            {
                authHeader = authHeader,
                ListadoProductos = new ListadoProductos
                {
                    cataInfo = request
                }
            };

            // Llama al servicio web y obtén la respuesta
            var response = await _sicopClient.ListadoProductosAsync(listadoProductosRequest);

            // Retorna la respuesta del servicio web
            return Ok(response.ListadoProductos);
        }


        private List<Producto> ParsearProductosDesdeXML(string xmlResponse)
        {
            try
            {
                XDocument doc = XDocument.Parse(xmlResponse);

                XNamespace ns = "http://www.cata.co.cr";
                XNamespace javaNs = "java:ws.cata.bean";

                var productos = doc.Descendants(ns + "ListadoProductos")
                                   .Descendants(javaNs + "Prod_list")
                                   .Select(p => new Producto  // Usamos Producto en lugar de tipo anónimo
                                   {
                                       Prod_id = (string?)p.Element(javaNs + "Prod_id") ?? "",
                                       Prod_nm = (string?)p.Element(javaNs + "Prod_nm") ?? "",
                                        Reg_dt = (DateTime?)p.Element(javaNs + "Reg_dt") ?? DateTime.Now,
                                     //  Use_yn = (string?)p.Element(javaNs + "Use_yn") ?? "N"
                                   })
                                   .ToList();

                // Imprimir los productos parseados
             
                return productos;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al parsear XML: {ex.Message}");
                return new List<Producto>(); // Devolvemos una lista vacía en caso de error
            }
        }



        [HttpGet("productos")]
        public async Task<IActionResult> GetListadoProductos()
        {
            try
            {
                string url = "https://www.SICOP.go.cr:8080/CataItemInfoWS/CataItemInfoWService";
                string soapAction = "http://www.cata.co.cr/ListadoProductos";

                // Formatear fechas con "T"
                string fromDate = DateTime.UtcNow.AddYears(-30).ToString("yyyy-MM-ddTHH:mm:ss");
                string toDate = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss");

                //string fromDate = DateTime.UtcNow.AddMonths(-50).ToString("yyyy-MM-ddTHH:mm:ss");
               // string toDate = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss");

                // Construcción del XML con LINQ to XML
                XNamespace soapenv = "http://schemas.xmlsoap.org/soap/envelope/";
                XNamespace cata = "http://www.cata.co.cr";
                XNamespace java = "java:webservice.vo";
                XNamespace java1 = "java:ws.cata.bean";

                XDocument soapRequestXml = new XDocument(
                    new XDeclaration("1.0", "UTF-8", null),
                    new XElement(soapenv + "Envelope",
                        new XAttribute(XNamespace.Xmlns + "soapenv", soapenv),
                        new XAttribute(XNamespace.Xmlns + "cata", cata),
                        new XAttribute(XNamespace.Xmlns + "java", java),
                        new XAttribute(XNamespace.Xmlns + "java1", java1),
                        new XElement(soapenv + "Header",
                            new XElement(cata + "authHeader",
                                new XElement(java + "UserName", "MERLINKUSER02"),
                                new XElement(java + "Password", "cata!wlsrnr")
                            )
                        ),
                        new XElement(soapenv + "Body",
                            new XElement(cata + "ListadoProductos",
                                new XElement(cata + "cataInfo",
                                    new XElement(java1 + "Code_id", "00"),
                                    new XElement(java1 + "Code_type", "2"),
                                    new XElement(java1 + "From_date", fromDate),
                                    new XElement(java1 + "To_date", toDate)
                                )
                            )
                        )
                    )
                );

                // Convertir XML a string
                string soapRequest = soapRequestXml.Declaration + "\n" + soapRequestXml.Root.ToString();

                // Configurar la solicitud HTTP
                var request = new HttpRequestMessage(HttpMethod.Post, url)
                {
                    Content = new StringContent(soapRequest, Encoding.UTF8, "text/xml")
                };
                request.Headers.Add("SOAPAction", soapAction);

                // Enviar la solicitud al servicio SOAP
                HttpResponseMessage response = await _httpClient.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                {
                    return StatusCode((int)response.StatusCode, $"Error en la solicitud SOAP. Código: {response.StatusCode}");
                }

                // Leer la respuesta XML
                string responseContent = await response.Content.ReadAsStringAsync();

                // Convertir XML a lista de productos
                List<Producto> productos = ParsearProductosDesdeXML(responseContent);

                if (productos == null || productos.Count == 0)
                {
                    return NotFound("No se encontraron productos.");
                }

                return Ok(productos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al obtener productos: {ex.Message}");
            }
        }



        [HttpGet("productosXMLBueno")]
        public async Task<IActionResult> GetListadoProductosXMLBueno()
    {
        try
        {
            string url = "https://www.SICOP.go.cr:8080/CataItemInfoWS/CataItemInfoWService"; // URL real del servicio SOAP
            string soapAction = "http://www.cata.co.cr/ListadoProductos"; // Acción SOAP

            // Formatear fechas correctamente con "T" en la hora
            string fromDate = DateTime.UtcNow.AddMonths(-1).ToString("yyyy-MM-ddTHH:mm:ss");
            string toDate = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss");

            // Construir el XML usando XDocument
            XNamespace soapenv = "http://schemas.xmlsoap.org/soap/envelope/";
            XNamespace cata = "http://www.cata.co.cr";
            XNamespace java = "java:webservice.vo";
            XNamespace java1 = "java:ws.cata.bean";

            XDocument soapRequestXml = new XDocument(
                new XDeclaration("1.0", "UTF-8", null),
                new XElement(soapenv + "Envelope",
                    new XAttribute(XNamespace.Xmlns + "soapenv", soapenv),
                    new XAttribute(XNamespace.Xmlns + "cata", cata),
                    new XAttribute(XNamespace.Xmlns + "java", java),
                    new XAttribute(XNamespace.Xmlns + "java1", java1),
                    new XElement(soapenv + "Header",
                        new XElement(cata + "authHeader",
                            new XElement(java + "UserName", "MERLINKUSER02"),
                            new XElement(java + "Password", "cata!wlsrnr")
                        )
                    ),
                    new XElement(soapenv + "Body",
                        new XElement(cata + "ListadoProductos",
                            new XElement(cata + "cataInfo",
                                new XElement(java1 + "Code_id", "00"),
                                new XElement(java1 + "Code_type", "0"),
                                new XElement(java1 + "From_date", fromDate),
                                new XElement(java1 + "To_date", toDate)
                            )
                        )
                    )
                )
            );

                // Convertir XML a string sin comillas incorrectas
                //string soapRequest = soapRequestXml.ToString();
                string soapRequest = soapRequestXml.Declaration + "\n" + soapRequestXml.Root.ToString();


                // Configurar la solicitud HTTP
                var request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = new StringContent(soapRequest, Encoding.UTF8, "text/xml")
            };

            request.Headers.Add("SOAPAction", soapAction);

            // Enviar la solicitud al servicio SOAP
            HttpResponseMessage response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                return StatusCode((int)response.StatusCode, $"Error en la solicitud SOAP. Código: {response.StatusCode}");
            }

            // Leer y devolver la respuesta
            string responseContent = await response.Content.ReadAsStringAsync();
            return Ok(responseContent);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error al obtener productos: {ex.Message}");
        }
    }


   
        [HttpGet("productos1")]
        public async Task<IActionResult> GetListadoProductos1()
        {
            try
            { 
             
                var request = new ListadoProductosRequest

                {

                    authHeader = new AuthHeader { UserName = "MERLINKUSER02", Password = "cata!wlsrnr" },
                    ListadoProductos = new ListadoProductos
                    {
                        cataInfo = new CataProductRequest
                        {
                            Code_id = "0",
                            Code_type = "0",
                            From_date = DateTime.UtcNow.AddMonths(-1).Date, // Último mes en UTC con T00:00:00
                            To_date = DateTime.UtcNow.Date // Fecha actual en UTC con T00:00:00

                        }
                    }
                };

                var response = await _sicopClient.ListadoProductosAsync(request);

                if (response?.ListadoProductos?.Prod_list == null)
                    return NotFound("No se encontraron productos.");

                var productos = response.ListadoProductos.Prod_list.Select(p => new
                {
                    p.Prod_id,
                    p.Prod_nm,
                    p.Reg_dt
                });

                return Ok(productos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al obtener productos: {ex.Message}");
            }
        }
    }
}