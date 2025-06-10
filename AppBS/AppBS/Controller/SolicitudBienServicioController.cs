using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AppBS.DAO;
using AppBS.Shared;
using Microsoft.AspNetCore.Identity.Data;
using AppBS.Services;
using System.Text.Json;
using AppBS.Shared.DTO;

namespace AppBS.Controller
{
    [Route("api/solicitudbienservicio")]
    [ApiController]
    public class SolicitudBienServicioController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ReporteService _reporteService;

        public SolicitudBienServicioController(ApplicationDbContext context, ReporteService reporteService)
        {
            _context = context;
            _reporteService = reporteService;
        }


        [HttpPost("generarpdf")]
        public async Task<IActionResult> GenerarPDFBienesYServicios([FromBody] GenerarPDFRequest request)
        {
            try
            {
                if (request == null)
                {
                    return BadRequest("❌ La solicitud no contiene datos.");
                }

                // Log para depuración
                var receivedJson = JsonSerializer.Serialize(request, new JsonSerializerOptions { WriteIndented = true });
                Console.WriteLine($"🛠 JSON recibido en el backend: {receivedJson}");

                // Verificar que las listas no sean nulas
                request.Productos ??= new List<Producto>();
                request.Presupuestos ??= new List<Presupuesto>();

                if (request.NuevoBien == null || string.IsNullOrWhiteSpace(request.NuevoBien.DescripcionRequerimiento))
                {
                    return BadRequest("❌ 'NuevoBien' tiene datos incompletos.");
                }

                if (!request.Productos.Any())
                {
                    return BadRequest("❌ No se enviaron productos válidos.");
                }

                if (!request.Presupuestos.Any())
                {
                    return BadRequest("❌ No se enviaron presupuestos válidos.");
                }

                // Generar el PDF
                byte[] pdfBytes = await _reporteService.GenerarPDFBienServicio(
                    request.NuevoBien,
                    request.Productos,
                    request.Presupuestos
                );

                if (pdfBytes == null || pdfBytes.Length == 0)
                {
                    return BadRequest("❌ Error al generar el PDF.");
                }

                return File(pdfBytes, "application/pdf", "SolicitudBienesServicios.pdf");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error en el backend: {ex.Message}");
                return StatusCode(500, $"Error al generar el PDF: {ex.Message}");
            }
        }



