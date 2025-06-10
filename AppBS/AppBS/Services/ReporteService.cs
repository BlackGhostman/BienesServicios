using System.Collections.Generic;
using System.IO;
using System.Linq;
using AppBS.Shared; // Asegúrate de que tus clases SolicitudBienServicio, Producto, Presupuesto, etc., estén en este namespace
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Globalization;
using System.Threading.Tasks; // Necesario para Task

namespace AppBS.Services
{
    public class ReporteService
    {
        public async Task<byte[]> GenerarPDFBienServicio(SolicitudBienServicio solicitud, List<Producto> productos, List<Presupuesto> presupuestos)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                Document document = new Document(PageSize.A4, 20, 20, 30, 30);
                PdfWriter writer = PdfWriter.GetInstance(document, memoryStream);
                document.Open();

                // 🔹 Agregar imagen del logo
                string imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/logo_curridabat.png");
                if (File.Exists(imagePath))
                {
                    Image logo = Image.GetInstance(imagePath);
                    logo.ScaleToFit(200, 200);
                    logo.Alignment = Image.ALIGN_LEFT;
                    document.Add(logo);
                }

                // 🔹 Título del reporte
                Font tituloFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 14);
                Paragraph titulo = new Paragraph("Reporte de Bienes y Servicios", tituloFont)
                {
                    Alignment = Element.ALIGN_CENTER
                };
                document.Add(titulo);
                document.Add(new Paragraph("\n"));

                // 🔹 Información de la solicitud en 2 columnas
                PdfPTable tablaSolicitud = new PdfPTable(2) { WidthPercentage = 100 };
                tablaSolicitud.SetWidths(new float[] { 30, 70 });

                AgregarCeldaEncabezado(tablaSolicitud, "Número de Solicitud:");
                AgregarCeldaNormal(tablaSolicitud, solicitud.NumeroConsecutivo.ToString());
                AgregarCeldaEncabezado(tablaSolicitud, "Fecha:");
                AgregarCeldaNormal(tablaSolicitud, solicitud.FechaEmision.ToShortDateString());

                AgregarCeldaEncabezado(tablaSolicitud, "Dependencia:");
                // Asumiendo que SolicitudBienServicio tiene una propiedad 'Dependencia' que es un objeto con 'Codigo' y 'Descripcion'
                AgregarCeldaNormal(tablaSolicitud, $"{solicitud.Dependencia?.Codigo} - {solicitud.Dependencia?.Descripcion}");


                AgregarCeldaEncabezado(tablaSolicitud, "Descripción:");
                AgregarCeldaNormal(tablaSolicitud, solicitud.DescripcionRequerimiento);

                document.Add(tablaSolicitud);
                document.Add(new Paragraph("\n"));

                // Variable para almacenar el total general de productos
                decimal totalGeneralProductos = 0;

