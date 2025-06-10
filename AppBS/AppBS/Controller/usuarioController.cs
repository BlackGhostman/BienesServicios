using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AppBS.DAO;
using AppBS.Shared;
using Microsoft.AspNetCore.Identity.Data;

namespace AppBS.Controller
{
    [Route("api/[controller]")]

    [ApiController]
    public class usuarioController : ControllerBase
    {

        private readonly ApplicationDbContext _context;

        public usuarioController(ApplicationDbContext context)
        {

            _context = context;
        }



        [HttpGet("ConexionServidor")]
        public async Task<ActionResult<string>> GetConexionServidor()
        {
            return "Conectado con el servidor";
        }




        [HttpGet("ConexionUsuarios")]
        public async Task<ActionResult<string>> GetConexionUsuarios()
        {
            try
            {
                var respuesta = await _context.Usuario.ToListAsync();
                return "Conectado la base de datos tabla usuarios";
            }
            catch (Exception ex)
            {
                return "Error de conexion con usuarios";
            }

        }


        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequests request)
        {
            var usuario = await _context.Usuario.FirstOrDefaultAsync(u => u.Correo == request.Email);

            if (usuario == null)
            {
                return BadRequest(new { mensaje = "El correo no está registrado." });
            }

            if (usuario.Clave != request.Password)
            {
                return BadRequest(new { mensaje = "La contraseña es incorrecta." });
            }

            // Devolver el usuario y el mensaje de éxito
            return Ok(new
            {
                mensaje = "Inicio de sesión exitoso.",
                usuario = new
                {
                    usuario.Correo,        // // Email
                    usuario.TipoPersona,    // Tipo  de persona
                    usuario.Rol,   // Rol del usuario
                    usuario.Cedula  // Rol del usuario

                }
            });
        
        }




        [HttpGet("loginUserAsync")]
        public async Task<Usuario?> ValidarUsuarioAsync(string correo, string clave)
        {
            // Busca el usuario por correo
            var usuario = await _context.Usuario.FirstOrDefaultAsync(u => u.Correo == correo);

            if (usuario == null)
            {
                // Usuario no existe
                return (null);
            }

            if (usuario.Clave != clave)
            {
                // Contraseña incorrecta
                return (usuario);
            }

            // Usuario válido
            return (usuario);
        }





    }
}