        [HttpPatch("modificar/{id}")]
        public async Task<IActionResult> ModificarSolicitudBienServicioAsync(int id, SolicitudBienServicioDTO dto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync(); // Iniciar la transacción

            try
            {
                // Recuperar el BienServicio por su Id
                var bienServicioExistente = await _context.SolicitudBienServicio
                    .FirstOrDefaultAsync(b => b.NumeroConsecutivo == id); // Buscar por Id

                if (bienServicioExistente == null)
                {
                    return NotFound("❌ El BienServicio con el ID proporcionado no fue encontrado.");
                }

                // Validación de los datos
                if (dto.BienServicio == null || dto.Productos == null || dto.Presupuestos == null || dto.Documentos == null)
                {
                    return BadRequest("❌ Los datos proporcionados son inválidos.");
                }

                // Actualizar los campos principales del BienServicio
                // **Actualizar los campos de la SolicitudBienServicio**
                bienServicioExistente.FechaEmision = dto.BienServicio.FechaEmision;
                bienServicioExistente.NumeroFicha = dto.BienServicio.NumeroFicha;
                bienServicioExistente.CodDependencia = dto.BienServicio.CodDependencia;
                bienServicioExistente.NomDependencia = dto.BienServicio.NomDependencia;
                bienServicioExistente.DescripcionRequerimiento = dto.BienServicio.DescripcionRequerimiento;
                bienServicioExistente.FinalidadPerseguida = dto.BienServicio.FinalidadPerseguida;
                bienServicioExistente.EstimacionTotal = dto.BienServicio.EstimacionTotal;
                bienServicioExistente.PresupuestoDisponible = dto.BienServicio.PresupuestoDisponible;
                bienServicioExistente.PartidaEspecifica = dto.BienServicio.PartidaEspecifica;
                bienServicioExistente.RecursosInfraestructura = dto.BienServicio.RecursosInfraestructura;
                bienServicioExistente.ObraPublica = dto.BienServicio.ObraPublica;
                bienServicioExistente.EncargadosInspeccion = dto.BienServicio.EncargadosInspeccion;
                bienServicioExistente.Observaciones = dto.BienServicio.Observaciones;
                bienServicioExistente.FirmaSolicitante = dto.BienServicio.FirmaSolicitante;
                bienServicioExistente.FirmaPresupuesto = dto.BienServicio.FirmaPresupuesto;
                bienServicioExistente.FirmaFinanciero = dto.BienServicio.FirmaFinanciero;
                bienServicioExistente.Estado = dto.BienServicio.Estado;
                bienServicioExistente.TipoBienServicio = dto.BienServicio.TipoBienServicio;
                bienServicioExistente.UsuarioRegistro = dto.BienServicio.UsuarioRegistro;
                bienServicioExistente.FechaRegistro = dto.BienServicio.FechaRegistro;


                // **Actualizar productos manualmente**
                var productosExistentes = await _context.Producto
                    .Where(p => p.IdSolicitudBS == bienServicioExistente.Id)
                    .ToListAsync(); // Obtener productos asociados al BienServicio

                foreach (var productoDto in dto.Productos)
                {
                    var productoExistente = productosExistentes
                        .FirstOrDefault(p => p.Id == productoDto.Id); // Buscar el producto por Id

                    if (productoExistente != null)
                    {
                        // Actualizamos las propiedades necesarias
                        // Actualizamos las propiedades del producto existente
                        productoExistente.Prod_id = productoDto.Prod_id;
                        productoExistente.Prod_nm = productoDto.Prod_nm;
                        productoExistente.Linea = productoDto.Linea;
                        productoExistente.Cantidad = productoDto.Cantidad;
                        productoExistente.MontoUnitario = productoDto.MontoUnitario;
                        productoExistente.Subtotal = productoDto.Subtotal;
                        productoExistente.AplicaPresupuesto = productoDto.AplicaPresupuesto;
                        productoExistente.Justificacion = productoDto.Justificacion;
                        productoExistente.TipoPresupuesto = productoDto.TipoPresupuesto;

                        //    productoExistente.Use_yn = productoDto.Use_yn;
                        //   productoExistente.Reg_dt = productoDto.Reg_dt;

                        // **Actualizar presupuestos manualmente**
                        var presupuestosExistentes = await _context.Presupuesto
                            .Where(p => p.IdProducto == productoExistente.Id)
                            .ToListAsync(); // Obtener presupuestos asociados al BienServicio

                        // Filtrar presupuestos que pertenecen al producto actual antes de iterar
                        var presupuestosProducto = dto.Presupuestos
                            .Where(p => p.IdBienServicio == productoExistente.Prod_id)
                            .ToList();


                        foreach (var presupuestoDto in presupuestosProducto)
                        {
                            var presupuestoExistente = presupuestosExistentes
                                .FirstOrDefault(p => p.Id == presupuestoDto.Id); // Buscar el presupuesto por Id


                            if (presupuestoExistente != null)
                            {
                                // Actualizamos las propiedades necesarias
                                // Actualizamos las propiedades del presupuesto existente
                                presupuestoExistente.Mnum = presupuestoDto.Mnum;
                                presupuestoExistente.Tnum = presupuestoDto.Tnum;
                                presupuestoExistente.C2cta = presupuestoDto.C2cta;
                                presupuestoExistente.C3cta = presupuestoDto.C3cta;
                                presupuestoExistente.C4cta = presupuestoDto.C4cta;
                                presupuestoExistente.C5cta = presupuestoDto.C5cta;
                                presupuestoExistente.C6cta = presupuestoDto.C6cta;
                                presupuestoExistente.Mdet = presupuestoDto.Mdet;
                              //  presupuestoExistente.Mtot = presupuestoDto.Mtot; // Actualizamos el total
                              //  presupuestoExistente.Mejec = presupuestoDto.Mejec;
                             //   presupuestoExistente.Mcomp = presupuestoDto.Mcomp;
                             //   presupuestoExistente.Msalop = presupuestoDto.Msalop;
                                presupuestoExistente.Msalfi = presupuestoDto.Msalfi;
                                presupuestoExistente.MetaCodigo = presupuestoDto.MetaCodigo; // Actualizamos el MetaCodigo

                                // Si tienes valores para las cuentas 7 a 10, se actualizan también
                             //   presupuestoExistente.C7cta = presupuestoDto.C7cta;
                               // presupuestoExistente.C8cta = presupuestoDto.C8cta;
                               // presupuestoExistente.C9cta = presupuestoDto.C9cta;
                               // presupuestoExistente.C10cta = presupuestoDto.C10cta;

                                // Si hay valores que deben ser recalculados o ajustados, puedes asignarlos aquí.
                              //  presupuestoExistente.Mvori = presupuestoDto.Mvori;
                              //  presupuestoExistente.Maume = presupuestoDto.Maume;
                               // presupuestoExistente.Mdism = presupuestoDto.Mdism;

                                // Asegúrate de actualizar el monto disponible si es necesario
                                presupuestoExistente.ADisponer = presupuestoDto.ADisponer;

                                // Asignamos MetaCodigo si es necesario
                                if (presupuestoDto.Meta != null)
                                {
                                    presupuestoExistente.MetaCodigo = presupuestoDto.Meta.Codigo;
                                }
                            }
                            else
                            {
                                // Si el presupuesto no existe, lo agregamos
                                presupuestoDto.MetaCodigo = presupuestoDto.Meta.Codigo;
                                presupuestoDto.IdBienServicio = productoExistente.Prod_id; // Asignamos el Id del BienServicio a cada presupuesto
                                presupuestoDto.IdProducto = productoExistente.Id;
                                _context.Presupuesto.Add(presupuestoDto);
                            }
                        }




                    }
                    else
                    {
                        // Si el producto no existe, lo agregamos
                        productoDto.IdSolicitudBS = bienServicioExistente.Id; // Asignamos el Id del BienServicio a cada producto
                        _context.Producto.Add(productoDto);
                    }
                }

                // **Actualizar documentos manualmente**
                var documentosExistentes = await _context.Documento
                    .Where(d => d.IdSolicitudBS == bienServicioExistente.Id)
                    .ToListAsync(); // Obtener documentos asociados al BienServicio

                foreach (var documentoDto in dto.Documentos)
                {
                    var documentoExistente = documentosExistentes
                        .FirstOrDefault(d => d.Id == documentoDto.Id); // Buscar el documento por Id

                    if (documentoExistente != null)
                    {
                        // Actualizamos las propiedades necesarias
                        // Actualizamos las propiedades del documento existente
                        documentoExistente.TipoDocumento = documentoDto.TipoDocumento;
                        documentoExistente.TipoPersona = documentoDto.TipoPersona;
                        documentoExistente.Nombre = documentoDto.Nombre;
                        documentoExistente.Descripcion = documentoDto.Descripcion;
                        documentoExistente.TipoMime = documentoDto.TipoMime;

                        // Si el contenido es diferente, actualizamos el archivo binario
                        if (documentoDto.Contenido != null && documentoDto.Contenido.Length > 0)
                        {
                            documentoExistente.Contenido = documentoDto.Contenido;
                        }
                    }
                    else
                    {
                        // Si el documento no existe, lo agregamos
                        documentoDto.IdSolicitudBS = bienServicioExistente.Id; // Asignamos el Id del BienServicio a cada documento
                        _context.Documento.Add(documentoDto);
                    }
                }

                // Guardar los cambios en la base de datos
                await _context.SaveChangesAsync();



                /******Se gudar en la bitacora de control de aprobaciones para q quede registro que se ingreso una nueva bienes y servicios y se envia el corroe
                *                 //Se preparan los datos para insertarlos en la tabla control Aprobaciones y Enviar el correo*/

                ControlAprobaciones control = new ControlAprobaciones
                {
                    IdSolicitudBS = dto.BienServicio.Id,
                    FechaRegistro = DateTime.UtcNow,
                    Observaciones = "Se ha modificado la Bienees y Servicios, según las observaciones planteadas",
                    UsuarioRegistro = dto.BienServicio.UsuarioRegistro,
                    Departamento = dto.BienServicio.CodDependencia,
                    Estado = (int)EnumExtensions.EstadoSolicitud.SolicitudCreada
                };


                _context.ControlAprobaciones.Add(control);
                // Guardar el BienServicio primero para que obtenga su Id
                await _context.SaveChangesAsync();




                // Declarar EmailRequest antes del condicional
             /*   EmailRequest emailRequest = new EmailRequest
                {

                    Destinatario = "presupuesto@curridabat.go.cr",  // "presupuesto@curridabat.go.cr Correo de quien envia el correo electronicca.
                    Titulo = "Bienes y Servicios - modificada recientemente:" + " " + dto.BienServicio.NumeroConsecutivo + " -- " + dto.BienServicio.NomDependencia,
                    Cuerpo = "Se ha modificado según observaciones presentadas a la Bienes y Servicios:" + " " + dto.BienServicio.NumeroConsecutivo + " -- " + dto.BienServicio.NomDependencia + "favor ingresar a validar al Sistema"

                };

                // Enviar el correo
                EmailController emailController = new EmailController();
                var resultadoEnvio = await emailController.EnviarCorreo(emailRequest);
             */

                // Confirmar la transacción (hacer los cambios permanentes)
                await transaction.CommitAsync();

                // Retornar el objeto actualizado
                return Ok(bienServicioExistente); // Se devuelve la entidad actualizada
            }
            catch (ArgumentException ex)
            {
                // Si ocurre un error, revertir la transacción
                await transaction.RollbackAsync();

                // Capturar excepciones de validación y retornar un mensaje claro al frontend
                return BadRequest($"❌ Error de validación: {ex.Message}");
            }
            catch (Exception ex)
            {
                // Si ocurre un error, revertir la transacción
                await transaction.RollbackAsync();

                // Capturar cualquier otro tipo de excepción y retornar un mensaje genérico
                return StatusCode(500, $"❌ Ocurrió un error al procesar la solicitud: {ex.Message}");
            }
        }