                // 🔹 Recorrer productos y mostrar presupuestos (Maestro-Detalle)
                foreach (var producto in productos)
                {
                    PdfPTable tablaProducto = new PdfPTable(7) { WidthPercentage = 100 };
                    tablaProducto.SetWidths(new float[] { 10, 20, 30, 10, 20, 20, 15 });

                    // 🔹 Resaltar la tabla de productos con color azul
                    // Esta línea aplicará el fondo azul a TODAS las celdas de tablaProducto que se añadan después,
                    // a menos que la celda tenga su propio BackgroundColor definido (como en AgregarCeldaEncabezadoColor).
                    // Si solo quieres los encabezados en azul, AgregarCeldaEncabezadoColor ya lo hace.
                    // Si quieres las celdas de datos con fondo blanco/transparente, esta línea debería eliminarse o
                    // modificar AgregarCeldaNormal para que establezca un fondo blanco.
                    // tablaProducto.DefaultCell.BackgroundColor = new BaseColor(0, 102, 204); // Comentado para evitar fondo azul en celdas de datos

                    AgregarCeldaEncabezadoColor(tablaProducto, "Linea");
                    AgregarCeldaEncabezadoColor(tablaProducto, "Código");
                    AgregarCeldaEncabezadoColor(tablaProducto, "Producto");
                    AgregarCeldaEncabezadoColor(tablaProducto, "Cantidad");
                    AgregarCeldaEncabezadoColor(tablaProducto, "Precio Unitario");
                    AgregarCeldaEncabezadoColor(tablaProducto, "Sub-Total");
                    AgregarCeldaEncabezadoColor(tablaProducto, "Presupuesto");

                    // Celdas de datos del producto
                    AgregarCeldaNormalConFondoDefinido(tablaProducto, producto.Linea?.ToString() ?? "", null); // null para fondo por defecto de la celda (transparente)
                    AgregarCeldaNormalConFondoDefinido(tablaProducto, producto.Prod_id, null);
                    AgregarCeldaNormalConFondoDefinido(tablaProducto, producto.Prod_nm, null);
                    AgregarCeldaNormalConFondoDefinido(tablaProducto, producto.Cantidad.ToString(), null);
                    AgregarCeldaNormalConFondoDefinido(tablaProducto, FormatearColones(producto.MontoUnitario), null);
                    AgregarCeldaNormalConFondoDefinido(tablaProducto, FormatearColones(producto.Subtotal), null);
                    AgregarCeldaNormalConFondoDefinido(tablaProducto, producto.AplicaPresupuesto ? "Sí" : "No", null);

                    document.Add(tablaProducto);
                    document.Add(new Paragraph("\n"));

                    // Acumular el subtotal del producto al total general
                    totalGeneralProductos += producto.Subtotal;

                    // 🔹 Tabla de Presupuestos (Detalle) - Solo si el producto aplica a presupuesto
                    var presupuestosProducto = presupuestos.Where(p => p.IdBienServicio == producto.Prod_id).ToList();
                    if (producto.AplicaPresupuesto && presupuestosProducto.Any())
                    {
                        PdfPTable tablaPresupuesto = new PdfPTable(5) { WidthPercentage = 100 };
                        tablaPresupuesto.SetWidths(new float[] { 20, 15, 20, 15, 15 });

                        AgregarCeldaEncabezado(tablaPresupuesto, "Cuenta");
                        AgregarCeldaEncabezado(tablaPresupuesto, "Meta");
                        AgregarCeldaEncabezado(tablaPresupuesto, "Descripción");
                        // AgregarCeldaEncabezado(tablaPresupuesto, "Cod. Servicio");
                        AgregarCeldaEncabezado(tablaPresupuesto, "Presupuesto");
                        AgregarCeldaEncabezado(tablaPresupuesto, "A Disponer");

                        foreach (var presupuesto in presupuestosProducto)
                        {
                            // Asumiendo que Presupuesto tiene una propiedad 'Meta' que es un objeto con 'Codigo'
                            AgregarCeldaNormal(tablaPresupuesto, presupuesto.C2cta + "-" + presupuesto.C3cta + "-" + presupuesto.C4cta + "-" + presupuesto.C5cta + "-" + presupuesto.C6cta);
                            AgregarCeldaNormal(tablaPresupuesto, presupuesto.Meta?.Codigo);
                            AgregarCeldaNormal(tablaPresupuesto, presupuesto.Mdet);
                            //   AgregarCeldaNormal(tablaPresupuesto, presupuesto.IdBienServicio);
                            AgregarCeldaNormal(tablaPresupuesto, FormatearColones(presupuesto.Msalfi));
                            AgregarCeldaNormal(tablaPresupuesto, FormatearColones(presupuesto.ADisponer));
                        }

                        document.Add(tablaPresupuesto);
                        document.Add(new Paragraph("\n"));
                    }
                }

                // 🔹 AGREGAR TOTAL DE PRODUCTO ANTES DE LAS FIRMAS
                PdfPTable tablaTotalProducto = new PdfPTable(2) { WidthPercentage = 100 };
                tablaTotalProducto.SetWidths(new float[] { 85, 15 }); // Ajusta anchos si es necesario (etiqueta más ancha, valor más corto)

