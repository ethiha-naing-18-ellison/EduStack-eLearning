using EduStack.API.Data;
using EduStack.API.DTOs;
using EduStack.API.Models;
using Microsoft.EntityFrameworkCore;

namespace EduStack.API.Services
{
    public class AdminService : IAdminService
    {
        private readonly EduStackDbContext _context;
        private readonly ILogger<AdminService> _logger;

        public AdminService(EduStackDbContext context, ILogger<AdminService> logger)
        {
            _context = context;
            _logger = logger;
        }

        // User Management
        public async Task<List<UserDto>> GetAllUsersAsync(int page = 1, int pageSize = 10, string? search = null, string? role = null)
        {
            var query = _context.Users
                .Include(u => u.Role)
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(u => u.Name.Contains(search) || u.Email.Contains(search));
            }

            if (!string.IsNullOrEmpty(role))
            {
                query = query.Where(u => u.Role!.Name == role);
            }

            var users = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return users.Select(MapToUserDto).ToList();
        }

        public async Task<UserDto> GetUserByIdAsync(int userId)
        {
            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
                return null!;

            return MapToUserDto(user);
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

        public async Task<bool> DeleteUserAsync(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                return false;

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }

        // Course Management
        public async Task<List<CourseDto>> GetAllCoursesAsync(int page = 1, int pageSize = 10, string? search = null, int? categoryId = null)
        {
            var query = _context.Courses
                .Include(c => c.Instructor)
                .Include(c => c.Category)
                .Include(c => c.Reviews)
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(c => c.Title.Contains(search) || c.Description!.Contains(search));
            }

            if (categoryId.HasValue)
            {
                query = query.Where(c => c.CategoryId == categoryId.Value);
            }

            var courses = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return courses.Select(MapToCourseDto).ToList();
        }