        [HttpPost("crear")]
        public async Task<IActionResult> CrearSolicitudBienServicioAsync(SolicitudBienServicioDTO dto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync(); // Iniciar la transacción

            try
            {

                // Validación de los datos
                if (dto.BienServicio == null || dto.Productos == null || dto.Presupuestos == null || dto.Documentos == null)
                {
                    return BadRequest("❌ Los datos proporcionados son inválidos.");
                }

                //Generamos el consecutivo automaticamente.

                // Obtener los dos últimos dígitos del año actual (ej. 25 para 2025)
                int añoActual = DateTime.Now.Year % 100;

                // Obtener el total de registros existentes
                int totalRegistros = await _context.SolicitudBienServicio.CountAsync();

                // Sumar 1 al total para generar el nuevo número consecutivo
                int consecutivo = totalRegistros + 1;

                // Formar el número con el formato: AAXXX (ej. 25001, 25123)
                int numeroConsecutivo = (añoActual * 1000) + consecutivo;


                dto.BienServicio.NumeroConsecutivo = numeroConsecutivo;


                // Crear la entidad SolicitudBienServicio
                _context.SolicitudBienServicio.Add(dto.BienServicio);

                // Guardar el BienServicio primero para que obtenga su Id
                await _context.SaveChangesAsync();

                // Asignar el Id del BienServicio a los productos antes de agregar
                foreach (var producto in dto.Productos)
                {
                    producto.IdSolicitudBS = dto.BienServicio.Id; // Asignando el Id del BienServicio a cada producto
                    _context.Producto.Add(producto);
                }

                // Guardar los productos para obtener los Ids generados
                await _context.SaveChangesAsync(); // Esto guarda los productos en la base de datos y les asigna los Ids

                // Relacionar cada presupuesto con su producto utilizando el prd_id
                foreach (var presupuesto in dto.Presupuestos)
                {
                    // Buscar el producto por su prd_id y asignar el IdProducto
                    var productoRelacionado = dto.Productos
                                                 .FirstOrDefault(p => p.Prod_id == presupuesto.IdBienServicio); // Usar prd_id para relacionar

                    if (productoRelacionado != null)
                    {
                        presupuesto.IdProducto = productoRelacionado.Id; // Asignamos el Id del producto al presupuesto
                    }

                    // Asignar el MetaCodigo si existe
                    if (presupuesto.Meta != null)
                    {
                        presupuesto.MetaCodigo = presupuesto.Meta.Codigo;
                    }

                    _context.Presupuesto.Add(presupuesto);
                }

                // Agregar los documentos
                foreach (var documento in dto.Documentos)
                {
                    documento.IdSolicitudBS = dto.BienServicio.Id; // Asignando el Id del BienServicio a cada documento
                    _context.Documento.Add(documento);
                }

                // Guardar los cambios en la base de datos
                await _context.SaveChangesAsync();



                /******Se gudar en la bitacora de control de aprobaciones para q quede registro que se ingreso una nueva bienes y servicios y se envia el corroe
                 *                 //Se preparan los datos para insertarlos en la tabla control Aprobaciones y Enviar el correo*/
                
                ControlAprobaciones control = new ControlAprobaciones
                {
                    IdSolicitudBS = dto.BienServicio.Id,
                    FechaRegistro = DateTime.UtcNow,
                    Observaciones = "Se ha creado la nueva Bienes y Servicios",
                    UsuarioRegistro = dto.BienServicio.UsuarioRegistro,
                    Departamento = dto.BienServicio.CodDependencia,
                    Estado = (int)EnumExtensions.EstadoSolicitud.SolicitudCreada,
                };

             
                _context.ControlAprobaciones.Add(control);
                // Guardar el BienServicio primero para que obtenga su Id
                await _context.SaveChangesAsync();




                // Declarar EmailRequest antes del condicional
              /*  EmailRequest emailRequest = new EmailRequest
                {

                    Destinatario = "presupuesto@curridabat.go.cr",  // "presupuesto@curridabat.go.cr Correo de quien envia el correo electronicca.
                    Titulo = "Bienes y Servicios - Creada recientemente:" + " " + dto.BienServicio.NumeroConsecutivo + " -- " + dto.BienServicio.NomDependencia,
                    Cuerpo = "Se ha ingresado una nueva solicitud de Bienes y Servicios:" + " " + dto.BienServicio.NumeroConsecutivo + " -- " + dto.BienServicio.NomDependencia + " favor ingresar a validar al Sistema"

                };

                // Enviar el correo
                EmailController emailController = new EmailController();
                var resultadoEnvio = await emailController.EnviarCorreo(emailRequest);
                 */


                // Confirmar la transacción (hacer los cambios permanentes)
                await transaction.CommitAsync();

                // Retornar el objeto creado
                return Ok(dto.BienServicio); // Se devuelve la entidad creada
            }
            catch (ArgumentException ex)
            {
                // Si ocurre un error, revertir la transacción
                await transaction.RollbackAsync();

                // Capturar excepciones de validación y retornar un mensaje claro al frontend
                return BadRequest($"❌ Error de validación: {ex.Message}");
            }
            catch (Exception ex)
            {
                // Si ocurre un error, revertir la transacción
                await transaction.RollbackAsync();

                // Capturar cualquier otro tipo de excepción y retornar un mensaje genérico
                return StatusCode(500, $"❌ Ocurrió un error al procesar la solicitud: {ex.Message}");
            }
        }


