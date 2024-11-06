using EmailService;
using Microsoft.AspNetCore.Mvc;

namespace antigal.server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmailController : ControllerBase
    {
        private readonly IEmailSender _emailSender;

        // Inyección de dependencias del servicio de envío de correo
        public EmailController(IEmailSender emailSender)
        {
            _emailSender = emailSender;
        }

        // Acción HTTP GET para enviar un email
        [HttpGet]
        public IActionResult SendTestEmail()
        {
            try
            {
                // Crear un mensaje de correo (ajusta los parámetros de acuerdo a tus necesidades)
                var message = new Message(
                    new string[] { "sdfsdfffasdffsddfsdf@gmail.com" }, // Dirección de destinatario
                    "Test email",                          // Asunto
                    "This is content from our email.", // Cuerpo del mensaje
                    null
                );

                // Enviar el correo
                _emailSender.SendEmail(message);

                // Retornar respuesta de éxito (200 OK)
                return Ok("Email sent successfully.");
            }
            catch (Exception ex)
            {
                // En caso de error, retornar un mensaje de error con status 500
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error sending email: {ex.Message}");
            }
        }

    }
}

