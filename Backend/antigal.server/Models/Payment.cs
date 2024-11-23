// File: Models/Payment.cs
using System;
using System.ComponentModel.DataAnnotations;

namespace antigal.server.Models
{
    public class Payment
    {
        [Key]
        public required string PaymentId { get; set; }  // ID del pago proporcionado por Mercado Pago

        public decimal Amount { get; set; }  // Monto del pago
        public required string Status { get; set; }  // Estado del pago ("approved", "pending", "rejected")
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;  // Fecha de creación del pago
        public DateTime? UpdatedAt { get; set; }  // Fecha de última actualización
        public required string UserId { get; set; }  // Usuario asociado al pago
    }
}
