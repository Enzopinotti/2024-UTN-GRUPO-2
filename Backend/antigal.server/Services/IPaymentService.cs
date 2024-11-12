// File: Services/IPaymentService.cs
using System.Threading.Tasks;

namespace antigal.server.Services
{
    public interface IPaymentService
    {
        Task<string> CreatePaymentPreferenceAsync(decimal amount, string title, int quantity);
        Task<bool> HandlePaymentNotificationAsync(string paymentId, string status); // Para procesar notificaciones IPN
    }
}
