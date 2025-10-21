using EduStack.API.Data;
using EduStack.API.DTOs;
using EduStack.API.Models;
using Microsoft.EntityFrameworkCore;

namespace EduStack.API.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly EduStackDbContext _context;
        private readonly ILogger<PaymentService> _logger;

        public PaymentService(EduStackDbContext context, ILogger<PaymentService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<PaymentResponse> CreatePaymentAsync(int userId, CreatePaymentRequest request)
        {
            var course = await _context.Courses.FindAsync(request.CourseId);
            if (course == null)
                throw new KeyNotFoundException("Course not found");

            var payment = new Payment
            {
                UserId = userId,
                CourseId = request.CourseId,
                Amount = request.Amount,
                Currency = request.Currency,
                PaymentMethod = request.PaymentMethod,
                PaymentStatus = "pending",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();

            return new PaymentResponse
            {
                PaymentId = payment.Id,
                PaymentStatus = payment.PaymentStatus,
                TransactionId = payment.TransactionId,
                PaymentUrl = $"https://payment-gateway.com/pay/{payment.Id}", // Mock URL
                GatewayResponse = payment.GatewayResponse
            };
        }

        public async Task<PaymentDto> GetPaymentByIdAsync(int paymentId)
        {
            var payment = await _context.Payments
                .Include(p => p.User)
                .Include(p => p.Course)
                .FirstOrDefaultAsync(p => p.Id == paymentId);

            if (payment == null)
                return null!;

            return MapToPaymentDto(payment);
        }

        public async Task<List<PaymentDto>> GetUserPaymentsAsync(int userId, int page = 1, int pageSize = 10)
        {
            var payments = await _context.Payments
                .Include(p => p.User)
                .Include(p => p.Course)
                .Where(p => p.UserId == userId)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return payments.Select(MapToPaymentDto).ToList();
        }

        public async Task<List<PaymentDto>> GetCoursePaymentsAsync(int courseId, int page = 1, int pageSize = 10)
        {
            var payments = await _context.Payments
                .Include(p => p.User)
                .Include(p => p.Course)
                .Where(p => p.CourseId == courseId)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return payments.Select(MapToPaymentDto).ToList();
        }

        public async Task<bool> UpdatePaymentStatusAsync(int paymentId, UpdatePaymentStatusRequest request)
        {
            var payment = await _context.Payments.FindAsync(paymentId);
            if (payment == null)
                return false;

            payment.PaymentStatus = request.PaymentStatus;
            if (!string.IsNullOrEmpty(request.TransactionId))
                payment.TransactionId = request.TransactionId;
            if (!string.IsNullOrEmpty(request.GatewayResponse))
                payment.GatewayResponse = request.GatewayResponse;

            payment.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ProcessPaymentAsync(int paymentId, string transactionId, string gatewayResponse)
        {
            var payment = await _context.Payments.FindAsync(paymentId);
            if (payment == null)
                return false;

            payment.PaymentStatus = "completed";
            payment.TransactionId = transactionId;
            payment.GatewayResponse = gatewayResponse;
            payment.UpdatedAt = DateTime.UtcNow;

            // Update enrollment payment status
            var enrollment = await _context.Enrollments
                .FirstOrDefaultAsync(e => e.StudentId == payment.UserId && e.CourseId == payment.CourseId);

            if (enrollment != null)
            {
                enrollment.PaymentStatus = "completed";
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RefundPaymentAsync(int paymentId, decimal? amount = null)
        {
            var payment = await _context.Payments.FindAsync(paymentId);
            if (payment == null)
                return false;

            payment.PaymentStatus = "refunded";
            payment.UpdatedAt = DateTime.UtcNow;

            // Update enrollment payment status
            var enrollment = await _context.Enrollments
                .FirstOrDefaultAsync(e => e.StudentId == payment.UserId && e.CourseId == payment.CourseId);

            if (enrollment != null)
            {
                enrollment.PaymentStatus = "refunded";
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<PaymentDto>> GetPaymentsByStatusAsync(string status, int page = 1, int pageSize = 10)
        {
            var payments = await _context.Payments
                .Include(p => p.User)
                .Include(p => p.Course)
                .Where(p => p.PaymentStatus == status)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return payments.Select(MapToPaymentDto).ToList();
        }

        public async Task<decimal> GetTotalRevenueAsync(int? instructorId = null, DateTime? fromDate = null, DateTime? toDate = null)
        {
            var query = _context.Payments
                .Where(p => p.PaymentStatus == "completed");

            if (instructorId.HasValue)
            {
                query = query.Where(p => p.Course.InstructorId == instructorId.Value);
            }

            if (fromDate.HasValue)
            {
                query = query.Where(p => p.CreatedAt >= fromDate.Value);
            }

            if (toDate.HasValue)
            {
                query = query.Where(p => p.CreatedAt <= toDate.Value);
            }

            return await query.SumAsync(p => p.Amount);
        }

        public async Task<List<PaymentDto>> GetRevenueReportAsync(int? instructorId = null, DateTime? fromDate = null, DateTime? toDate = null)
        {
            var query = _context.Payments
                .Include(p => p.User)
                .Include(p => p.Course)
                .Where(p => p.PaymentStatus == "completed");

            if (instructorId.HasValue)
            {
                query = query.Where(p => p.Course.InstructorId == instructorId.Value);
            }

            if (fromDate.HasValue)
            {
                query = query.Where(p => p.CreatedAt >= fromDate.Value);
            }

            if (toDate.HasValue)
            {
                query = query.Where(p => p.CreatedAt <= toDate.Value);
            }

            var payments = await query
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();

            return payments.Select(MapToPaymentDto).ToList();
        }

        private PaymentDto MapToPaymentDto(Payment payment)
        {
            return new PaymentDto
            {
                Id = payment.Id,
                User = new UserDto
                {
                    Id = payment.User.Id,
                    Name = payment.User.Name,
                    Email = payment.User.Email,
                    Role = payment.User.Role?.Name ?? "Student",
                    ProfileImage = payment.User.ProfileImage,
                    Phone = payment.User.Phone,
                    Bio = payment.User.Bio,
                    IsActive = payment.User.IsActive,
                    EmailVerified = payment.User.EmailVerified,
                    CreatedAt = payment.User.CreatedAt
                },
                Course = new CourseDto
                {
                    Id = payment.Course.Id,
                    Title = payment.Course.Title,
                    Description = payment.Course.Description,
                    Price = payment.Course.Price,
                    ThumbnailUrl = payment.Course.ThumbnailUrl,
                    IsPublished = payment.Course.IsPublished,
                    DifficultyLevel = payment.Course.DifficultyLevel,
                    DurationHours = payment.Course.DurationHours,
                    Language = payment.Course.Language,
                    CreatedAt = payment.Course.CreatedAt,
                    UpdatedAt = payment.Course.UpdatedAt
                },
                Amount = payment.Amount,
                Currency = payment.Currency,
                PaymentMethod = payment.PaymentMethod,
                PaymentStatus = payment.PaymentStatus,
                TransactionId = payment.TransactionId,
                GatewayResponse = payment.GatewayResponse,
                CreatedAt = payment.CreatedAt,
                UpdatedAt = payment.UpdatedAt
            };
        }
    }
}