        /*
        [HttpPost("crear")]
        public async Task<IActionResult> CrearSolicitudBienServicioAsync(SolicitudBienServicioDTO dto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync(); // Iniciar la transacción

            try
            {
                // Validación de los datos
                if (dto.BienServicio == null || dto.Productos == null || dto.Presupuestos == null || dto.Documentos == null)
                {
                    return BadRequest("❌ Los datos proporcionados son inválidos.");
                }

                // Crear la entidad SolicitudBienServicio
                _context.SolicitudBienServicio.Add(dto.BienServicio);

                // Guardar el BienServicio primero para que obtenga su Id
                await _context.SaveChangesAsync();

                // Asignar el Id del BienServicio a los productos antes de agregar
                foreach (var producto in dto.Productos)
                {
                    producto.IdSolicitudBS = dto.BienServicio.Id; // Asignando el Id del BienServicio a cada producto
                    _context.Producto.Add(producto);
                }

                // Agregar los presupuestos
                foreach (var presupuesto in dto.Presupuestos)
                {
                   // presupuesto.IdProducto =
                    // Asignar el MetaCodigo si existe
                    if (presupuesto.Meta != null)
                    {
                        presupuesto.MetaCodigo = presupuesto.Meta.Codigo;
                    }
                    _context.Presupuesto.Add(presupuesto);
                }

                // Agregar los documentos
                foreach (var documento in dto.Documentos)
                {
                    documento.IdSolicitudBS = dto.BienServicio.Id; // Asignando el Id del BienServicio a cada documento
                    _context.Documento.Add(documento);
                }

                // Guardar los cambios en la base de datos
                await _context.SaveChangesAsync();

                // Confirmar la transacción (hacer los cambios permanentes)
                await transaction.CommitAsync();

                // Retornar el objeto creado
                return Ok(dto.BienServicio); // Se devuelve la entidad creada
            }
            catch (ArgumentException ex)
            {
                // Si ocurre un error, revertir la transacción
                await transaction.RollbackAsync();

                // Capturar excepciones de validación y retornar un mensaje claro al frontend
                return BadRequest($"❌ Error de validación: {ex.Message}");
            }
            catch (Exception ex)
            {
                // Si ocurre un error, revertir la transacción
                await transaction.RollbackAsync();

                // Capturar cualquier otro tipo de excepción y retornar un mensaje genérico
                return StatusCode(500, $"❌ Ocurrió un error al procesar la solicitud: {ex.Message}");
            }
        }
        */


