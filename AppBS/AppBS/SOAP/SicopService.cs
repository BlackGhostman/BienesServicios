using System.Text;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using ServiceReferenceSICOP;

namespace AppBS.SOAP
{
    public class SicopService
    {
        private readonly CataWebServiceClient _client;

        public SicopService()
        {
            // Configura el cliente del servicio web de SICOP
            _client = new CataWebServiceClient();
        }

        public async Task<CataProductResponse> GetProductosAsync(CataProductRequest request)
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
            var response = await _client.ListadoProductosAsync(listadoProductosRequest);

            return response.ListadoProductos;
        }
    }
}
