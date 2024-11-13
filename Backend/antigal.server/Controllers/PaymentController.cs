// File: Controllers/PaymentController.cs
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using antigal.server.Services;

namespace antigal.server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        // Endpoint para crear un pago
        [HttpPost("create-payment")]
        public async Task<IActionResult> CreatePayment(decimal amount, string title, int quantity)
        {
            var paymentUrl = await _paymentService.CreatePaymentPreferenceAsync(amount, title, quantity);
            return Ok(new { paymentUrl });
        }

        // Endpoint para recibir notificaciones de Mercado Pago
        [HttpPost("notification")]
        public async Task<IActionResult> ReceiveNotification([FromQuery] string paymentId, [FromQuery] string status)
        {
            var result = await _paymentService.HandlePaymentNotificationAsync(paymentId, status);
            if (result)
                return Ok("Notification processed");
            return BadRequest("Error processing notification");
        }
    }
}
