// File: Controllers/PaymentController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using antigal.server.Services;

namespace antigal.server.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpPost("create-payment")]
        public async Task<IActionResult> CreatePayment(decimal amount, string title, int quantity)
        {
            var paymentUrl = await _paymentService.CreatePaymentPreferenceAsync(amount, title, quantity);
            return Ok(new { paymentUrl });
        }
    }
}
