using System.ComponentModel.DataAnnotations;

namespace EduStack.API.DTOs
{
    public class PaymentDto
    {
        public int Id { get; set; }
        public UserDto User { get; set; } = null!;
        public CourseDto Course { get; set; } = null!;
        public decimal Amount { get; set; }
        public string Currency { get; set; } = string.Empty;
        public string PaymentMethod { get; set; } = string.Empty;
        public string PaymentStatus { get; set; } = string.Empty;
        public string? TransactionId { get; set; }
        public string? GatewayResponse { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class CreatePaymentRequest
    {
        [Required]
        public int CourseId { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal Amount { get; set; }

        [MaxLength(3)]
        public string Currency { get; set; } = "USD";

        [Required]
        [MaxLength(50)]
        public string PaymentMethod { get; set; } = string.Empty;
    }

    public class PaymentResponse
    {
        public int PaymentId { get; set; }
        public string PaymentStatus { get; set; } = string.Empty;
        public string? TransactionId { get; set; }
        public string? PaymentUrl { get; set; }
        public string? GatewayResponse { get; set; }
    }

    public class UpdatePaymentStatusRequest
    {
        [Required]
        public string PaymentStatus { get; set; } = string.Empty;

        public string? TransactionId { get; set; }

        public string? GatewayResponse { get; set; }
    }
}
