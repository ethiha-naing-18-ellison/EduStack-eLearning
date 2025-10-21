using EduStack.API.Data;
using EduStack.API.DTOs;
using EduStack.API.Models;
using Microsoft.EntityFrameworkCore;

namespace EduStack.API.Services
{
    public class EnrollmentService : IEnrollmentService
    {
        private readonly EduStackDbContext _context;
        private readonly ILogger<EnrollmentService> _logger;

        public EnrollmentService(EduStackDbContext context, ILogger<EnrollmentService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<EnrollmentDto> EnrollInCourseAsync(int studentId, EnrollRequest request)
        {
            // Check if already enrolled
            var existingEnrollment = await _context.Enrollments
                .FirstOrDefaultAsync(e => e.StudentId == studentId && e.CourseId == request.CourseId);

            if (existingEnrollment != null)
                throw new InvalidOperationException("Already enrolled in this course");

            var course = await _context.Courses.FindAsync(request.CourseId);
            if (course == null)
                throw new KeyNotFoundException("Course not found");

            var enrollment = new Enrollment
            {
                StudentId = studentId,
                CourseId = request.CourseId,
                EnrollmentDate = DateTime.UtcNow,
                ProgressPercentage = 0,
                IsActive = true,
                PaymentStatus = course.Price > 0 ? "pending" : "completed"
            };

            _context.Enrollments.Add(enrollment);
            await _context.SaveChangesAsync();

            return await GetEnrollmentDtoAsync(enrollment.Id);
        }

        public async Task<bool> UnenrollFromCourseAsync(int studentId, int courseId)
        {
            var enrollment = await _context.Enrollments
                .FirstOrDefaultAsync(e => e.StudentId == studentId && e.CourseId == courseId);

            if (enrollment == null)
                return false;

            _context.Enrollments.Remove(enrollment);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<EnrollmentDto>> GetStudentEnrollmentsAsync(int studentId, int page = 1, int pageSize = 10)
        {
            var enrollments = await _context.Enrollments
                .Include(e => e.Student)
                .Include(e => e.Course)
                .Where(e => e.StudentId == studentId)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return enrollments.Select(MapToEnrollmentDto).ToList();
        }

        public async Task<List<EnrollmentDto>> GetCourseEnrollmentsAsync(int courseId, int page = 1, int pageSize = 10)
        {
            var enrollments = await _context.Enrollments
                .Include(e => e.Student)
                .Include(e => e.Course)
                .Where(e => e.CourseId == courseId)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return enrollments.Select(MapToEnrollmentDto).ToList();
        }

        public async Task<EnrollmentProgressDto> GetEnrollmentProgressAsync(int studentId, int courseId)
        {
            var enrollment = await _context.Enrollments
                .Include(e => e.Course)
                .FirstOrDefaultAsync(e => e.StudentId == studentId && e.CourseId == courseId);

            if (enrollment == null)
                throw new KeyNotFoundException("Enrollment not found");

            var course = await _context.Courses
                .Include(c => c.CourseSections)
                    .ThenInclude(cs => cs.Lessons)
                .FirstOrDefaultAsync(c => c.Id == courseId);

            if (course == null)
                throw new KeyNotFoundException("Course not found");

            var sections = course.CourseSections
                .OrderBy(cs => cs.OrderIndex)
                .Select(cs => new SectionProgressDto
                {
                    SectionId = cs.Id,
                    SectionTitle = cs.Title,
                    OrderIndex = cs.OrderIndex,
                    Lessons = cs.Lessons
                        .OrderBy(l => l.OrderIndex)
                        .Select(l => new LessonProgressDto
                        {
                            LessonId = l.Id,
                            LessonTitle = l.Title,
                            LessonType = l.LessonType,
                            DurationMinutes = l.DurationMinutes,
                            OrderIndex = l.OrderIndex,
                            IsCompleted = false, // Would need to check lesson progress
                            CompletionDate = null,
                            TimeSpentMinutes = 0,
                            LastPositionSeconds = 0,
                            IsPreview = l.IsPreview
                        }).ToList(),
                    SectionProgress = 0 // Would need to calculate
                }).ToList();

            return new EnrollmentProgressDto
            {
                CourseId = course.Id,
                CourseTitle = course.Title,
                ThumbnailUrl = course.ThumbnailUrl,
                ProgressPercentage = enrollment.ProgressPercentage,
                EnrollmentDate = enrollment.EnrollmentDate,
                CompletionDate = enrollment.CompletionDate,
                IsActive = enrollment.IsActive,
                PaymentStatus = enrollment.PaymentStatus,
                Sections = sections
            };
        }

        public async Task<bool> UpdateLessonProgressAsync(int studentId, UpdateLessonProgressRequest request)
        {
            var lessonProgress = await _context.LessonProgresses
                .FirstOrDefaultAsync(lp => lp.StudentId == studentId && lp.LessonId == request.LessonId);

            if (lessonProgress == null)
            {
                lessonProgress = new LessonProgress
                {
                    StudentId = studentId,
                    LessonId = request.LessonId,
                    IsCompleted = request.IsCompleted ?? false,
                    CompletionDate = request.IsCompleted == true ? DateTime.UtcNow : null,
                    TimeSpentMinutes = request.TimeSpentMinutes ?? 0,
                    LastPositionSeconds = request.LastPositionSeconds ?? 0,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                _context.LessonProgresses.Add(lessonProgress);
            }
            else
            {
                if (request.IsCompleted.HasValue)
                {
                    lessonProgress.IsCompleted = request.IsCompleted.Value;
                    lessonProgress.CompletionDate = request.IsCompleted.Value ? DateTime.UtcNow : null;
                }
                if (request.TimeSpentMinutes.HasValue)
                    lessonProgress.TimeSpentMinutes = request.TimeSpentMinutes.Value;
                if (request.LastPositionSeconds.HasValue)
                    lessonProgress.LastPositionSeconds = request.LastPositionSeconds.Value;

                lessonProgress.UpdatedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();

            // Update course progress
            await UpdateCourseProgressAsync(studentId, request.LessonId);
            return true;
        }

        public async Task<List<LessonProgressDto>> GetStudentLessonProgressAsync(int studentId, int courseId)
        {
            var course = await _context.Courses
                .Include(c => c.CourseSections)
                    .ThenInclude(cs => cs.Lessons)
                .FirstOrDefaultAsync(c => c.Id == courseId);

            if (course == null)
                throw new KeyNotFoundException("Course not found");

            var lessonProgresses = await _context.LessonProgresses
                .Where(lp => lp.StudentId == studentId)
                .ToListAsync();

            var lessons = course.CourseSections
                .SelectMany(cs => cs.Lessons)
                .OrderBy(l => l.OrderIndex)
                .Select(l =>
                {
                    var progress = lessonProgresses.FirstOrDefault(lp => lp.LessonId == l.Id);
                    return new LessonProgressDto
                    {
                        LessonId = l.Id,
                        LessonTitle = l.Title,
                        LessonType = l.LessonType,
                        DurationMinutes = l.DurationMinutes,
                        OrderIndex = l.OrderIndex,
                        IsCompleted = progress?.IsCompleted ?? false,
                        CompletionDate = progress?.CompletionDate,
                        TimeSpentMinutes = progress?.TimeSpentMinutes ?? 0,
                        LastPositionSeconds = progress?.LastPositionSeconds ?? 0,
                        IsPreview = l.IsPreview
                    };
                }).ToList();

            return lessons;
        }

        public async Task<bool> CompleteCourseAsync(int studentId, int courseId)
        {
            var enrollment = await _context.Enrollments
                .FirstOrDefaultAsync(e => e.StudentId == studentId && e.CourseId == courseId);

            if (enrollment == null)
                return false;

            enrollment.CompletionDate = DateTime.UtcNow;
            enrollment.ProgressPercentage = 100;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> IsEnrolledAsync(int studentId, int courseId)
        {
            return await _context.Enrollments
                .AnyAsync(e => e.StudentId == studentId && e.CourseId == courseId && e.IsActive);
        }

        public async Task<decimal> CalculateCourseProgressAsync(int studentId, int courseId)
        {
            var course = await _context.Courses
                .Include(c => c.CourseSections)
                    .ThenInclude(cs => cs.Lessons)
                .FirstOrDefaultAsync(c => c.Id == courseId);

            if (course == null)
                return 0;

            var totalLessons = course.CourseSections
                .SelectMany(cs => cs.Lessons)
                .Count();

            if (totalLessons == 0)
                return 0;

            var completedLessons = await _context.LessonProgresses
                .Where(lp => lp.StudentId == studentId && lp.IsCompleted)
                .CountAsync();

            return (decimal)completedLessons / totalLessons * 100;
        }

        private async Task UpdateCourseProgressAsync(int studentId, int lessonId)
        {
            var lesson = await _context.Lessons
                .Include(l => l.Section)
                    .ThenInclude(s => s.Course)
                .FirstOrDefaultAsync(l => l.Id == lessonId);

            if (lesson == null)
                return;

            var courseId = lesson.Section.CourseId;
            var progress = await CalculateCourseProgressAsync(studentId, courseId);

            var enrollment = await _context.Enrollments
                .FirstOrDefaultAsync(e => e.StudentId == studentId && e.CourseId == courseId);

            if (enrollment != null)
            {
                enrollment.ProgressPercentage = progress;
                if (progress >= 100)
                {
                    enrollment.CompletionDate = DateTime.UtcNow;
                }
                await _context.SaveChangesAsync();
            }
        }

        private async Task<EnrollmentDto> GetEnrollmentDtoAsync(int enrollmentId)
        {
            var enrollment = await _context.Enrollments
                .Include(e => e.Student)
                .Include(e => e.Course)
                .FirstOrDefaultAsync(e => e.Id == enrollmentId);

            if (enrollment == null)
                throw new KeyNotFoundException("Enrollment not found");

            return MapToEnrollmentDto(enrollment);
        }

        private EnrollmentDto MapToEnrollmentDto(Enrollment enrollment)
        {
            return new EnrollmentDto
            {
                Id = enrollment.Id,
                Student = new UserDto
                {
                    Id = enrollment.Student.Id,
                    Name = enrollment.Student.Name,
                    Email = enrollment.Student.Email,
                    Role = enrollment.Student.Role?.Name ?? "Student",
                    ProfileImage = enrollment.Student.ProfileImage,
                    Phone = enrollment.Student.Phone,
                    Bio = enrollment.Student.Bio,
                    IsActive = enrollment.Student.IsActive,
                    EmailVerified = enrollment.Student.EmailVerified,
                    CreatedAt = enrollment.Student.CreatedAt
                },
                Course = new CourseDto
                {
                    Id = enrollment.Course.Id,
                    Title = enrollment.Course.Title,
                    Description = enrollment.Course.Description,
                    Price = enrollment.Course.Price,
                    ThumbnailUrl = enrollment.Course.ThumbnailUrl,
                    IsPublished = enrollment.Course.IsPublished,
                    DifficultyLevel = enrollment.Course.DifficultyLevel,
                    DurationHours = enrollment.Course.DurationHours,
                    Language = enrollment.Course.Language,
                    CreatedAt = enrollment.Course.CreatedAt,
                    UpdatedAt = enrollment.Course.UpdatedAt
                },
                EnrollmentDate = enrollment.EnrollmentDate,
                ProgressPercentage = enrollment.ProgressPercentage,
                CompletionDate = enrollment.CompletionDate,
                IsActive = enrollment.IsActive,
                PaymentStatus = enrollment.PaymentStatus
            };
        }
    }
}
