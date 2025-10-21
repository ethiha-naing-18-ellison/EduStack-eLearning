using EduStack.API.Data;
using EduStack.API.DTOs;
using EduStack.API.Models;
using Microsoft.EntityFrameworkCore;

namespace EduStack.API.Services
{
    public class CourseService : ICourseService
    {
        private readonly EduStackDbContext _context;
        private readonly ILogger<CourseService> _logger;

        public CourseService(EduStackDbContext context, ILogger<CourseService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<CourseDto> GetCourseByIdAsync(int id)
        {
            var course = await _context.Courses
                .Include(c => c.Instructor)
                .Include(c => c.Category)
                .Include(c => c.CourseSections)
                    .ThenInclude(cs => cs.Lessons)
                        .ThenInclude(l => l.Resources)
                .Include(c => c.Reviews)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (course == null)
                return null!;

            return MapToCourseDto(course);
        }

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

        public async Task<List<CourseDto>> GetCoursesByInstructorAsync(int instructorId, int page = 1, int pageSize = 10)
        {
            var courses = await _context.Courses
                .Include(c => c.Instructor)
                .Include(c => c.Category)
                .Include(c => c.Reviews)
                .Where(c => c.InstructorId == instructorId)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return courses.Select(MapToCourseDto).ToList();
        }

        public async Task<List<CourseDto>> GetPublishedCoursesAsync(int page = 1, int pageSize = 10, string? search = null, int? categoryId = null)
        {
            var query = _context.Courses
                .Include(c => c.Instructor)
                .Include(c => c.Category)
                .Include(c => c.Reviews)
                .Where(c => c.IsPublished)
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

        public async Task<CourseDto> CreateCourseAsync(int instructorId, CreateCourseRequest request)
        {
            var course = new Course
            {
                Title = request.Title,
                Description = request.Description,
                Price = request.Price,
                InstructorId = instructorId,
                CategoryId = request.CategoryId,
                ThumbnailUrl = request.ThumbnailUrl,
                DifficultyLevel = request.DifficultyLevel,
                DurationHours = request.DurationHours,
                Language = request.Language,
                IsPublished = false,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Courses.Add(course);
            await _context.SaveChangesAsync();

            return await GetCourseByIdAsync(course.Id);
        }

        public async Task<CourseDto> UpdateCourseAsync(int courseId, int instructorId, UpdateCourseRequest request)
        {
            var course = await _context.Courses.FindAsync(courseId);
            if (course == null)
                throw new KeyNotFoundException("Course not found");

            if (course.InstructorId != instructorId)
                throw new UnauthorizedAccessException("You can only update your own courses");

            if (!string.IsNullOrEmpty(request.Title))
                course.Title = request.Title;
            if (request.Description != null)
                course.Description = request.Description;
            if (request.Price.HasValue)
                course.Price = request.Price.Value;
            if (request.CategoryId.HasValue)
                course.CategoryId = request.CategoryId.Value;
            if (request.ThumbnailUrl != null)
                course.ThumbnailUrl = request.ThumbnailUrl;
            if (!string.IsNullOrEmpty(request.DifficultyLevel))
                course.DifficultyLevel = request.DifficultyLevel;
            if (request.DurationHours.HasValue)
                course.DurationHours = request.DurationHours.Value;
            if (!string.IsNullOrEmpty(request.Language))
                course.Language = request.Language;
            if (request.IsPublished.HasValue)
                course.IsPublished = request.IsPublished.Value;

            course.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return await GetCourseByIdAsync(courseId);
        }

        public async Task<bool> DeleteCourseAsync(int courseId, int instructorId)
        {
            var course = await _context.Courses.FindAsync(courseId);
            if (course == null)
                return false;

            if (course.InstructorId != instructorId)
                throw new UnauthorizedAccessException("You can only delete your own courses");

            _context.Courses.Remove(course);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> PublishCourseAsync(int courseId, int instructorId)
        {
            var course = await _context.Courses.FindAsync(courseId);
            if (course == null)
                return false;

            if (course.InstructorId != instructorId)
                throw new UnauthorizedAccessException("You can only publish your own courses");

            course.IsPublished = true;
            course.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UnpublishCourseAsync(int courseId, int instructorId)
        {
            var course = await _context.Courses.FindAsync(courseId);
            if (course == null)
                return false;

            if (course.InstructorId != instructorId)
                throw new UnauthorizedAccessException("You can only unpublish your own courses");

            course.IsPublished = false;
            course.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        // Course Sections
        public async Task<CourseSectionDto> CreateCourseSectionAsync(int courseId, int instructorId, CreateCourseSectionRequest request)
        {
            var course = await _context.Courses.FindAsync(courseId);
            if (course == null)
                throw new KeyNotFoundException("Course not found");

            if (course.InstructorId != instructorId)
                throw new UnauthorizedAccessException("You can only add sections to your own courses");

            var section = new CourseSection
            {
                CourseId = courseId,
                Title = request.Title,
                Description = request.Description,
                OrderIndex = request.OrderIndex,
                IsPublished = false,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.CourseSections.Add(section);
            await _context.SaveChangesAsync();

            return MapToCourseSectionDto(section);
        }

        public async Task<CourseSectionDto> UpdateCourseSectionAsync(int sectionId, int instructorId, UpdateCourseSectionRequest request)
        {
            var section = await _context.CourseSections
                .Include(cs => cs.Course)
                .FirstOrDefaultAsync(cs => cs.Id == sectionId);

            if (section == null)
                throw new KeyNotFoundException("Section not found");

            if (section.Course.InstructorId != instructorId)
                throw new UnauthorizedAccessException("You can only update sections of your own courses");

            if (!string.IsNullOrEmpty(request.Title))
                section.Title = request.Title;
            if (request.Description != null)
                section.Description = request.Description;
            if (request.OrderIndex.HasValue)
                section.OrderIndex = request.OrderIndex.Value;
            if (request.IsPublished.HasValue)
                section.IsPublished = request.IsPublished.Value;

            section.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return MapToCourseSectionDto(section);
        }

        public async Task<bool> DeleteCourseSectionAsync(int sectionId, int instructorId)
        {
            var section = await _context.CourseSections
                .Include(cs => cs.Course)
                .FirstOrDefaultAsync(cs => cs.Id == sectionId);

            if (section == null)
                return false;

            if (section.Course.InstructorId != instructorId)
                throw new UnauthorizedAccessException("You can only delete sections of your own courses");

            _context.CourseSections.Remove(section);
            await _context.SaveChangesAsync();
            return true;
        }

        // Lessons
        public async Task<LessonDto> CreateLessonAsync(int sectionId, int instructorId, CreateLessonRequest request)
        {
            var section = await _context.CourseSections
                .Include(cs => cs.Course)
                .FirstOrDefaultAsync(cs => cs.Id == sectionId);

            if (section == null)
                throw new KeyNotFoundException("Section not found");

            if (section.Course.InstructorId != instructorId)
                throw new UnauthorizedAccessException("You can only add lessons to your own courses");

            var lesson = new Lesson
            {
                SectionId = sectionId,
                Title = request.Title,
                Description = request.Description,
                LessonType = request.LessonType,
                Content = request.Content,
                VideoUrl = request.VideoUrl,
                FileUrl = request.FileUrl,
                DurationMinutes = request.DurationMinutes,
                OrderIndex = request.OrderIndex,
                IsPreview = request.IsPreview,
                IsPublished = false,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Lessons.Add(lesson);
            await _context.SaveChangesAsync();

            return MapToLessonDto(lesson);
        }

        public async Task<LessonDto> UpdateLessonAsync(int lessonId, int instructorId, UpdateLessonRequest request)
        {
            var lesson = await _context.Lessons
                .Include(l => l.Section)
                    .ThenInclude(s => s.Course)
                .FirstOrDefaultAsync(l => l.Id == lessonId);

            if (lesson == null)
                throw new KeyNotFoundException("Lesson not found");

            if (lesson.Section.Course.InstructorId != instructorId)
                throw new UnauthorizedAccessException("You can only update lessons of your own courses");

            if (!string.IsNullOrEmpty(request.Title))
                lesson.Title = request.Title;
            if (request.Description != null)
                lesson.Description = request.Description;
            if (!string.IsNullOrEmpty(request.LessonType))
                lesson.LessonType = request.LessonType;
            if (request.Content != null)
                lesson.Content = request.Content;
            if (request.VideoUrl != null)
                lesson.VideoUrl = request.VideoUrl;
            if (request.FileUrl != null)
                lesson.FileUrl = request.FileUrl;
            if (request.DurationMinutes.HasValue)
                lesson.DurationMinutes = request.DurationMinutes.Value;
            if (request.OrderIndex.HasValue)
                lesson.OrderIndex = request.OrderIndex.Value;
            if (request.IsPublished.HasValue)
                lesson.IsPublished = request.IsPublished.Value;
            if (request.IsPreview.HasValue)
                lesson.IsPreview = request.IsPreview.Value;

            lesson.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return MapToLessonDto(lesson);
        }

        public async Task<bool> DeleteLessonAsync(int lessonId, int instructorId)
        {
            var lesson = await _context.Lessons
                .Include(l => l.Section)
                    .ThenInclude(s => s.Course)
                .FirstOrDefaultAsync(l => l.Id == lessonId);

            if (lesson == null)
                return false;

            if (lesson.Section.Course.InstructorId != instructorId)
                throw new UnauthorizedAccessException("You can only delete lessons of your own courses");

            _context.Lessons.Remove(lesson);
            await _context.SaveChangesAsync();
            return true;
        }

        // Resources
        public async Task<ResourceDto> CreateResourceAsync(int lessonId, int instructorId, CreateResourceRequest request)
        {
            var lesson = await _context.Lessons
                .Include(l => l.Section)
                    .ThenInclude(s => s.Course)
                .FirstOrDefaultAsync(l => l.Id == lessonId);

            if (lesson == null)
                throw new KeyNotFoundException("Lesson not found");

            if (lesson.Section.Course.InstructorId != instructorId)
                throw new UnauthorizedAccessException("You can only add resources to your own courses");

            var resource = new Resource
            {
                LessonId = lessonId,
                FileName = request.FileName,
                FileUrl = request.FileUrl,
                FileType = request.FileType,
                FileSize = request.FileSize,
                DownloadCount = 0,
                CreatedAt = DateTime.UtcNow
            };

            _context.Resources.Add(resource);
            await _context.SaveChangesAsync();

            return MapToResourceDto(resource);
        }

        public async Task<bool> DeleteResourceAsync(int resourceId, int instructorId)
        {
            var resource = await _context.Resources
                .Include(r => r.Lesson)
                    .ThenInclude(l => l.Section)
                        .ThenInclude(s => s.Course)
                .FirstOrDefaultAsync(r => r.Id == resourceId);

            if (resource == null)
                return false;

            if (resource.Lesson.Section.Course.InstructorId != instructorId)
                throw new UnauthorizedAccessException("You can only delete resources of your own courses");

            _context.Resources.Remove(resource);
            await _context.SaveChangesAsync();
            return true;
        }

        // Categories
        public async Task<List<CategoryDto>> GetAllCategoriesAsync()
        {
            var categories = await _context.Categories
                .Where(c => c.IsActive)
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

        // Mapping methods
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
                Sections = course.CourseSections.Select(MapToCourseSectionDto).ToList(),
                Reviews = course.Reviews.Select(MapToReviewDto).ToList(),
                AverageRating = course.Reviews.Any() ? course.Reviews.Average(r => r.Rating) : 0,
                TotalReviews = course.Reviews.Count,
                TotalStudents = course.Enrollments.Count
            };
        }

        private CourseSectionDto MapToCourseSectionDto(CourseSection section)
        {
            return new CourseSectionDto
            {
                Id = section.Id,
                CourseId = section.CourseId,
                Title = section.Title,
                Description = section.Description,
                OrderIndex = section.OrderIndex,
                IsPublished = section.IsPublished,
                CreatedAt = section.CreatedAt,
                Lessons = section.Lessons.Select(MapToLessonDto).ToList()
            };
        }

        private LessonDto MapToLessonDto(Lesson lesson)
        {
            return new LessonDto
            {
                Id = lesson.Id,
                SectionId = lesson.SectionId,
                Title = lesson.Title,
                Description = lesson.Description,
                LessonType = lesson.LessonType,
                Content = lesson.Content,
                VideoUrl = lesson.VideoUrl,
                FileUrl = lesson.FileUrl,
                DurationMinutes = lesson.DurationMinutes,
                OrderIndex = lesson.OrderIndex,
                IsPublished = lesson.IsPublished,
                IsPreview = lesson.IsPreview,
                CreatedAt = lesson.CreatedAt,
                Resources = lesson.Resources.Select(MapToResourceDto).ToList()
            };
        }

        private ResourceDto MapToResourceDto(Resource resource)
        {
            return new ResourceDto
            {
                Id = resource.Id,
                LessonId = resource.LessonId,
                FileName = resource.FileName,
                FileUrl = resource.FileUrl,
                FileType = resource.FileType,
                FileSize = resource.FileSize,
                DownloadCount = resource.DownloadCount,
                CreatedAt = resource.CreatedAt
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
                Course = new CourseDto(), // Simplified for now
                Rating = review.Rating,
                Comment = review.Comment,
                IsApproved = review.IsApproved,
                CreatedAt = review.CreatedAt,
                UpdatedAt = review.UpdatedAt
            };
        }
    }
}
