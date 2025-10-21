using EduStack.API.DTOs;
using EduStack.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EduStack.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PaymentsController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        private readonly ILogger<PaymentsController> _logger;

        public PaymentsController(IPaymentService paymentService, ILogger<PaymentsController> logger)
        {
            _paymentService = paymentService;
            _logger = logger;
        }

        [HttpPost]
        public async Task<ActionResult<PaymentResponse>> CreatePayment(CreatePaymentRequest request)
        {
            try
            {
                var userId = GetCurrentUserId();
                var payment = await _paymentService.CreatePaymentAsync(userId, request);
                return Ok(payment);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating payment for course {CourseId}", request.CourseId);
                return StatusCode(500, new { message = "An error occurred while creating the payment" });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PaymentDto>> GetPayment(int id)
        {
            try
            {
                var payment = await _paymentService.GetPaymentByIdAsync(id);
                if (payment == null)
                {
                    return NotFound(new { message = "Payment not found" });
                }
                return Ok(payment);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving payment {PaymentId}", id);
                return StatusCode(500, new { message = "An error occurred while retrieving the payment" });
            }
        }

        [HttpGet("my-payments")]
        public async Task<ActionResult<List<PaymentDto>>> GetMyPayments(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var userId = GetCurrentUserId();
                var payments = await _paymentService.GetUserPaymentsAsync(userId, page, pageSize);
                return Ok(payments);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user payments");
                return StatusCode(500, new { message = "An error occurred while retrieving your payments" });
            }
        }

        [HttpGet("course/{courseId}")]
        [Authorize(Roles = "Instructor,Admin")]
        public async Task<ActionResult<List<PaymentDto>>> GetCoursePayments(
            int courseId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var payments = await _paymentService.GetCoursePaymentsAsync(courseId, page, pageSize);
                return Ok(payments);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving payments for course {CourseId}", courseId);
                return StatusCode(500, new { message = "An error occurred while retrieving course payments" });
            }
        }

        [HttpPut("{id}/status")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> UpdatePaymentStatus(int id, UpdatePaymentStatusRequest request)
        {
            try
            {
                var result = await _paymentService.UpdatePaymentStatusAsync(id, request);
                
                if (result)
                {
                    return Ok(new { message = "Payment status updated successfully" });
                }
                
                return BadRequest(new { message = "Failed to update payment status" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating payment status for payment {PaymentId}", id);
                return StatusCode(500, new { message = "An error occurred while updating the payment status" });
            }
        }

        [HttpPost("{id}/process")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> ProcessPayment(int id, ProcessPaymentRequest request)
        {
            try
            {
                var result = await _paymentService.ProcessPaymentAsync(id, request.TransactionId, request.GatewayResponse);
                
                if (result)
                {
                    return Ok(new { message = "Payment processed successfully" });
                }
                
                return BadRequest(new { message = "Failed to process payment" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing payment {PaymentId}", id);
                return StatusCode(500, new { message = "An error occurred while processing the payment" });
            }
        }

        [HttpPost("{id}/refund")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> RefundPayment(int id, RefundPaymentRequest? request = null)
        {
            try
            {
                var result = await _paymentService.RefundPaymentAsync(id, request?.Amount);
                
                if (result)
                {
                    return Ok(new { message = "Payment refunded successfully" });
                }
                
                return BadRequest(new { message = "Failed to refund payment" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error refunding payment {PaymentId}", id);
                return StatusCode(500, new { message = "An error occurred while refunding the payment" });
            }
        }

        [HttpGet("status/{status}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<List<PaymentDto>>> GetPaymentsByStatus(
            string status,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var payments = await _paymentService.GetPaymentsByStatusAsync(status, page, pageSize);
                return Ok(payments);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving payments by status {Status}", status);
                return StatusCode(500, new { message = "An error occurred while retrieving payments by status" });
            }
        }

        [HttpGet("revenue")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<decimal>> GetTotalRevenue(
            [FromQuery] int? instructorId = null,
            [FromQuery] DateTime? fromDate = null,
            [FromQuery] DateTime? toDate = null)
        {
            try
            {
                var revenue = await _paymentService.GetTotalRevenueAsync(instructorId, fromDate, toDate);
                return Ok(new { totalRevenue = revenue });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving total revenue");
                return StatusCode(500, new { message = "An error occurred while retrieving total revenue" });
            }
        }

        [HttpGet("revenue-report")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<List<PaymentDto>>> GetRevenueReport(
            [FromQuery] int? instructorId = null,
            [FromQuery] DateTime? fromDate = null,
            [FromQuery] DateTime? toDate = null)
        {
            try
            {
                var payments = await _paymentService.GetRevenueReportAsync(instructorId, fromDate, toDate);
                return Ok(payments);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving revenue report");
                return StatusCode(500, new { message = "An error occurred while retrieving the revenue report" });
            }
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                throw new UnauthorizedAccessException("Invalid user ID");
            }
            return userId;
        }
    }

    public class ProcessPaymentRequest
    {
        public string TransactionId { get; set; } = string.Empty;
        public string GatewayResponse { get; set; } = string.Empty;
    }

    public class RefundPaymentRequest
    {
        public decimal? Amount { get; set; }
    }
}
