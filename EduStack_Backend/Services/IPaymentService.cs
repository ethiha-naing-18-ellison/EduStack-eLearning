using EduStack.API.DTOs;

namespace EduStack.API.Services
{
    public interface IPaymentService
    {
        Task<PaymentResponse> CreatePaymentAsync(int userId, CreatePaymentRequest request);
        Task<PaymentDto> GetPaymentByIdAsync(int paymentId);
        Task<List<PaymentDto>> GetUserPaymentsAsync(int userId, int page = 1, int pageSize = 10);
        Task<List<PaymentDto>> GetCoursePaymentsAsync(int courseId, int page = 1, int pageSize = 10);
        Task<bool> UpdatePaymentStatusAsync(int paymentId, UpdatePaymentStatusRequest request);
        Task<bool> ProcessPaymentAsync(int paymentId, string transactionId, string gatewayResponse);
        Task<bool> RefundPaymentAsync(int paymentId, decimal? amount = null);
        Task<List<PaymentDto>> GetPaymentsByStatusAsync(string status, int page = 1, int pageSize = 10);
        Task<decimal> GetTotalRevenueAsync(int? instructorId = null, DateTime? fromDate = null, DateTime? toDate = null);
        Task<List<PaymentDto>> GetRevenueReportAsync(int? instructorId = null, DateTime? fromDate = null, DateTime? toDate = null);
    }
}