                PdfPCell celdaEtiquetaTotal = new PdfPCell(new Phrase("Total de Producto/Servicios:", FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10)))
                {
                    HorizontalAlignment = Element.ALIGN_RIGHT,
                    VerticalAlignment = Element.ALIGN_MIDDLE, // Para alinear verticalmente si las celdas tienen alturas diferentes
                    Padding = 5,
                    Border = Rectangle.NO_BORDER
                };
                tablaTotalProducto.AddCell(celdaEtiquetaTotal);

                PdfPCell celdaValorTotal = new PdfPCell(new Phrase(FormatearColones(totalGeneralProductos), FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10)))
                {
                    HorizontalAlignment = Element.ALIGN_RIGHT, // Alinear el monto a la derecha
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    Padding = 5,
                    Border = Rectangle.NO_BORDER
                };
                tablaTotalProducto.AddCell(celdaValorTotal);

                document.Add(tablaTotalProducto);
                document.Add(new Paragraph("\n\n")); // Espacio adicional antes de las firmas


                // 🔹 Agregar sección de firmas
                PdfPTable tablaFirmas = new PdfPTable(4) { WidthPercentage = 100 };
                tablaFirmas.SetWidths(new float[] { 25, 25, 25, 25 });
                tablaFirmas.SpacingBefore = 20f; // Añadir un poco de espacio antes de la tabla de firmas

                // Función para agregar una celda de firma bien alineada (declarada localmente o como método privado de la clase)
                // Para mantenerlo como en tu código original, la declaro localmente.
                // Si la usas en otros métodos, sería mejor hacerla un método privado de la clase.
                void AgregarFirma(string tituloFirma) // Cambiado el nombre del parámetro para evitar confusión
                {
                    PdfPCell celda = new PdfPCell();
                    celda.Border = Rectangle.NO_BORDER;
                    celda.HorizontalAlignment = Element.ALIGN_CENTER;
                    celda.PaddingTop = 10; // Espacio para la línea de firma

                    Paragraph firma = new Paragraph();
                    firma.Add(new Chunk("________________________", FontFactory.GetFont(FontFactory.HELVETICA, 10))); // Línea un poco más delgada
                    firma.Add(new Chunk("\n\n" + tituloFirma, FontFactory.GetFont(FontFactory.HELVETICA, 9, Font.NORMAL))); // Texto un poco más pequeño

                    celda.AddElement(firma);
                    tablaFirmas.AddCell(celda);
                }

                // Agregar las cuatro firmas con formato mejorado
                AgregarFirma("Solicitante");
                AgregarFirma("Presupuesto");
                AgregarFirma("Dirección Financiera/Alcaldía");
                AgregarFirma("Proveeduría");

                document.Add(tablaFirmas);


                document.Close();
                return memoryStream.ToArray();
            }
        }

        // 🔹 Método para formatear los valores en colones con la configuración regional de Costa Rica
        private string FormatearColones(decimal valor)
        {
            CultureInfo culturaPersonalizada = new CultureInfo("es-CR")
            {
                NumberFormat =
                {
                    CurrencySymbol = "₡",       // Definir el símbolo de moneda
                    CurrencyGroupSeparator = ",", // Coma para separar miles
                    CurrencyDecimalSeparator = ".", // Punto para los decimales
                    CurrencyDecimalDigits = 2    // Dos decimales para moneda
                }
            };
            // Usar "C2" para formato de moneda que utiliza la configuración de CultureInfo, incluyendo el símbolo.
            return valor.ToString("C2", culturaPersonalizada);
        }


        // 🔹 Métodos para agregar celdas a las tablas
        private void AgregarCeldaEncabezado(PdfPTable tabla, string texto)
        {
            Font font = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10);
            PdfPCell celda = new PdfPCell(new Phrase(texto, font))
            {
                BackgroundColor = new BaseColor(230, 230, 230), // Color gris claro para destacar
                HorizontalAlignment = Element.ALIGN_LEFT,
                Padding = 5,
                Border = Rectangle.NO_BORDER // Sin bordes para un diseño limpio
            };
            tabla.AddCell(celda);
        }

        private void AgregarCeldaEncabezadoColor(PdfPTable tabla, string texto)
        {
            Font font = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10, BaseColor.WHITE);
            PdfPCell celda = new PdfPCell(new Phrase(texto, font))
            {
                BackgroundColor = new BaseColor(0, 102, 204), // Azul
                HorizontalAlignment = Element.ALIGN_CENTER,
                Padding = 5,
                // Considera agregar bordes si los encabezados deben tenerlos, ej: Border = Rectangle.BOX
            };
            tabla.AddCell(celda);
        }

        // Modifiqué AgregarCeldaNormal para opcionalmente aceptar un color de fondo,
        // y para evitar que DefaultCell.BackgroundColor afecte las celdas de datos si no se desea.
        private void AgregarCeldaNormalConFondoDefinido(PdfPTable tabla, string texto, BaseColor backgroundColor)
        {
            Font font = FontFactory.GetFont(FontFactory.HELVETICA, 10);
            PdfPCell celda = new PdfPCell(new Phrase(texto ?? string.Empty, font)) // Manejo de nulos para 'texto'
            {
                HorizontalAlignment = Element.ALIGN_LEFT,
                Padding = 5,
                Border = Rectangle.NO_BORDER // Sin bordes para diseño limpio
            };
            if (backgroundColor != null)
            {
                celda.BackgroundColor = backgroundColor;
            }
            tabla.AddCell(celda);
        }

        // Tu método AgregarCeldaNormal original, mantenido por si lo usas donde el fondo no es una preocupación.
        private void AgregarCeldaNormal(PdfPTable tabla, string texto)
        {
            Font font = FontFactory.GetFont(FontFactory.HELVETICA, 10);
            PdfPCell celda = new PdfPCell(new Phrase(texto ?? string.Empty, font)) // Manejo de nulos para 'texto'
            {
                HorizontalAlignment = Element.ALIGN_LEFT,
                Padding = 5,
                Border = Rectangle.NO_BORDER // Sin bordes para diseño limpio
            };
            tabla.AddCell(celda);
        }
    }
}