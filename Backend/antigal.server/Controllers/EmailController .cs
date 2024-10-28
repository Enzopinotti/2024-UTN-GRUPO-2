using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SendGrid.Helpers.Mail;
using antigal.server.Services;
using antigal.server.Models.Dto;

namespace antigal.server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmailController : ControllerBase
    {
        private readonly IEmailSender _emailSender;

        public EmailController(IEmailSender emailSender)
        {
            _emailSender = emailSender;
        }

        [HttpPost("send-test-email")]
        public async Task<IActionResult> SendTestEmail([FromBody] TestEmailDto emailRequest)
        {
            if (string.IsNullOrWhiteSpace(emailRequest.Email))
            {
                return BadRequest("El campo 'Email' no puede estar vacío.");
            }

            try
            {
                await _emailSender.SendEmailAsync(emailRequest.Email, emailRequest.Subject, emailRequest.Message);
                return Ok("Test email sent successfully.");
            }
            catch (Exception ex)
            {
                // Puedes registrar el error aquí si es necesario
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