                [HttpGet("total-registros")]
        public async Task<IActionResult> ObtenerTotalRegistros()
        {
            try
            {
                // Obtener el total de registros de la tabla de SolicitudBienServicio
                var totalRegistros = await _context.SolicitudBienServicio.CountAsync();

                // Retornar el total de registros al frontend
                return Ok(totalRegistros); // Devuelve el total como un valor entero
            }
            catch (Exception ex)
            {
                // Manejo de excepciones
                Console.WriteLine($"❌ Error al obtener el total de registros: {ex.Message}");
                return StatusCode(500, "Ocurrió un error al obtener el total de registros.");
            }
        }




        [HttpGet("obtener-bien-servicio/{numeroConsecutivo}")]
        public async Task<IActionResult> ObtenerBienServicioConDatos(int numeroConsecutivo)
        {
            try
            {
                // 1. Obtener el BienServicio usando el NumeroConsecutivo
                var bienServicio = await _context.SolicitudBienServicio
                    .FirstOrDefaultAsync(b => b.NumeroConsecutivo == numeroConsecutivo);

                if (bienServicio == null)
                {
                    return NotFound($"No se encontró el BienServicio con el NumeroConsecutivo {numeroConsecutivo}.");
                }

                // 2. Obtener los documentos asociados a este BienServicio
                var documentos = await _context.Documento
                    .Where(d => d.IdSolicitudBS == bienServicio.Id)
                    .ToListAsync();

                // 3. Obtener los productos asociados a este BienServicio
                var productos = await _context.Producto
                    .Where(p => p.IdSolicitudBS == bienServicio.Id)
                    .ToListAsync();



                // 4. Obtener los presupuestos asociados a los productos a través de Prod_id
                // 4. Obtener los presupuestos asociados a los productos a través de Prod_id
                var presupuestos = await _context.Presupuesto
                    .Where(p => productos.Select(prod => prod.Id).Contains(p.IdProducto))
                    .ToListAsync();






                // Asignar CodigoMeta a Meta.Codigo en cada presupuesto recuperado
                foreach (var presupuesto in presupuestos)
                {
                    if (presupuesto.Meta == null)
                    {
                        presupuesto.Meta = new Meta(); // Asegurar que el objeto Meta no sea null
                    }

                    presupuesto.Meta.Codigo = presupuesto.MetaCodigo; // Asignar el valor de CodigoMeta
                }




                // 6. Crear el objeto final con los datos del BienServicio, documentos, productos y cuentas
                var SolicitudBienServicioDTO = new
                {
                    BienServicio = bienServicio,
                    Documentos = documentos,
                    Productos = productos,
                    Presupuestos = presupuestos

                };

                // 7. Retornar los datos en formato JSON
                return Ok(SolicitudBienServicioDTO);
            }
            catch (Exception ex)
            {
                // Manejo de excepciones
                Console.WriteLine($"❌ Error al obtener los datos: {ex.Message}");
                return StatusCode(500, "Ocurrió un error al obtener los datos del BienServicio.");
            }
        }