        public async Task<bool> PublishCourseAsync(int courseId)
        {
            var course = await _context.Courses.FindAsync(courseId);
            if (course == null)
                return false;

            course.IsPublished = true;
            course.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UnpublishCourseAsync(int courseId)
        {
            var course = await _context.Courses.FindAsync(courseId);
            if (course == null)
                return false;

            course.IsPublished = false;
            course.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteCourseAsync(int courseId)
        {
            var course = await _context.Courses.FindAsync(courseId);
            if (course == null)
                return false;

            _context.Courses.Remove(course);
            await _context.SaveChangesAsync();
            return true;
        }

        // Instructor Applications
        public async Task<List<InstructorApplicationDto>> GetPendingInstructorApplicationsAsync()
        {
            var applications = await _context.InstructorApplications
                .Include(ia => ia.User)
                .Include(ia => ia.Reviewer)
                .Where(ia => ia.ApplicationStatus == "pending")
                .OrderByDescending(ia => ia.CreatedAt)
                .ToListAsync();

            return applications.Select(MapToInstructorApplicationDto).ToList();
        }

        public async Task<List<InstructorApplicationDto>> GetAllInstructorApplicationsAsync(int page = 1, int pageSize = 10)
        {
            var applications = await _context.InstructorApplications
                .Include(ia => ia.User)
                .Include(ia => ia.Reviewer)
                .OrderByDescending(ia => ia.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
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

        // Reviews Management
        public async Task<List<ReviewDto>> GetPendingReviewsAsync(int page = 1, int pageSize = 10)
        {
            var reviews = await _context.Reviews
                .Include(r => r.Student)
                .Include(r => r.Course)
                .Where(r => !r.IsApproved)
                .OrderByDescending(r => r.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return reviews.Select(MapToReviewDto).ToList();
        }

        public async Task<bool> ApproveReviewAsync(int reviewId)
        {
            var review = await _context.Reviews.FindAsync(reviewId);
            if (review == null)
                return false;

            review.IsApproved = true;
            review.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RejectReviewAsync(int reviewId)
        {
            var review = await _context.Reviews.FindAsync(reviewId);
            if (review == null)
                return false;

            _context.Reviews.Remove(review);
            await _context.SaveChangesAsync();
            return true;
        }

        // Categories Management
        public async Task<List<CategoryDto>> GetAllCategoriesAsync()
        {
            var categories = await _context.Categories
                .OrderBy(c => c.Name)
                .ToListAsync();

            return categories.Select(MapToCategoryDto).ToList();
        }

        public async Task<CategoryDto> CreateCategoryAsync(CreateCategoryRequest request)
        {
            var category = new Category
            {
                Name = request.Name,
                Description = request.Description,
                ParentId = request.ParentId,
                IconUrl = request.IconUrl,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            return MapToCategoryDto(category);
        }

        public async Task<CategoryDto> UpdateCategoryAsync(int categoryId, UpdateCategoryRequest request)
        {
            var category = await _context.Categories.FindAsync(categoryId);
            if (category == null)
                throw new KeyNotFoundException("Category not found");

            if (!string.IsNullOrEmpty(request.Name))
                category.Name = request.Name;
            if (request.Description != null)
                category.Description = request.Description;
            if (request.ParentId.HasValue)
                category.ParentId = request.ParentId.Value;
            if (request.IconUrl != null)
                category.IconUrl = request.IconUrl;
            if (request.IsActive.HasValue)
                category.IsActive = request.IsActive.Value;

            await _context.SaveChangesAsync();
            return MapToCategoryDto(category);
        }

        public async Task<bool> DeleteCategoryAsync(int categoryId)
        {
            var category = await _context.Categories.FindAsync(categoryId);
            if (category == null)
                return false;

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
            return true;
        }

        // Analytics and Reports
        public async Task<object> GetDashboardStatsAsync()
        {
            var totalUsers = await _context.Users.CountAsync();
            var totalCourses = await _context.Courses.CountAsync();
            var totalEnrollments = await _context.Enrollments.CountAsync();
            var totalRevenue = await _context.Payments
                .Where(p => p.PaymentStatus == "completed")
                .SumAsync(p => p.Amount);
            var pendingApplications = await _context.InstructorApplications
                .CountAsync(ia => ia.ApplicationStatus == "pending");
            var pendingReviews = await _context.Reviews
                .CountAsync(r => !r.IsApproved);

            return new
            {
                TotalUsers = totalUsers,
                TotalCourses = totalCourses,
                TotalEnrollments = totalEnrollments,
                TotalRevenue = totalRevenue,
                PendingApplications = pendingApplications,
                PendingReviews = pendingReviews
            };
        }

        public async Task<List<PaymentDto>> GetRevenueReportAsync(DateTime? fromDate = null, DateTime? toDate = null)
        {
            var query = _context.Payments
                .Include(p => p.User)
                .Include(p => p.Course)
                .Where(p => p.PaymentStatus == "completed");

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

        public async Task<List<CourseDto>> GetTopCoursesAsync(int count = 10)
        {
            var courses = await _context.Courses
                .Include(c => c.Instructor)
                .Include(c => c.Category)
                .Include(c => c.Reviews)
                .Where(c => c.IsPublished)
                .OrderByDescending(c => c.Enrollments.Count)
                .Take(count)
                .ToListAsync();

            return courses.Select(MapToCourseDto).ToList();
        }

        public async Task<List<UserDto>> GetTopInstructorsAsync(int count = 10)
        {
            var instructors = await _context.Users
                .Include(u => u.Role)
                .Include(u => u.Courses)
                .Where(u => u.Role!.Name == "Instructor")
                .OrderByDescending(u => u.Courses.Count)
                .Take(count)
                .ToListAsync();

            return instructors.Select(MapToUserDto).ToList();
        }

        // Mapping methods
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

        private CourseDto MapToCourseDto(Course course)
        {
            return new CourseDto
            {
                Id = course.Id,
                Title = course.Title,
                Description = course.Description,
                Price = course.Price,
                Instructor = new UserDto
                {
                    Id = course.Instructor.Id,
                    Name = course.Instructor.Name,
                    Email = course.Instructor.Email,
                    Role = course.Instructor.Role?.Name ?? "Student",
                    ProfileImage = course.Instructor.ProfileImage,
                    Phone = course.Instructor.Phone,
                    Bio = course.Instructor.Bio,
                    IsActive = course.Instructor.IsActive,
                    EmailVerified = course.Instructor.EmailVerified,
                    CreatedAt = course.Instructor.CreatedAt
                },
                Category = new CategoryDto
                {
                    Id = course.Category.Id,
                    Name = course.Category.Name,
                    Description = course.Category.Description,
                    ParentId = course.Category.ParentId,
                    IconUrl = course.Category.IconUrl,
                    IsActive = course.Category.IsActive,
                    CreatedAt = course.Category.CreatedAt
                },
                ThumbnailUrl = course.ThumbnailUrl,
                IsPublished = course.IsPublished,
                DifficultyLevel = course.DifficultyLevel,
                DurationHours = course.DurationHours,
                Language = course.Language,
                CreatedAt = course.CreatedAt,
                UpdatedAt = course.UpdatedAt,
                AverageRating = course.Reviews.Any() ? course.Reviews.Average(r => r.Rating) : 0,
                TotalReviews = course.Reviews.Count,
                TotalStudents = course.Enrollments.Count
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

        private ReviewDto MapToReviewDto(Review review)
        {
            return new ReviewDto
            {
                Id = review.Id,
                Student = MapToUserDto(review.Student),
                Course = new CourseDto
                {
                    Id = review.Course.Id,
                    Title = review.Course.Title,
                    Description = review.Course.Description,
                    Price = review.Course.Price,
                    ThumbnailUrl = review.Course.ThumbnailUrl,
                    IsPublished = review.Course.IsPublished,
                    DifficultyLevel = review.Course.DifficultyLevel,
                    DurationHours = review.Course.DurationHours,
                    Language = review.Course.Language,
                    CreatedAt = review.Course.CreatedAt,
                    UpdatedAt = review.Course.UpdatedAt
                },
                Rating = review.Rating,
                Comment = review.Comment,
                IsApproved = review.IsApproved,
                CreatedAt = review.CreatedAt,
                UpdatedAt = review.UpdatedAt
            };
        }

        private CategoryDto MapToCategoryDto(Category category)
        {
            return new CategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                ParentId = category.ParentId,
                IconUrl = category.IconUrl,
                IsActive = category.IsActive,
                CreatedAt = category.CreatedAt
            };
        }

        private PaymentDto MapToPaymentDto(Payment payment)
        {
            return new PaymentDto
            {
                Id = payment.Id,
                User = MapToUserDto(payment.User),
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
