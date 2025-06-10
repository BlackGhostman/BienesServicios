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
        [Route("api/producto")]
        [ApiController]
        public class ProductoController : ControllerBase
        {
            private readonly ApplicationDbContext _context;
            private readonly ReporteService _reporteService;

            public ProductoController(ApplicationDbContext context, ReporteService reporteService)
            {
                _context = context;
                _reporteService = reporteService;
            }

            // ✅ GET: Obtener todos los productos
            [HttpGet]
            public async Task<ActionResult<IEnumerable<Producto>>> GetProductos()
            {
                return await _context.Producto.ToListAsync();
            }

            // ✅ GET: Obtener un producto por ID
            [HttpGet("{id}")]
            public async Task<ActionResult<Producto>> GetProducto(int id)
            {
                var producto = await _context.Producto.FindAsync(id);
                if (producto == null)
                {
                    return NotFound("❌ Producto no encontrado.");
                }
                return producto;
            }

            // ✅ POST: Crear un nuevo producto
            [HttpPost]
            public async Task<ActionResult<Producto>> CreateProducto(Producto producto)
            {
                if (producto == null)
                {
                    return BadRequest("❌ Los datos del producto son inválidos.");
                }

                _context.Producto.Add(producto);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetProducto), new { id = producto.Id }, producto);
            }

            // ✅ PUT: Actualizar un producto
            [HttpPut("{id}")]
            public async Task<IActionResult> UpdateProducto(int id, Producto producto)
            {
                if (id != producto.Id)
                {
                    return BadRequest("❌ El ID del producto no coincide.");
                }

                _context.Entry(producto).State = EntityState.Modified;

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductoExists(id))
                    {
                        return NotFound("❌ Producto no encontrado.");
                    }
                    else
                    {
                        throw;
                    }
                }

                return NoContent();
            }

            // ✅ DELETE: Eliminar un producto
            [HttpDelete("{id}")]
            public async Task<IActionResult> DeleteProducto(int id)
            {
                var producto = await _context.Producto.FindAsync(id);
                if (producto == null)
                {
                    return NotFound("❌ Producto no encontrado.");
                }

                _context.Producto.Remove(producto);
                await _context.SaveChangesAsync();

                return NoContent();
            }

            // 📌 Método privado para verificar si un producto existe
            private bool ProductoExists(int id)
            {
                return _context.Producto.Any(e => e.Id == id);
            }

        
        }
    }
