
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AppBS.DAO;
using AppBS.Shared;
using Microsoft.AspNetCore.Identity.Data;
using System.Net.Mail;
using System.Net;

namespace AppBS.Controller
{
    [Route("api/email")]
    [ApiController]
    public class EmailController : ControllerBase
    {


          /***Credenciales ofrecidos por TI******/
        // correo: sistemas.informaticos@curridabat.go.cr
        // Clave: SIadmlocal2024

        
        private readonly string _smtpServer = "smtp.office365.com";
        private readonly int _smtpPort = 587;
        private readonly string _smtpUser = "sistemas.informaticos@curridabat.go.cr";
        private readonly string _smtpPass = "SIadmlocal2024";


        [HttpPost("enviar")]
        public async Task<IActionResult> EnviarCorreo([FromBody] EmailRequest request)
        {
            try
            {
                using (var smtpClient = new SmtpClient(_smtpServer, _smtpPort))
                {
                    smtpClient.Credentials = new NetworkCredential(_smtpUser, _smtpPass);
                    smtpClient.EnableSsl = true; // Activa la seguridad
                    smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                    smtpClient.UseDefaultCredentials = false;
                    var mailMessage = new MailMessage
                    {
                        From = new MailAddress(_smtpUser, "Sistema de Solicitudes"),
                        Subject = request.Titulo,
                        Body = request.Cuerpo,
                        IsBodyHtml = true
                    };

                    mailMessage.To.Add(request.Destinatario);
                    await smtpClient.SendMailAsync(mailMessage);
                }

                return Ok(new { message = "✅ Correo enviado correctamente" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"❌ Error al enviar correo: {ex.Message}" });
            }
        }


        /*
        private readonly string _smtpServer = "smtp.office365.com";
        private readonly int _smtpPort = 587;
        private readonly string _smtpUser = "joaquin.gutierrez@curridabat.go.cr";
        private readonly string _smtpPass = "Jgvmsd2105$";



        [HttpPost("enviar")]
        public async Task<IActionResult> EnviarCorreo([FromBody] EmailRequest request)
        {
            try
            {
                using (var smtpClient = new SmtpClient(_smtpServer, _smtpPort))
                {
                    smtpClient.Credentials = new NetworkCredential(_smtpUser, _smtpPass);
                    smtpClient.EnableSsl = true;

                    var mailMessage = new MailMessage
                    {
                        From = new MailAddress(_smtpUser, "Sistema de Solicitudes"),
                        Subject = request.Titulo,
                        Body = request.Cuerpo,
                        IsBodyHtml = true
                    };

                    mailMessage.To.Add(request.Destinatario);
                    await smtpClient.SendMailAsync(mailMessage);
                }

                return Ok(new { message = "✅ Correo enviado correctamente" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"❌ Error al enviar correo: {ex.Message}" });
            }
        }*/
    }

    /*    private readonly string _smtpServer = "smtp.gmail.com";
            private readonly int _smtpPort = 587;
            private readonly string _smtpUser = "gv.joaquin@gmail.com";  // Cambia esto por tu correo Gmail
            private readonly string _smtpPass = "ecgp gxty eoio uply";  // Usa una contraseña de aplicación si tienes 2FA   
        */


}

