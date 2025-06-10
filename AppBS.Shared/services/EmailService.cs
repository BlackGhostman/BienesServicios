using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace AppBS.Shared.services
{
    public class EmailService
    {
        private readonly string _smtpServer = "smtp.office365.com"; // Servidor SMTP
        private readonly int _smtpPort = 587; // Puerto SMTP (25, 465 o 587 según el proveedor)
        private readonly string _smtpUser = "joaquin.gutierrez@curridabat.go.cr"; // Tu correo de envío
        private readonly string _smtpPass = "Jgvmsd2105$"; // Contraseña del correo

        public async Task EnviarCorreoAsync(string destinatario, string titulo, string cuerpo)
        {
            try
            {
                using (var smtpClient = new SmtpClient(_smtpServer, _smtpPort))
                {
                    smtpClient.Credentials = new NetworkCredential(_smtpUser, _smtpPass);
                    smtpClient.EnableSsl = true; // Habilita SSL/TLS según el servidor

                    var mailMessage = new MailMessage
                    {
                        From = new MailAddress(_smtpUser, "Sistema de Solicitudes"),
                        Subject = titulo,
                        Body = cuerpo,
                        IsBodyHtml = true
                    };

                    mailMessage.To.Add(destinatario); // Agregar destinatario

                    await smtpClient.SendMailAsync(mailMessage); // Enviar correo
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error al enviar el correo: {ex.Message}");
            }
        }

    }
}