        [HttpGet("obtener-bien-servicio-estado/{estado}")]
        public async Task<IActionResult> ObtenerBienServicioPorEstado(int estado)
        {
            try
            {
                // Obtener todas las solicitudes que coincidan con el estado proporcionado
                var solicitudes = await _context.SolicitudBienServicio
                    .Where(b => b.Estado == estado)
                    .ToListAsync();

                if (solicitudes == null || !solicitudes.Any())
                {
                    return NotFound($"No se encontraron solicitudes con el estado {EnumExtensions.GetDescription((EnumExtensions.EstadoSolicitud)estado)}.");
                }

                // Retornar solo la lista de bienes y servicios
                return Ok(solicitudes);
            }
            catch (Exception ex)
            {
                // Manejo de excepciones
                Console.WriteLine($"❌ Error al obtener los datos: {ex.Message}");
                return StatusCode(500, "Ocurrió un error al obtener los datos de las solicitudes de BienServicio.");
            }
        }



        [HttpGet("obtener-bien-servicio-estado")]
        public async Task<IActionResult> ObtenerBienServicioPorEstado([FromQuery] int tipoBienServicio, [FromQuery] List<int> estados)
        {
            try
            {
                if (estados == null || !estados.Any())
                {
                    return BadRequest("Debe proporcionar al menos un estado válido.");
                }

                var solicitudes = await _context.SolicitudBienServicio
                    .Where(b => estados.Contains(b.Estado) && b.TipoBienServicio == tipoBienServicio)
                    .ToListAsync();

                if (solicitudes == null || !solicitudes.Any())
                {
                    return NotFound($"No se encontraron solicitudes con el tipo de Bienes y Servicios {EnumExtensions.GetDescription((EnumExtensions.TipoBienServicio)tipoBienServicio)} y los estados proporcionados.");
                }

                return Ok(solicitudes);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error al obtener los datos: {ex.Message}");
                return StatusCode(500, "Ocurrió un error al obtener los datos de las solicitudes de BienServicio.");
            }
        }



