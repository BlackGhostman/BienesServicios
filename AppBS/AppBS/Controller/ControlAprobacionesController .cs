using AppBS.DAO;
using AppBS.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

[Route("api/controlaprobaciones")]
[ApiController]
public class ControlAprobacionesController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public ControlAprobacionesController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: api/ControlAprobaciones/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<ControlAprobaciones>> GetAprobacion(int id)
    {
        var aprobacion = await _context.ControlAprobaciones.FindAsync(id);

        if (aprobacion == null)
        {
            return NotFound();
        }

        return aprobacion;
    }

    // POST: api/ControlAprobaciones
    [HttpPost]
    public async Task<ActionResult<ControlAprobaciones>> PostAprobacion(ControlAprobaciones aprobacion)
    {
        if (aprobacion == null)
        {
            return BadRequest("❌ Los datos de aprobación son inválidos.");
        }

        await using var transaction = await _context.Database.BeginTransactionAsync(); // Iniciar transacción

        try
        {
            // 1. Registrar la nueva Aprobación en ControlAprobaciones
            aprobacion.FechaRegistro = DateTime.UtcNow; // Fecha automática

            _context.ControlAprobaciones.Add(aprobacion);
            await _context.SaveChangesAsync(); // Guardar primero la aprobación

            // 2. Buscar y actualizar el estado del BienServicio
            var bienServicioExistente = await _context.SolicitudBienServicio
                .FirstOrDefaultAsync(b => b.Id == aprobacion.IdSolicitudBS);

            if (bienServicioExistente == null)
            {
                await transaction.RollbackAsync(); // Deshacer si no existe el bien
                return NotFound($"❌ El BienServicio con el ID {aprobacion.IdSolicitudBS} no fue encontrado.");
            }

            bienServicioExistente.Estado = aprobacion.Estado; // Actualizar estado
            await _context.SaveChangesAsync(); // Guardar cambios en el BienServicio

            // 3. Confirmar la transacción (hacer los cambios permanentes)
            await transaction.CommitAsync();

            // 4. Retornar la nueva aprobación registrada con detalles del BienServicio
            return Ok(new
            {
                Message = "✅ Aprobación registrada y BienServicio actualizado correctamente.",
                Aprobacion = aprobacion,
                EstadoActualizado = bienServicioExistente.Estado
            });
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(); // Si hay error, deshacer todo
            return StatusCode(500, $"❌ Ocurrió un error al procesar la solicitud: {ex.Message}");
        }
    }

    // PUT: api/ControlAprobaciones/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> PutAprobacion(int id, ControlAprobaciones aprobacion)
    {
        if (id != aprobacion.IdSolicitudBS)
        {
            return BadRequest();
        }

        _context.Entry(aprobacion).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!_context.ControlAprobaciones.Any(e => e.IdSolicitudBS == id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        return NoContent();
    }


    // GET: api/ControlAprobaciones/ultimo-rechazo?idSolicitudBS=123&estado=5
    [HttpGet("ultimo-rechazo")]
    public async Task<IActionResult> ObtenerUltimoRechazo([FromQuery] int idSolicitudBS, [FromQuery] int estado)
    {
        try
        {
            // Buscar la última aprobación o rechazo con el estado especificado
            var ultimoRechazo = await _context.ControlAprobaciones
                .Where(ca => ca.IdSolicitudBS == idSolicitudBS && ca.Estado == estado)
                .OrderByDescending(ca => ca.FechaRegistro) // Ordenar por fecha y hora (incluyendo minutos y segundos)
                .FirstOrDefaultAsync();

            if (ultimoRechazo == null)
            {
                return NotFound("No se encontró ningún rechazo con los parámetros especificados.");
            }

            return Ok(ultimoRechazo);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error al obtener el último rechazo: {ex.Message}");
            return StatusCode(500, "Ocurrió un error al consultar los datos.");
        }
    }



}
