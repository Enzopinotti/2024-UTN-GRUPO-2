// File: Services/IPaymentService.cs

namespace antigal.server.Services
{
    public interface IPaymentService
    {
        Task<string> CreatePaymentPreferenceAsync(decimal amount, string title, int quantity);
    }
}