        [HttpGet("obtener-bien-servicio-Lista-estado")]
        public async Task<IActionResult> ObtenerBienServicioPorListaEstado([FromQuery] List<int> estados)
        {
            try
            {
                if (estados == null || estados.Count == 0)
                {
                    return BadRequest("Debe proporcionar al menos un estado.");
                }

                var solicitudes = await _context.SolicitudBienServicio
                    .Where(b => estados.Contains(b.Estado))
                    .ToListAsync();

                if (solicitudes == null || !solicitudes.Any())
                {
                    return NotFound("No se encontraron solicitudes con los estados proporcionados.");
                }

                return Ok(solicitudes);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error al obtener los datos: {ex.Message}");
                return StatusCode(500, "Ocurrió un error al obtener los datos de las solicitudes de BienServicio.");
            }
        }


        [HttpGet("obtener-bien-servicio-Lista-estado-usuario")]
        public async Task<IActionResult> ObtenerBienServicioPorListaEstadoYUsuario(
               [FromQuery] List<int> estados,
               [FromQuery] string usuarioRegistro)
        {
            try
            {
                if (estados == null || estados.Count == 0)
                {
                    return BadRequest("Debe proporcionar al menos un estado.");
                }

                if (string.IsNullOrEmpty(usuarioRegistro))
                {
                    return BadRequest("Debe proporcionar el nombre del usuario que registró la solicitud.");
                }

                var solicitudes = await _context.SolicitudBienServicio
                    .Where(b => estados.Contains(b.Estado) )
                    .ToListAsync();

                if (solicitudes == null || !solicitudes.Any())
                {
                    return NotFound($"No se encontraron solicitudes con los estados proporcionados para el usuario '{usuarioRegistro}'.");
                }

                return Ok(solicitudes);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error al obtener los datos: {ex.Message}");
                return StatusCode(500, "Ocurrió un error al obtener los datos de las solicitudes de BienServicio.");
            }
        }





        [HttpPatch("actualizar-estado-bien-servicio")]
        public async Task<IActionResult> ActualizarEstadoBienServicio([FromBody] SolicitudBienServicio solicitudParcial)
        {
            try
            {
                // Buscar la solicitud por NúmeroConsecutivo
                var solicitud = await _context.SolicitudBienServicio
                    .FirstOrDefaultAsync(b => b.NumeroConsecutivo == solicitudParcial.NumeroConsecutivo);

                if (solicitud == null)
                {
                    return NotFound($"No se encontró ninguna solicitud con el ID {solicitudParcial.Id}.");
                }

                // Actualizar solo el campo Estado
                solicitud.Estado = 1;

                // Crear el objeto de correo
                var emailRequest = new EmailRequest
                {
                    Destinatario = $"{solicitudParcial.UsuarioRegistro}, presupuesto@curridabat.go.cr",
                    Titulo = $"Bienes y Servicios - Firmada recientemente: {solicitud.NumeroConsecutivo} -- {solicitud.NomDependencia}",
                    Cuerpo = $"Se ha firmado por parte del solicitante la Bienes y Servicios: {solicitud.NumeroConsecutivo} -- {solicitud.NomDependencia}. Favor ingresar a validar al Sistema."
                };

                // Enviar el correo
                var emailController = new EmailController();
                var resultadoEnvio = await emailController.EnviarCorreo(emailRequest);

                // Guardar cambios
                await _context.SaveChangesAsync();

                return Ok($"✅ El estado de la solicitud con ID {solicitud.Id} se actualizó correctamente a {EnumExtensions.GetDescription((EnumExtensions.EstadoSolicitud)solicitud.Estado)}.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error al actualizar el estado: {ex.Message}");
                return StatusCode(500, "Ocurrió un error al actualizar el estado de la solicitud.");
            }
        }



