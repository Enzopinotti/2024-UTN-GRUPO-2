// File: Repositories/IPaymentRepository.cs
using System.Threading.Tasks;
using antigal.server.Models;  // Asegúrate de que este espacio de nombres esté correcto

namespace antigal.server.Repositories
{
    public interface IPaymentRepository
    {
        Task<Payment> AddPaymentAsync(Payment payment);  // Método para agregar un pago
        Task<bool> UpdatePaymentStatusAsync(string paymentId, string status); // Método para actualizar el estado del pago
    }
}
