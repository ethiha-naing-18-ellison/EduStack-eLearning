using EduStack.API.DTOs;
using EduStack.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EduStack.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<UsersController> _logger;

        public UsersController(IUserService userService, ILogger<UsersController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        [HttpGet("profile")]
        public async Task<ActionResult<UserDto>> GetMyProfile()
        {
            try
            {
                var userId = GetCurrentUserId();
                var user = await _userService.GetUserByIdAsync(userId);
                return Ok(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user profile");
                return StatusCode(500, new { message = "An error occurred while retrieving your profile" });
            }
        }

        [HttpPut("profile")]
        public async Task<ActionResult<UserDto>> UpdateProfile(UpdateProfileRequest request)
        {
            try
            {
                var userId = GetCurrentUserId();
                var user = await _userService.UpdateProfileAsync(userId, request);
                return Ok(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user profile");
                return StatusCode(500, new { message = "An error occurred while updating your profile" });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserDto>> GetUser(int id)
        {
            try
            {
                var user = await _userService.GetUserByIdAsync(id);
                if (user == null)
                {
                    return NotFound(new { message = "User not found" });
                }
                return Ok(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user {UserId}", id);
                return StatusCode(500, new { message = "An error occurred while retrieving the user" });
            }
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<List<UserDto>>> GetAllUsers(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? search = null,
            [FromQuery] string? role = null)
        {
            try
            {
                var users = await _userService.GetAllUsersAsync(page, pageSize);
                return Ok(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving users");
                return StatusCode(500, new { message = "An error occurred while retrieving users" });
            }
        }

        [HttpGet("role/{role}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<List<UserDto>>> GetUsersByRole(
            string role,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var users = await _userService.GetUsersByRoleAsync(role, page, pageSize);
                return Ok(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving users by role {Role}", role);
                return StatusCode(500, new { message = "An error occurred while retrieving users by role" });
            }
        }

        [HttpPost("deactivate/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeactivateUser(int id)
        {
            try
            {
                var result = await _userService.DeactivateUserAsync(id);
                
                if (result)
                {
                    return Ok(new { message = "User deactivated successfully" });
                }
                
                return BadRequest(new { message = "Failed to deactivate user" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deactivating user {UserId}", id);
                return StatusCode(500, new { message = "An error occurred while deactivating the user" });
            }
        }

        [HttpPost("activate/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> ActivateUser(int id)
        {
            try
            {
                var result = await _userService.ActivateUserAsync(id);
                
                if (result)
                {
                    return Ok(new { message = "User activated successfully" });
                }
                
                return BadRequest(new { message = "Failed to activate user" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error activating user {UserId}", id);
                return StatusCode(500, new { message = "An error occurred while activating the user" });
            }
        }

        // Instructor Application endpoints
        [HttpPost("apply-instructor")]
        public async Task<ActionResult<InstructorApplicationDto>> ApplyForInstructor(InstructorApplicationRequest request)
        {
            try
            {
                var userId = GetCurrentUserId();
                var application = await _userService.ApplyForInstructorAsync(userId, request);
                return CreatedAtAction(nameof(GetMyInstructorApplications), application);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error submitting instructor application");
                return StatusCode(500, new { message = "An error occurred while submitting the instructor application" });
            }
        }

        [HttpGet("my-instructor-applications")]
        public async Task<ActionResult<List<InstructorApplicationDto>>> GetMyInstructorApplications()
        {
            try
            {
                var userId = GetCurrentUserId();
                var applications = await _userService.GetInstructorApplicationsByUserAsync(userId);
                return Ok(applications);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving instructor applications");
                return StatusCode(500, new { message = "An error occurred while retrieving instructor applications" });
            }
        }

        [HttpGet("instructor-applications/pending")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<List<InstructorApplicationDto>>> GetPendingInstructorApplications()
        {
            try
            {
                var applications = await _userService.GetPendingInstructorApplicationsAsync();
                return Ok(applications);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving pending instructor applications");
                return StatusCode(500, new { message = "An error occurred while retrieving pending instructor applications" });
            }
        }

        [HttpPut("instructor-applications/{applicationId}/review")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<InstructorApplicationDto>> ReviewInstructorApplication(
            int applicationId,
            ReviewInstructorApplicationRequest request)
        {
            try
            {
                var adminId = GetCurrentUserId();
                var application = await _userService.ReviewInstructorApplicationAsync(applicationId, adminId, request);
                return Ok(application);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error reviewing instructor application {ApplicationId}", applicationId);
                return StatusCode(500, new { message = "An error occurred while reviewing the instructor application" });
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