        /* [HttpPut("actualizar-estado-bien-servicio")]
         public async Task<IActionResult> ActualizarEstadoBienServicio([FromBody] SolicitudBienServicio solicitudActualizada)
         {
             try
             {
                 // Buscar la solicitud por ID
                 var solicitud = await _context.SolicitudBienServicio
                     .FirstOrDefaultAsync(b => b.NumeroConsecutivo == solicitudActualizada.NumeroConsecutivo);

                 if (solicitud == null)
                 {
                     return NotFound($"No se encontró ninguna solicitud con el ID {solicitudActualizada.Id}.");
                 }

                 // Actualizar campos (en este caso, solo estado, pero puedes actualizar otros si deseas)
                 solicitudActualizada.Estado = 1;
                 solicitud.Estado = solicitudActualizada.Estado;
                 // Crear el objeto de correo
                 EmailRequest emailRequest = new EmailRequest
                 {
                     Destinatario = $"{solicitudActualizada.UsuarioRegistro}, presupuesto@curridabat.go.cr",
                     Titulo = "Bienes y Servicios - Firmada recientemente: " + solicitud.NumeroConsecutivo + " -- " + solicitud.NomDependencia,
                     Cuerpo = "Se ha firmado por parte del solicitante la Bienes y Servicios: " + solicitud.NumeroConsecutivo + " -- " + solicitud.NomDependencia + ". Favor ingresar a validar al Sistema."
                 };

                 // Enviar el correo
                 EmailController emailController = new EmailController();
                 var resultadoEnvio = await emailController.EnviarCorreo(emailRequest);

                 // Guardar los cambios
                 await _context.SaveChangesAsync();

                 return Ok($"✅ El estado de la solicitud con ID {solicitud.Id} se actualizó correctamente a {EnumExtensions.GetDescription((EnumExtensions.EstadoSolicitud)solicitud.Estado)}.");
             }
             catch (Exception ex)
             {
                 Console.WriteLine($"❌ Error al actualizar el estado: {ex.Message}");
                 return StatusCode(500, "Ocurrió un error al actualizar el estado de la solicitud.");
             }
         }*/



        /*   // Método para modificar una SolicitudBienServicio existente
           [HttpPut("modificar/{id}")]
           public async Task<SolicitudBienServicio> ModificarSolicitudBienServicioAsync(int id, SolicitudBienServicioDTO dto)
           {
               var solicitudExistente = await _context.SolicitudBienServicio
                   .Include(s => s.Producto)
                   .Include(s => s.Presupuestos)
                   .Include(s => s.Documentos)
                   .FirstOrDefaultAsync(s => s.Id == id);

               if (solicitudExistente == null)
               {
                   throw new ArgumentException("No se encontró la solicitud bien servicio con el ID proporcionado.");
               }

               // Actualizar la entidad SolicitudBienServicio
               solicitudExistente = dto.BienServicio;

               // Actualizar productos
               foreach (var producto in dto.Productos)
               {
                   var productoExistente = await _context.Producto.FindAsync(producto.Id);
                   if (productoExistente != null)
                   {
                       _context.Entry(productoExistente).CurrentValues.SetValues(producto);
                   }
                   else
                   {
                       _context.Producto.Add(producto);
                   }
               }

               // Actualizar presupuestos
               foreach (var presupuesto in dto.Presupuestos)
               {
                   var presupuestoExistente = await _context.Presupuesto.FindAsync(presupuesto.Id);
                   if (presupuestoExistente != null)
                   {
                       _context.Entry(presupuestoExistente).CurrentValues.SetValues(presupuesto);
                   }
                   else
                   {
                       _context.Presupuesto.Add(presupuesto);
                   }
               }

               // Actualizar documentos
               foreach (var documento in dto.Documentos)
               {
                   var documentoExistente = await _context.Documento.FindAsync(documento.Id);
                   if (documentoExistente != null)
                   {
                       _context.Entry(documentoExistente).CurrentValues.SetValues(documento);
                   }
                   else
                   {
                       _context.Documento.Add(documento);
                   }
               }

               // Guardar los cambios en la base de datos
               await _context.SaveChangesAsync();

               return solicitudExistente; // Retornar la solicitud modificada
           }
       }*/


    }
}



