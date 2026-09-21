// File: Repositories/IPaymentRepository.cs
using antigal.server.Models;

namespace antigal.server.Repositories
{
    public interface IPaymentRepository
    {
        Task<Payment> AddPaymentAsync(Payment payment);
    }
}
