using EduStack.API.Data;
using EduStack.API.DTOs;
using EduStack.API.Models;
using Microsoft.EntityFrameworkCore;

namespace EduStack.API.Services
{
    public class LessonService : ILessonService
    {
        private readonly EduStackDbContext _context;
        private readonly ILogger<LessonService> _logger;

        public LessonService(EduStackDbContext context, ILogger<LessonService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<LessonDto> GetLessonByIdAsync(int id)
        {
            var lesson = await _context.Lessons
                .Include(l => l.Section)
                .Include(l => l.Resources)
                .FirstOrDefaultAsync(l => l.Id == id);

            if (lesson == null)
                return null!;

            return MapToLessonDto(lesson);
        }

        public async Task<List<LessonDto>> GetLessonsBySectionAsync(int sectionId)
        {
            var lessons = await _context.Lessons
                .Include(l => l.Resources)
                .Where(l => l.SectionId == sectionId)
                .OrderBy(l => l.OrderIndex)
                .ToListAsync();

            return lessons.Select(MapToLessonDto).ToList();
        }

        public async Task<List<LessonDto>> GetLessonsByCourseAsync(int courseId)
        {
            var lessons = await _context.Lessons
                .Include(l => l.Section)
                .Include(l => l.Resources)
                .Where(l => l.Section.CourseId == courseId)
                .OrderBy(l => l.Section.OrderIndex)
                .ThenBy(l => l.OrderIndex)
                .ToListAsync();

            return lessons.Select(MapToLessonDto).ToList();
        }

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

        public async Task<bool> PublishLessonAsync(int lessonId, int instructorId)
        {
            var lesson = await _context.Lessons
                .Include(l => l.Section)
                    .ThenInclude(s => s.Course)
                .FirstOrDefaultAsync(l => l.Id == lessonId);

            if (lesson == null)
                return false;

            if (lesson.Section.Course.InstructorId != instructorId)
                throw new UnauthorizedAccessException("You can only publish lessons of your own courses");

            lesson.IsPublished = true;
            lesson.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UnpublishLessonAsync(int lessonId, int instructorId)
        {
            var lesson = await _context.Lessons
                .Include(l => l.Section)
                    .ThenInclude(s => s.Course)
                .FirstOrDefaultAsync(l => l.Id == lessonId);

            if (lesson == null)
                return false;

            if (lesson.Section.Course.InstructorId != instructorId)
                throw new UnauthorizedAccessException("You can only unpublish lessons of your own courses");

            lesson.IsPublished = false;
            lesson.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> SetLessonPreviewAsync(int lessonId, int instructorId, bool isPreview)
        {
            var lesson = await _context.Lessons
                .Include(l => l.Section)
                    .ThenInclude(s => s.Course)
                .FirstOrDefaultAsync(l => l.Id == lessonId);

            if (lesson == null)
                return false;

            if (lesson.Section.Course.InstructorId != instructorId)
                throw new UnauthorizedAccessException("You can only modify lessons of your own courses");

            lesson.IsPreview = isPreview;
            lesson.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<LessonDto>> GetStudentLessonsAsync(int studentId, int courseId)
        {
            // Check if student is enrolled
            var enrollment = await _context.Enrollments
                .FirstOrDefaultAsync(e => e.StudentId == studentId && e.CourseId == courseId);

            if (enrollment == null)
                throw new UnauthorizedAccessException("You must be enrolled in the course to access lessons");

            var lessons = await _context.Lessons
                .Include(l => l.Section)
                .Include(l => l.Resources)
                .Where(l => l.Section.CourseId == courseId && l.IsPublished)
                .OrderBy(l => l.Section.OrderIndex)
                .ThenBy(l => l.OrderIndex)
                .ToListAsync();

            return lessons.Select(MapToLessonDto).ToList();
        }

        public async Task<bool> CanStudentAccessLessonAsync(int studentId, int lessonId)
        {
            var lesson = await _context.Lessons
                .Include(l => l.Section)
                    .ThenInclude(s => s.Course)
                .FirstOrDefaultAsync(l => l.Id == lessonId);

            if (lesson == null)
                return false;

            // Check if lesson is preview (accessible to all)
            if (lesson.IsPreview)
                return true;

            // Check if student is enrolled
            var enrollment = await _context.Enrollments
                .FirstOrDefaultAsync(e => e.StudentId == studentId && e.CourseId == lesson.Section.CourseId);

            return enrollment != null && enrollment.IsActive;
        }

        public async Task<int> GetTotalCourseDurationAsync(int courseId)
        {
            var totalMinutes = await _context.Lessons
                .Where(l => l.Section.CourseId == courseId && l.IsPublished)
                .SumAsync(l => l.DurationMinutes);

            return totalMinutes;
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
    }
}
