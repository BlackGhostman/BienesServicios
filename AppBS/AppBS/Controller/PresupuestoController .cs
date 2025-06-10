using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AppBS.DAO;
using AppBS.Shared;
using AppBS.Services;
using System.Text.Json;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AppBS.Controller
{
    [Route("api/presupuesto")]
    [ApiController]
    public class PresupuestoController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ReporteService _reporteService;

        public PresupuestoController(ApplicationDbContext context, ReporteService reporteService)
        {
            _context = context;
            _reporteService = reporteService;
        }

        // ✅ GET: Obtener todos los presupuestos
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Presupuesto>>> GetPresupuestos()
        {
            return await _context.Presupuesto.ToListAsync();
        }

        // ✅ GET: Obtener un presupuesto por ID
        [HttpGet("{id}")]
        public async Task<ActionResult<Presupuesto>> GetPresupuesto(int id)
        {
            var presupuesto = await _context.Presupuesto.FindAsync(id);
            if (presupuesto == null)
            {
                return NotFound("❌ Presupuesto no encontrado.");
            }
            return presupuesto;
        }

        // ✅ POST: Crear un nuevo presupuesto
        [HttpPost]
        public async Task<ActionResult<Presupuesto>> CreatePresupuesto(Presupuesto presupuesto)
        {
            if (presupuesto == null)
            {
                return BadRequest("❌ Los datos del presupuesto son inválidos.");
            }

            _context.Presupuesto.Add(presupuesto);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPresupuesto), new { id = presupuesto.Id }, presupuesto);
        }

        // ✅ PUT: Actualizar un presupuesto
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePresupuesto(int id, Presupuesto presupuesto)
        {
            if (id != presupuesto.Id)
            {
                return BadRequest("❌ El ID del presupuesto no coincide.");
            }

            _context.Entry(presupuesto).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PresupuestoExists(id))
                {
                    return NotFound("❌ Presupuesto no encontrado.");
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // ✅ DELETE: Eliminar un presupuesto
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePresupuesto(int id)
        {
            var presupuesto = await _context.Presupuesto.FindAsync(id);
            if (presupuesto == null)
            {
                return NotFound("❌ Presupuesto no encontrado.");
            }

            _context.Presupuesto.Remove(presupuesto);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // 📌 Método privado para verificar si un presupuesto existe
        private bool PresupuestoExists(int id)
        {
            return _context.Presupuesto.Any(e => e.Id == id);
        }
    }
}
