using EduStack.API.DTOs;
using EduStack.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EduStack.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class EnrollmentsController : ControllerBase
    {
        private readonly IEnrollmentService _enrollmentService;
        private readonly ILogger<EnrollmentsController> _logger;

        public EnrollmentsController(IEnrollmentService enrollmentService, ILogger<EnrollmentsController> logger)
        {
            _enrollmentService = enrollmentService;
            _logger = logger;
        }

        [HttpPost]
        public async Task<ActionResult<EnrollmentDto>> EnrollInCourse(EnrollRequest request)
        {
            try
            {
                var studentId = GetCurrentUserId();
                var enrollment = await _enrollmentService.EnrollInCourseAsync(studentId, request);
                return CreatedAtAction(nameof(GetEnrollment), new { id = enrollment.Id }, enrollment);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error enrolling in course {CourseId}", request.CourseId);
                return StatusCode(500, new { message = "An error occurred while enrolling in the course" });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<EnrollmentDto>> GetEnrollment(int id)
        {
            try
            {
                var studentId = GetCurrentUserId();
                // Implementation would need to verify the enrollment belongs to the current user
                await Task.CompletedTask; // Placeholder for async operation
                return Ok(new { message = "Enrollment details would be returned here" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving enrollment {EnrollmentId}", id);
                return StatusCode(500, new { message = "An error occurred while retrieving the enrollment" });
            }
        }

        [HttpGet("my-enrollments")]
        public async Task<ActionResult<List<EnrollmentDto>>> GetMyEnrollments(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var studentId = GetCurrentUserId();
                var enrollments = await _enrollmentService.GetStudentEnrollmentsAsync(studentId, page, pageSize);
                return Ok(enrollments);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving student enrollments");
                return StatusCode(500, new { message = "An error occurred while retrieving enrollments" });
            }
        }

        [HttpGet("course/{courseId}/progress")]
        public async Task<ActionResult<EnrollmentProgressDto>> GetCourseProgress(int courseId)
        {
            try
            {
                var studentId = GetCurrentUserId();
                var progress = await _enrollmentService.GetEnrollmentProgressAsync(studentId, courseId);
                return Ok(progress);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving course progress for course {CourseId}", courseId);
                return StatusCode(500, new { message = "An error occurred while retrieving course progress" });
            }
        }

        [HttpPut("lessons/progress")]
        public async Task<ActionResult> UpdateLessonProgress(UpdateLessonProgressRequest request)
        {
            try
            {
                var studentId = GetCurrentUserId();
                var result = await _enrollmentService.UpdateLessonProgressAsync(studentId, request);
                
                if (result)
                {
                    return Ok(new { message = "Lesson progress updated successfully" });
                }
                
                return BadRequest(new { message = "Failed to update lesson progress" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating lesson progress");
                return StatusCode(500, new { message = "An error occurred while updating lesson progress" });
            }
        }

        [HttpGet("course/{courseId}/lessons")]
        public async Task<ActionResult<List<LessonProgressDto>>> GetCourseLessons(int courseId)
        {
            try
            {
                var studentId = GetCurrentUserId();
                var lessons = await _enrollmentService.GetStudentLessonProgressAsync(studentId, courseId);
                return Ok(lessons);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving course lessons for course {CourseId}", courseId);
                return StatusCode(500, new { message = "An error occurred while retrieving course lessons" });
            }
        }

        [HttpPost("course/{courseId}/complete")]
        public async Task<ActionResult> CompleteCourse(int courseId)
        {
            try
            {
                var studentId = GetCurrentUserId();
                var result = await _enrollmentService.CompleteCourseAsync(studentId, courseId);
                
                if (result)
                {
                    return Ok(new { message = "Course completed successfully" });
                }
                
                return BadRequest(new { message = "Failed to complete course" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error completing course {CourseId}", courseId);
                return StatusCode(500, new { message = "An error occurred while completing the course" });
            }
        }

        [HttpDelete("course/{courseId}")]
        public async Task<ActionResult> UnenrollFromCourse(int courseId)
        {
            try
            {
                var studentId = GetCurrentUserId();
                var result = await _enrollmentService.UnenrollFromCourseAsync(studentId, courseId);
                
                if (result)
                {
                    return Ok(new { message = "Successfully unenrolled from course" });
                }
                
                return BadRequest(new { message = "Failed to unenroll from course" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error unenrolling from course {CourseId}", courseId);
                return StatusCode(500, new { message = "An error occurred while unenrolling from the course" });
            }
        }

        [HttpGet("course/{courseId}/check-enrollment")]
        public async Task<ActionResult> CheckEnrollment(int courseId)
        {
            try
            {
                var studentId = GetCurrentUserId();
                var isEnrolled = await _enrollmentService.IsEnrolledAsync(studentId, courseId);
                return Ok(new { isEnrolled });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking enrollment for course {CourseId}", courseId);
                return StatusCode(500, new { message = "An error occurred while checking enrollment" });
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
