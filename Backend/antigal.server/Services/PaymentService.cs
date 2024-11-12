// File: Services/PaymentService.cs
using Microsoft.AspNetCore.Http;
using antigal.server.Repositories;
using MercadoPago.Client.Preference;
using MercadoPago.Resource.Preference;
using System.Threading.Tasks;
using antigal.server.Models;
using antigal.server.Services;

public class PaymentService : IPaymentService
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public PaymentService(IPaymentRepository paymentRepository, IHttpContextAccessor httpContextAccessor)
    {
        _paymentRepository = paymentRepository;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<string> CreatePaymentPreferenceAsync(decimal amount, string title, int quantity)
    {
        var userId = _httpContextAccessor.HttpContext?.User?.Identity?.Name; // Esto asume que el UserId está en el nombre del usuario

        if (string.IsNullOrEmpty(userId))
        {
            // Maneja el caso si el usuario no está autenticado
            throw new UnauthorizedAccessException("Usuario no autenticado.");
        }

        var request = new PreferenceRequest
        {
            Items = new List<PreferenceItemRequest>
            {
                new PreferenceItemRequest
                {
                    Title = title,
                    Quantity = quantity,
                    CurrencyId = "ARS",
                    UnitPrice = amount
                }
            },
            BackUrls = new PreferenceBackUrlsRequest
            {
                Success = "https://tu-sitio.com/success",
                Failure = "https://tu-sitio.com/failure",
                Pending = "https://tu-sitio.com/pending"
            },
            AutoReturn = "approved"
        };

        var client = new PreferenceClient();
        var preference = await client.CreateAsync(request);

        // Guardar el pago en la base de datos
        var payment = new Payment
        {
            UserId = userId, // Aquí asignas el UserId obtenido
            PaymentId = preference.Id,
            Amount = amount,
            Status = "pending"  // Estado inicial
        };
        await _paymentRepository.AddPaymentAsync(payment);

        return preference.InitPoint; // URL para realizar el pago en Mercado Pago
    }

    public async Task<bool> HandlePaymentNotificationAsync(string paymentId, string status)
    {
        // Actualizar el estado del pago en la base de datos
        return await _paymentRepository.UpdatePaymentStatusAsync(paymentId, status);
    }
}
