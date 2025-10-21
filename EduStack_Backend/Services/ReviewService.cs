using EduStack.API.Data;
using EduStack.API.DTOs;
using EduStack.API.Models;
using Microsoft.EntityFrameworkCore;

namespace EduStack.API.Services
{
    public class ReviewService : IReviewService
    {
        private readonly EduStackDbContext _context;
        private readonly ILogger<ReviewService> _logger;

        public ReviewService(EduStackDbContext context, ILogger<ReviewService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<ReviewDto> CreateReviewAsync(int studentId, CreateReviewRequest request)
        {
            // Check if user has already reviewed this course
            var existingReview = await _context.Reviews
                .FirstOrDefaultAsync(r => r.StudentId == studentId && r.CourseId == request.CourseId);

            if (existingReview != null)
                throw new InvalidOperationException("You have already reviewed this course");

            // Check if user is enrolled in the course
            var enrollment = await _context.Enrollments
                .FirstOrDefaultAsync(e => e.StudentId == studentId && e.CourseId == request.CourseId);

            if (enrollment == null)
                throw new UnauthorizedAccessException("You must be enrolled in the course to review it");

            var review = new Review
            {
                StudentId = studentId,
                CourseId = request.CourseId,
                Rating = request.Rating,
                Comment = request.Comment,
                IsApproved = false, // Requires admin approval
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();

            return await GetReviewByIdAsync(review.Id);
        }

        public async Task<ReviewDto> UpdateReviewAsync(int reviewId, int studentId, UpdateReviewRequest request)
        {
            var review = await _context.Reviews
                .FirstOrDefaultAsync(r => r.Id == reviewId && r.StudentId == studentId);

            if (review == null)
                throw new KeyNotFoundException("Review not found");

            if (request.Rating.HasValue)
                review.Rating = request.Rating.Value;
            if (request.Comment != null)
                review.Comment = request.Comment;

            review.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return await GetReviewByIdAsync(reviewId);
        }

        public async Task<bool> DeleteReviewAsync(int reviewId, int studentId)
        {
            var review = await _context.Reviews
                .FirstOrDefaultAsync(r => r.Id == reviewId && r.StudentId == studentId);

            if (review == null)
                return false;

            _context.Reviews.Remove(review);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<ReviewDto> GetReviewByIdAsync(int reviewId)
        {
            var review = await _context.Reviews
                .Include(r => r.Student)
                .Include(r => r.Course)
                .FirstOrDefaultAsync(r => r.Id == reviewId);

            if (review == null)
                return null!;

            return MapToReviewDto(review);
        }

        public async Task<List<ReviewDto>> GetCourseReviewsAsync(int courseId, int page = 1, int pageSize = 10)
        {
            var reviews = await _context.Reviews
                .Include(r => r.Student)
                .Include(r => r.Course)
                .Where(r => r.CourseId == courseId && r.IsApproved)
                .OrderByDescending(r => r.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return reviews.Select(MapToReviewDto).ToList();
        }

        public async Task<List<ReviewDto>> GetUserReviewsAsync(int userId, int page = 1, int pageSize = 10)
        {
            var reviews = await _context.Reviews
                .Include(r => r.Student)
                .Include(r => r.Course)
                .Where(r => r.StudentId == userId)
                .OrderByDescending(r => r.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return reviews.Select(MapToReviewDto).ToList();
        }

        public async Task<ReviewStatsDto> GetCourseReviewStatsAsync(int courseId)
        {
            var reviews = await _context.Reviews
                .Where(r => r.CourseId == courseId && r.IsApproved)
                .ToListAsync();

            if (!reviews.Any())
            {
                return new ReviewStatsDto
                {
                    AverageRating = 0,
                    TotalReviews = 0,
                    Rating5 = 0,
                    Rating4 = 0,
                    Rating3 = 0,
                    Rating2 = 0,
                    Rating1 = 0
                };
            }

            return new ReviewStatsDto
            {
                AverageRating = reviews.Average(r => r.Rating),
                TotalReviews = reviews.Count,
                Rating5 = reviews.Count(r => r.Rating == 5),
                Rating4 = reviews.Count(r => r.Rating == 4),
                Rating3 = reviews.Count(r => r.Rating == 3),
                Rating2 = reviews.Count(r => r.Rating == 2),
                Rating1 = reviews.Count(r => r.Rating == 1)
            };
        }

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

        public async Task<bool> HasUserReviewedCourseAsync(int userId, int courseId)
        {
            return await _context.Reviews
                .AnyAsync(r => r.StudentId == userId && r.CourseId == courseId);
        }

        private ReviewDto MapToReviewDto(Review review)
        {
            return new ReviewDto
            {
                Id = review.Id,
                Student = new UserDto
                {
                    Id = review.Student.Id,
                    Name = review.Student.Name,
                    Email = review.Student.Email,
                    Role = review.Student.Role?.Name ?? "Student",
                    ProfileImage = review.Student.ProfileImage,
                    Phone = review.Student.Phone,
                    Bio = review.Student.Bio,
                    IsActive = review.Student.IsActive,
                    EmailVerified = review.Student.EmailVerified,
                    CreatedAt = review.Student.CreatedAt
                },
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
    }
}
