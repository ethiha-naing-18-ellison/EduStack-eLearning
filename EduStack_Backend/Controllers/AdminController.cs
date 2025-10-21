using EduStack.API.DTOs;
using EduStack.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EduStack.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;
        private readonly ILogger<AdminController> _logger;

        public AdminController(IAdminService adminService, ILogger<AdminController> logger)
        {
            _adminService = adminService;
            _logger = logger;
        }

        // Dashboard
        [HttpGet("dashboard")]
        public async Task<ActionResult<object>> GetDashboardStats()
        {
            try
            {
                var stats = await _adminService.GetDashboardStatsAsync();
                return Ok(stats);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving dashboard stats");
                return StatusCode(500, new { message = "An error occurred while retrieving dashboard statistics" });
            }
        }

        // User Management
        [HttpGet("users")]
        public async Task<ActionResult<List<UserDto>>> GetAllUsers(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? search = null,
            [FromQuery] string? role = null)
        {
            try
            {
                var users = await _adminService.GetAllUsersAsync(page, pageSize, search, role);
                return Ok(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving users");
                return StatusCode(500, new { message = "An error occurred while retrieving users" });
            }
        }

        [HttpGet("users/{userId}")]
        public async Task<ActionResult<UserDto>> GetUser(int userId)
        {
            try
            {
                var user = await _adminService.GetUserByIdAsync(userId);
                if (user == null)
                {
                    return NotFound(new { message = "User not found" });
                }
                return Ok(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user {UserId}", userId);
                return StatusCode(500, new { message = "An error occurred while retrieving the user" });
            }
        }

        [HttpPost("users/{userId}/activate")]
        public async Task<ActionResult> ActivateUser(int userId)
        {
            try
            {
                var result = await _adminService.ActivateUserAsync(userId);
                
                if (result)
                {
                    return Ok(new { message = "User activated successfully" });
                }
                
                return BadRequest(new { message = "Failed to activate user" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error activating user {UserId}", userId);
                return StatusCode(500, new { message = "An error occurred while activating the user" });
            }
        }

        [HttpPost("users/{userId}/deactivate")]
        public async Task<ActionResult> DeactivateUser(int userId)
        {
            try
            {
                var result = await _adminService.DeactivateUserAsync(userId);
                
                if (result)
                {
                    return Ok(new { message = "User deactivated successfully" });
                }
                
                return BadRequest(new { message = "Failed to deactivate user" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deactivating user {UserId}", userId);
                return StatusCode(500, new { message = "An error occurred while deactivating the user" });
            }
        }

        [HttpDelete("users/{userId}")]
        public async Task<ActionResult> DeleteUser(int userId)
        {
            try
            {
                var result = await _adminService.DeleteUserAsync(userId);
                
                if (result)
                {
                    return Ok(new { message = "User deleted successfully" });
                }
                
                return BadRequest(new { message = "Failed to delete user" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting user {UserId}", userId);
                return StatusCode(500, new { message = "An error occurred while deleting the user" });
            }
        }

        // Course Management
        [HttpGet("courses")]
        public async Task<ActionResult<List<CourseDto>>> GetAllCourses(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? search = null,
            [FromQuery] int? categoryId = null)
        {
            try
            {
                var courses = await _adminService.GetAllCoursesAsync(page, pageSize, search, categoryId);
                return Ok(courses);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving courses");
                return StatusCode(500, new { message = "An error occurred while retrieving courses" });
            }
        }

        [HttpPost("courses/{courseId}/publish")]
        public async Task<ActionResult> PublishCourse(int courseId)
        {
            try
            {
                var result = await _adminService.PublishCourseAsync(courseId);
                
                if (result)
                {
                    return Ok(new { message = "Course published successfully" });
                }
                
                return BadRequest(new { message = "Failed to publish course" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error publishing course {CourseId}", courseId);
                return StatusCode(500, new { message = "An error occurred while publishing the course" });
            }
        }

        [HttpPost("courses/{courseId}/unpublish")]
        public async Task<ActionResult> UnpublishCourse(int courseId)
        {
            try
            {
                var result = await _adminService.UnpublishCourseAsync(courseId);
                
                if (result)
                {
                    return Ok(new { message = "Course unpublished successfully" });
                }
                
                return BadRequest(new { message = "Failed to unpublish course" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error unpublishing course {CourseId}", courseId);
                return StatusCode(500, new { message = "An error occurred while unpublishing the course" });
            }
        }

        [HttpDelete("courses/{courseId}")]
        public async Task<ActionResult> DeleteCourse(int courseId)
        {
            try
            {
                var result = await _adminService.DeleteCourseAsync(courseId);
                
                if (result)
                {
                    return Ok(new { message = "Course deleted successfully" });
                }
                
                return BadRequest(new { message = "Failed to delete course" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting course {CourseId}", courseId);
                return StatusCode(500, new { message = "An error occurred while deleting the course" });
            }
        }

        // Instructor Applications
        [HttpGet("instructor-applications")]
        public async Task<ActionResult<List<InstructorApplicationDto>>> GetAllInstructorApplications(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var applications = await _adminService.GetAllInstructorApplicationsAsync(page, pageSize);
                return Ok(applications);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving instructor applications");
                return StatusCode(500, new { message = "An error occurred while retrieving instructor applications" });
            }
        }

        [HttpGet("instructor-applications/pending")]
        public async Task<ActionResult<List<InstructorApplicationDto>>> GetPendingInstructorApplications()
        {
            try
            {
                var applications = await _adminService.GetPendingInstructorApplicationsAsync();
                return Ok(applications);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving pending instructor applications");
                return StatusCode(500, new { message = "An error occurred while retrieving pending instructor applications" });
            }
        }

        [HttpPut("instructor-applications/{applicationId}/review")]
        public async Task<ActionResult<InstructorApplicationDto>> ReviewInstructorApplication(
            int applicationId,
            ReviewInstructorApplicationRequest request)
        {
            try
            {
                var adminId = GetCurrentUserId();
                var application = await _adminService.ReviewInstructorApplicationAsync(applicationId, adminId, request);
                return Ok(application);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error reviewing instructor application {ApplicationId}", applicationId);
                return StatusCode(500, new { message = "An error occurred while reviewing the instructor application" });
            }
        }

        // Reviews Management
        [HttpGet("reviews/pending")]
        public async Task<ActionResult<List<ReviewDto>>> GetPendingReviews(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var reviews = await _adminService.GetPendingReviewsAsync(page, pageSize);
                return Ok(reviews);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving pending reviews");
                return StatusCode(500, new { message = "An error occurred while retrieving pending reviews" });
            }
        }

        [HttpPost("reviews/{reviewId}/approve")]
        public async Task<ActionResult> ApproveReview(int reviewId)
        {
            try
            {
                var result = await _adminService.ApproveReviewAsync(reviewId);
                
                if (result)
                {
                    return Ok(new { message = "Review approved successfully" });
                }
                
                return BadRequest(new { message = "Failed to approve review" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error approving review {ReviewId}", reviewId);
                return StatusCode(500, new { message = "An error occurred while approving the review" });
            }
        }

        [HttpPost("reviews/{reviewId}/reject")]
        public async Task<ActionResult> RejectReview(int reviewId)
        {
            try
            {
                var result = await _adminService.RejectReviewAsync(reviewId);
                
                if (result)
                {
                    return Ok(new { message = "Review rejected successfully" });
                }
                
                return BadRequest(new { message = "Failed to reject review" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error rejecting review {ReviewId}", reviewId);
                return StatusCode(500, new { message = "An error occurred while rejecting the review" });
            }
        }

        // Categories Management
        [HttpGet("categories")]
        public async Task<ActionResult<List<CategoryDto>>> GetAllCategories()
        {
            try
            {
                var categories = await _adminService.GetAllCategoriesAsync();
                return Ok(categories);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving categories");
                return StatusCode(500, new { message = "An error occurred while retrieving categories" });
            }
        }

        [HttpPost("categories")]
        public async Task<ActionResult<CategoryDto>> CreateCategory(CreateCategoryRequest request)
        {
            try
            {
                var category = await _adminService.CreateCategoryAsync(request);
                return CreatedAtAction(nameof(GetAllCategories), category);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating category");
                return StatusCode(500, new { message = "An error occurred while creating the category" });
            }
        }

        [HttpPut("categories/{categoryId}")]
        public async Task<ActionResult<CategoryDto>> UpdateCategory(int categoryId, UpdateCategoryRequest request)
        {
            try
            {
                var category = await _adminService.UpdateCategoryAsync(categoryId, request);
                return Ok(category);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating category {CategoryId}", categoryId);
                return StatusCode(500, new { message = "An error occurred while updating the category" });
            }
        }

        [HttpDelete("categories/{categoryId}")]
        public async Task<ActionResult> DeleteCategory(int categoryId)
        {
            try
            {
                var result = await _adminService.DeleteCategoryAsync(categoryId);
                
                if (result)
                {
                    return Ok(new { message = "Category deleted successfully" });
                }
                
                return BadRequest(new { message = "Failed to delete category" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting category {CategoryId}", categoryId);
                return StatusCode(500, new { message = "An error occurred while deleting the category" });
            }
        }

        // Analytics and Reports
        [HttpGet("revenue-report")]
        public async Task<ActionResult<List<PaymentDto>>> GetRevenueReport(
            [FromQuery] DateTime? fromDate = null,
            [FromQuery] DateTime? toDate = null)
        {
            try
            {
                var payments = await _adminService.GetRevenueReportAsync(fromDate, toDate);
                return Ok(payments);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving revenue report");
                return StatusCode(500, new { message = "An error occurred while retrieving the revenue report" });
            }
        }

        [HttpGet("top-courses")]
        public async Task<ActionResult<List<CourseDto>>> GetTopCourses([FromQuery] int count = 10)
        {
            try
            {
                var courses = await _adminService.GetTopCoursesAsync(count);
                return Ok(courses);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving top courses");
                return StatusCode(500, new { message = "An error occurred while retrieving top courses" });
            }
        }

        [HttpGet("top-instructors")]
        public async Task<ActionResult<List<UserDto>>> GetTopInstructors([FromQuery] int count = 10)
        {
            try
            {
                var instructors = await _adminService.GetTopInstructorsAsync(count);
                return Ok(instructors);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving top instructors");
                return StatusCode(500, new { message = "An error occurred while retrieving top instructors" });
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
