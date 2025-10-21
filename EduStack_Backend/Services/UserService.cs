using EduStack.API.Data;
using EduStack.API.DTOs;
using EduStack.API.Models;
using Microsoft.EntityFrameworkCore;

namespace EduStack.API.Services
{
    public class UserService : IUserService
    {
        private readonly EduStackDbContext _context;
        private readonly ILogger<UserService> _logger;

        public UserService(EduStackDbContext context, ILogger<UserService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<UserDto> GetUserByIdAsync(int id)
        {
            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
                return null!;

            return MapToUserDto(user);
        }

        public async Task<UserDto> GetUserByEmailAsync(string email)
        {
            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Email == email);

            if (user == null)
                return null!;

            return MapToUserDto(user);
        }

        public async Task<UserDto> UpdateProfileAsync(int userId, UpdateProfileRequest request)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                throw new KeyNotFoundException("User not found");

            if (!string.IsNullOrEmpty(request.Name))
                user.Name = request.Name;
            if (!string.IsNullOrEmpty(request.Phone))
                user.Phone = request.Phone;
            if (request.Bio != null)
                user.Bio = request.Bio;
            if (request.ProfileImage != null)
                user.ProfileImage = request.ProfileImage;

            user.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return MapToUserDto(user);
        }

        public async Task<bool> DeactivateUserAsync(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                return false;

            user.IsActive = false;
            user.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ActivateUserAsync(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                return false;

            user.IsActive = true;
            user.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<UserDto>> GetAllUsersAsync(int page = 1, int pageSize = 10)
        {
            var users = await _context.Users
                .Include(u => u.Role)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return users.Select(MapToUserDto).ToList();
        }

        public async Task<List<UserDto>> GetUsersByRoleAsync(string role, int page = 1, int pageSize = 10)
        {
            var users = await _context.Users
                .Include(u => u.Role)
                .Where(u => u.Role!.Name == role)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return users.Select(MapToUserDto).ToList();
        }

        public async Task<InstructorApplicationDto> ApplyForInstructorAsync(int userId, InstructorApplicationRequest request)
        {
            var application = new InstructorApplication
            {
                UserId = userId,
                ApplicationStatus = "pending",
                Qualifications = request.Qualifications,
                ExperienceYears = request.ExperienceYears,
                PortfolioUrl = request.PortfolioUrl,
                Motivation = request.Motivation,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.InstructorApplications.Add(application);
            await _context.SaveChangesAsync();

            return MapToInstructorApplicationDto(application);
        }

        public async Task<List<InstructorApplicationDto>> GetPendingInstructorApplicationsAsync()
        {
            var applications = await _context.InstructorApplications
                .Include(ia => ia.User)
                .Include(ia => ia.Reviewer)
                .Where(ia => ia.ApplicationStatus == "pending")
                .ToListAsync();

            return applications.Select(MapToInstructorApplicationDto).ToList();
        }

        public async Task<InstructorApplicationDto> ReviewInstructorApplicationAsync(int applicationId, int adminId, ReviewInstructorApplicationRequest request)
        {
            var application = await _context.InstructorApplications
                .Include(ia => ia.User)
                .Include(ia => ia.Reviewer)
                .FirstOrDefaultAsync(ia => ia.Id == applicationId);

            if (application == null)
                throw new KeyNotFoundException("Application not found");

            application.ApplicationStatus = request.Status;
            application.AdminRemarks = request.Remarks;
            application.ReviewedBy = adminId;
            application.ReviewedAt = DateTime.UtcNow;
            application.UpdatedAt = DateTime.UtcNow;

            // If approved, update user role
            if (request.Status == "approved")
            {
                var instructorRole = await _context.Roles.FirstOrDefaultAsync(r => r.Name == "Instructor");
                if (instructorRole != null)
                {
                    application.User.RoleId = instructorRole.Id;
                    application.User.UpdatedAt = DateTime.UtcNow;
                }
            }

            await _context.SaveChangesAsync();
            return MapToInstructorApplicationDto(application);
        }

        public async Task<List<InstructorApplicationDto>> GetInstructorApplicationsByUserAsync(int userId)
        {
            var applications = await _context.InstructorApplications
                .Include(ia => ia.User)
                .Include(ia => ia.Reviewer)
                .Where(ia => ia.UserId == userId)
                .ToListAsync();

            return applications.Select(MapToInstructorApplicationDto).ToList();
        }

        private UserDto MapToUserDto(User user)
        {
            return new UserDto
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Role = user.Role?.Name ?? "Student",
                ProfileImage = user.ProfileImage,
                Phone = user.Phone,
                Bio = user.Bio,
                IsActive = user.IsActive,
                EmailVerified = user.EmailVerified,
                CreatedAt = user.CreatedAt
            };
        }

        private InstructorApplicationDto MapToInstructorApplicationDto(InstructorApplication application)
        {
            return new InstructorApplicationDto
            {
                Id = application.Id,
                User = MapToUserDto(application.User),
                ApplicationStatus = application.ApplicationStatus,
                Qualifications = application.Qualifications,
                ExperienceYears = application.ExperienceYears,
                PortfolioUrl = application.PortfolioUrl,
                Motivation = application.Motivation,
                AdminRemarks = application.AdminRemarks,
                Reviewer = application.Reviewer != null ? MapToUserDto(application.Reviewer) : null,
                ReviewedAt = application.ReviewedAt,
                CreatedAt = application.CreatedAt
            };
        }
    }
}
