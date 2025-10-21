using EduStack.API.DTOs;
using EduStack.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EduStack.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CoursesController : ControllerBase
    {
        private readonly ICourseService _courseService;
        private readonly ILogger<CoursesController> _logger;

        public CoursesController(ICourseService courseService, ILogger<CoursesController> logger)
        {
            _courseService = courseService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<List<CourseDto>>> GetCourses(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? search = null,
            [FromQuery] int? categoryId = null)
        {
            try
            {
                var courses = await _courseService.GetPublishedCoursesAsync(page, pageSize, search, categoryId);
                return Ok(courses);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving courses");
                return StatusCode(500, new { message = "An error occurred while retrieving courses" });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CourseDto>> GetCourse(int id)
        {
            try
            {
                var course = await _courseService.GetCourseByIdAsync(id);
                if (course == null)
                {
                    return NotFound(new { message = "Course not found" });
                }
                return Ok(course);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving course {CourseId}", id);
                return StatusCode(500, new { message = "An error occurred while retrieving the course" });
            }
        }

        [HttpPost]
        [Authorize(Roles = "Instructor,Admin")]
        public async Task<ActionResult<CourseDto>> CreateCourse(CreateCourseRequest request)
        {
            try
            {
                var instructorId = GetCurrentUserId();
                var course = await _courseService.CreateCourseAsync(instructorId, request);
                return CreatedAtAction(nameof(GetCourse), new { id = course.Id }, course);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating course");
                return StatusCode(500, new { message = "An error occurred while creating the course" });
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Instructor,Admin")]
        public async Task<ActionResult<CourseDto>> UpdateCourse(int id, UpdateCourseRequest request)
        {
            try
            {
                var instructorId = GetCurrentUserId();
                var course = await _courseService.UpdateCourseAsync(id, instructorId, request);
                return Ok(course);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating course {CourseId}", id);
                return StatusCode(500, new { message = "An error occurred while updating the course" });
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Instructor,Admin")]
        public async Task<ActionResult> DeleteCourse(int id)
        {
            try
            {
                var instructorId = GetCurrentUserId();
                var result = await _courseService.DeleteCourseAsync(id, instructorId);
                
                if (result)
                {
                    return Ok(new { message = "Course deleted successfully" });
                }
                
                return BadRequest(new { message = "Failed to delete course" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting course {CourseId}", id);
                return StatusCode(500, new { message = "An error occurred while deleting the course" });
            }
        }

        [HttpPost("{id}/publish")]
        [Authorize(Roles = "Instructor,Admin")]
        public async Task<ActionResult> PublishCourse(int id)
        {
            try
            {
                var instructorId = GetCurrentUserId();
                var result = await _courseService.PublishCourseAsync(id, instructorId);
                
                if (result)
                {
                    return Ok(new { message = "Course published successfully" });
                }
                
                return BadRequest(new { message = "Failed to publish course" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error publishing course {CourseId}", id);
                return StatusCode(500, new { message = "An error occurred while publishing the course" });
            }
        }

        [HttpPost("{id}/unpublish")]
        [Authorize(Roles = "Instructor,Admin")]
        public async Task<ActionResult> UnpublishCourse(int id)
        {
            try
            {
                var instructorId = GetCurrentUserId();
                var result = await _courseService.UnpublishCourseAsync(id, instructorId);
                
                if (result)
                {
                    return Ok(new { message = "Course unpublished successfully" });
                }
                
                return BadRequest(new { message = "Failed to unpublish course" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error unpublishing course {CourseId}", id);
                return StatusCode(500, new { message = "An error occurred while unpublishing the course" });
            }
        }

        // Course Sections
        [HttpPost("{courseId}/sections")]
        [Authorize(Roles = "Instructor,Admin")]
        public async Task<ActionResult<CourseSectionDto>> CreateCourseSection(int courseId, CreateCourseSectionRequest request)
        {
            try
            {
                var instructorId = GetCurrentUserId();
                var section = await _courseService.CreateCourseSectionAsync(courseId, instructorId, request);
                return CreatedAtAction(nameof(GetCourse), new { id = courseId }, section);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating course section for course {CourseId}", courseId);
                return StatusCode(500, new { message = "An error occurred while creating the course section" });
            }
        }

        [HttpPut("sections/{sectionId}")]
        [Authorize(Roles = "Instructor,Admin")]
        public async Task<ActionResult<CourseSectionDto>> UpdateCourseSection(int sectionId, UpdateCourseSectionRequest request)
        {
            try
            {
                var instructorId = GetCurrentUserId();
                var section = await _courseService.UpdateCourseSectionAsync(sectionId, instructorId, request);
                return Ok(section);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating course section {SectionId}", sectionId);
                return StatusCode(500, new { message = "An error occurred while updating the course section" });
            }
        }

        [HttpDelete("sections/{sectionId}")]
        [Authorize(Roles = "Instructor,Admin")]
        public async Task<ActionResult> DeleteCourseSection(int sectionId)
        {
            try
            {
                var instructorId = GetCurrentUserId();
                var result = await _courseService.DeleteCourseSectionAsync(sectionId, instructorId);
                
                if (result)
                {
                    return Ok(new { message = "Course section deleted successfully" });
                }
                
                return BadRequest(new { message = "Failed to delete course section" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting course section {SectionId}", sectionId);
                return StatusCode(500, new { message = "An error occurred while deleting the course section" });
            }
        }

        // Lessons
        [HttpPost("sections/{sectionId}/lessons")]
        [Authorize(Roles = "Instructor,Admin")]
        public async Task<ActionResult<LessonDto>> CreateLesson(int sectionId, CreateLessonRequest request)
        {
            try
            {
                var instructorId = GetCurrentUserId();
                var lesson = await _courseService.CreateLessonAsync(sectionId, instructorId, request);
                return CreatedAtAction(nameof(GetCourse), new { id = sectionId }, lesson);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating lesson for section {SectionId}", sectionId);
                return StatusCode(500, new { message = "An error occurred while creating the lesson" });
            }
        }

        [HttpPut("lessons/{lessonId}")]
        [Authorize(Roles = "Instructor,Admin")]
        public async Task<ActionResult<LessonDto>> UpdateLesson(int lessonId, UpdateLessonRequest request)
        {
            try
            {
                var instructorId = GetCurrentUserId();
                var lesson = await _courseService.UpdateLessonAsync(lessonId, instructorId, request);
                return Ok(lesson);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating lesson {LessonId}", lessonId);
                return StatusCode(500, new { message = "An error occurred while updating the lesson" });
            }
        }

        [HttpDelete("lessons/{lessonId}")]
        [Authorize(Roles = "Instructor,Admin")]
        public async Task<ActionResult> DeleteLesson(int lessonId)
        {
            try
            {
                var instructorId = GetCurrentUserId();
                var result = await _courseService.DeleteLessonAsync(lessonId, instructorId);
                
                if (result)
                {
                    return Ok(new { message = "Lesson deleted successfully" });
                }
                
                return BadRequest(new { message = "Failed to delete lesson" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting lesson {LessonId}", lessonId);
                return StatusCode(500, new { message = "An error occurred while deleting the lesson" });
            }
        }

        // Resources
        [HttpPost("lessons/{lessonId}/resources")]
        [Authorize(Roles = "Instructor,Admin")]
        public async Task<ActionResult<ResourceDto>> CreateResource(int lessonId, CreateResourceRequest request)
        {
            try
            {
                var instructorId = GetCurrentUserId();
                var resource = await _courseService.CreateResourceAsync(lessonId, instructorId, request);
                return CreatedAtAction(nameof(GetCourse), new { id = lessonId }, resource);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating resource for lesson {LessonId}", lessonId);
                return StatusCode(500, new { message = "An error occurred while creating the resource" });
            }
        }

        [HttpDelete("resources/{resourceId}")]
        [Authorize(Roles = "Instructor,Admin")]
        public async Task<ActionResult> DeleteResource(int resourceId)
        {
            try
            {
                var instructorId = GetCurrentUserId();
                var result = await _courseService.DeleteResourceAsync(resourceId, instructorId);
                
                if (result)
                {
                    return Ok(new { message = "Resource deleted successfully" });
                }
                
                return BadRequest(new { message = "Failed to delete resource" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting resource {ResourceId}", resourceId);
                return StatusCode(500, new { message = "An error occurred while deleting the resource" });
            }
        }

        // Categories
        [HttpGet("categories")]
        public async Task<ActionResult<List<CategoryDto>>> GetCategories()
        {
            try
            {
                var categories = await _courseService.GetAllCategoriesAsync();
                return Ok(categories);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving categories");
                return StatusCode(500, new { message = "An error occurred while retrieving categories" });
            }
        }

        [HttpPost("categories")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<CategoryDto>> CreateCategory(CreateCategoryRequest request)
        {
            try
            {
                var category = await _courseService.CreateCategoryAsync(request);
                return CreatedAtAction(nameof(GetCategories), category);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating category");
                return StatusCode(500, new { message = "An error occurred while creating the category" });
            }
        }

        [HttpPut("categories/{categoryId}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<CategoryDto>> UpdateCategory(int categoryId, UpdateCategoryRequest request)
        {
            try
            {
                var category = await _courseService.UpdateCategoryAsync(categoryId, request);
                return Ok(category);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating category {CategoryId}", categoryId);
                return StatusCode(500, new { message = "An error occurred while updating the category" });
            }
        }

        [HttpDelete("categories/{categoryId}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeleteCategory(int categoryId)
        {
            try
            {
                var result = await _courseService.DeleteCategoryAsync(categoryId);
                
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
