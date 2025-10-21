using EduStack.API.DTOs;
using EduStack.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EduStack.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReviewsController : ControllerBase
    {
        private readonly IReviewService _reviewService;
        private readonly ILogger<ReviewsController> _logger;

        public ReviewsController(IReviewService reviewService, ILogger<ReviewsController> logger)
        {
            _reviewService = reviewService;
            _logger = logger;
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<ReviewDto>> CreateReview(CreateReviewRequest request)
        {
            try
            {
                var studentId = GetCurrentUserId();
                var review = await _reviewService.CreateReviewAsync(studentId, request);
                return CreatedAtAction(nameof(GetReview), new { id = review.Id }, review);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating review for course {CourseId}", request.CourseId);
                return StatusCode(500, new { message = "An error occurred while creating the review" });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ReviewDto>> GetReview(int id)
        {
            try
            {
                var review = await _reviewService.GetReviewByIdAsync(id);
                if (review == null)
                {
                    return NotFound(new { message = "Review not found" });
                }
                return Ok(review);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving review {ReviewId}", id);
                return StatusCode(500, new { message = "An error occurred while retrieving the review" });
            }
        }

        [HttpGet("course/{courseId}")]
        public async Task<ActionResult<List<ReviewDto>>> GetCourseReviews(
            int courseId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var reviews = await _reviewService.GetCourseReviewsAsync(courseId, page, pageSize);
                return Ok(reviews);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving reviews for course {CourseId}", courseId);
                return StatusCode(500, new { message = "An error occurred while retrieving course reviews" });
            }
        }

        [HttpGet("course/{courseId}/stats")]
        public async Task<ActionResult<ReviewStatsDto>> GetCourseReviewStats(int courseId)
        {
            try
            {
                var stats = await _reviewService.GetCourseReviewStatsAsync(courseId);
                return Ok(stats);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving review stats for course {CourseId}", courseId);
                return StatusCode(500, new { message = "An error occurred while retrieving review statistics" });
            }
        }

        [HttpGet("my-reviews")]
        [Authorize]
        public async Task<ActionResult<List<ReviewDto>>> GetMyReviews(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var studentId = GetCurrentUserId();
                var reviews = await _reviewService.GetUserReviewsAsync(studentId, page, pageSize);
                return Ok(reviews);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user reviews");
                return StatusCode(500, new { message = "An error occurred while retrieving your reviews" });
            }
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<ActionResult<ReviewDto>> UpdateReview(int id, UpdateReviewRequest request)
        {
            try
            {
                var studentId = GetCurrentUserId();
                var review = await _reviewService.UpdateReviewAsync(id, studentId, request);
                return Ok(review);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating review {ReviewId}", id);
                return StatusCode(500, new { message = "An error occurred while updating the review" });
            }
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<ActionResult> DeleteReview(int id)
        {
            try
            {
                var studentId = GetCurrentUserId();
                var result = await _reviewService.DeleteReviewAsync(id, studentId);
                
                if (result)
                {
                    return Ok(new { message = "Review deleted successfully" });
                }
                
                return BadRequest(new { message = "Failed to delete review" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting review {ReviewId}", id);
                return StatusCode(500, new { message = "An error occurred while deleting the review" });
            }
        }

        [HttpGet("course/{courseId}/check-review")]
        [Authorize]
        public async Task<ActionResult> CheckUserReview(int courseId)
        {
            try
            {
                var studentId = GetCurrentUserId();
                var hasReviewed = await _reviewService.HasUserReviewedCourseAsync(studentId, courseId);
                return Ok(new { hasReviewed });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking review for course {CourseId}", courseId);
                return StatusCode(500, new { message = "An error occurred while checking review status" });
            }
        }

        // Admin endpoints
        [HttpGet("pending")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<List<ReviewDto>>> GetPendingReviews(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var reviews = await _reviewService.GetPendingReviewsAsync(page, pageSize);
                return Ok(reviews);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving pending reviews");
                return StatusCode(500, new { message = "An error occurred while retrieving pending reviews" });
            }
        }

        [HttpPost("{id}/approve")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> ApproveReview(int id)
        {
            try
            {
                var result = await _reviewService.ApproveReviewAsync(id);
                
                if (result)
                {
                    return Ok(new { message = "Review approved successfully" });
                }
                
                return BadRequest(new { message = "Failed to approve review" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error approving review {ReviewId}", id);
                return StatusCode(500, new { message = "An error occurred while approving the review" });
            }
        }

        [HttpPost("{id}/reject")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> RejectReview(int id)
        {
            try
            {
                var result = await _reviewService.RejectReviewAsync(id);
                
                if (result)
                {
                    return Ok(new { message = "Review rejected successfully" });
                }
                
                return BadRequest(new { message = "Failed to reject review" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error rejecting review {ReviewId}", id);
                return StatusCode(500, new { message = "An error occurred while rejecting the review" });
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
}
