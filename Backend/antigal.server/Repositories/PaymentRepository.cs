// File: Repositories/PaymentRepository.cs
using System.Threading.Tasks;
using antigal.server.Data;
using antigal.server.Models;
using Microsoft.EntityFrameworkCore;

namespace antigal.server.Repositories
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly AppDbContext _context;

        public PaymentRepository(AppDbContext context)
        {
            _context = context;
        }

        // Método para agregar un nuevo pago
        public async Task<Payment> AddPaymentAsync(Payment payment)
        {
            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();
            return payment;
        }

        // Método para actualizar el estado de un pago existente
        public async Task<bool> UpdatePaymentStatusAsync(string paymentId, string status)
        {
            var payment = await _context.Payments.FirstOrDefaultAsync(p => p.PaymentId == paymentId);
            if (payment == null)
                return false;

            payment.Status = status;
            payment.UpdatedAt = DateTime.UtcNow;
            _context.Payments.Update(payment);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
