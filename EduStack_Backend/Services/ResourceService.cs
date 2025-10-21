using EduStack.API.Data;
using EduStack.API.DTOs;
using EduStack.API.Models;
using Microsoft.EntityFrameworkCore;

namespace EduStack.API.Services
{
    public class ResourceService : IResourceService
    {
        private readonly EduStackDbContext _context;
        private readonly ILogger<ResourceService> _logger;

        public ResourceService(EduStackDbContext context, ILogger<ResourceService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<ResourceDto> GetResourceByIdAsync(int id)
        {
            var resource = await _context.Resources
                .Include(r => r.Lesson)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (resource == null)
                return null!;

            return MapToResourceDto(resource);
        }

        public async Task<List<ResourceDto>> GetResourcesByLessonAsync(int lessonId)
        {
            var resources = await _context.Resources
                .Where(r => r.LessonId == lessonId)
                .OrderBy(r => r.CreatedAt)
                .ToListAsync();

            return resources.Select(MapToResourceDto).ToList();
        }

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

        public async Task<bool> UpdateResourceAsync(int resourceId, int instructorId, CreateResourceRequest request)
        {
            var resource = await _context.Resources
                .Include(r => r.Lesson)
                    .ThenInclude(l => l.Section)
                        .ThenInclude(s => s.Course)
                .FirstOrDefaultAsync(r => r.Id == resourceId);

            if (resource == null)
                return false;

            if (resource.Lesson.Section.Course.InstructorId != instructorId)
                throw new UnauthorizedAccessException("You can only update resources of your own courses");

            resource.FileName = request.FileName;
            resource.FileUrl = request.FileUrl;
            resource.FileType = request.FileType;
            resource.FileSize = request.FileSize;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> IncrementDownloadCountAsync(int resourceId)
        {
            var resource = await _context.Resources.FindAsync(resourceId);
            if (resource == null)
                return false;

            resource.DownloadCount++;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<ResourceDto>> GetResourcesByCourseAsync(int courseId)
        {
            var resources = await _context.Resources
                .Include(r => r.Lesson)
                    .ThenInclude(l => l.Section)
                .Where(r => r.Lesson.Section.CourseId == courseId)
                .OrderBy(r => r.Lesson.Section.OrderIndex)
                .ThenBy(r => r.Lesson.OrderIndex)
                .ThenBy(r => r.CreatedAt)
                .ToListAsync();

            return resources.Select(MapToResourceDto).ToList();
        }

        public async Task<bool> CanStudentAccessResourceAsync(int studentId, int resourceId)
        {
            var resource = await _context.Resources
                .Include(r => r.Lesson)
                    .ThenInclude(l => l.Section)
                        .ThenInclude(s => s.Course)
                .FirstOrDefaultAsync(r => r.Id == resourceId);

            if (resource == null)
                return false;

            // Check if lesson is preview (accessible to all)
            if (resource.Lesson.IsPreview)
                return true;

            // Check if student is enrolled
            var enrollment = await _context.Enrollments
                .FirstOrDefaultAsync(e => e.StudentId == studentId && e.CourseId == resource.Lesson.Section.CourseId);

            return enrollment != null && enrollment.IsActive;
        }

        public async Task<List<ResourceDto>> GetStudentResourcesAsync(int studentId, int courseId)
        {
            // Check if student is enrolled
            var enrollment = await _context.Enrollments
                .FirstOrDefaultAsync(e => e.StudentId == studentId && e.CourseId == courseId);

            if (enrollment == null)
                throw new UnauthorizedAccessException("You must be enrolled in the course to access resources");

            var resources = await _context.Resources
                .Include(r => r.Lesson)
                    .ThenInclude(l => l.Section)
                .Where(r => r.Lesson.Section.CourseId == courseId)
                .OrderBy(r => r.Lesson.Section.OrderIndex)
                .ThenBy(r => r.Lesson.OrderIndex)
                .ThenBy(r => r.CreatedAt)
                .ToListAsync();

            return resources.Select(MapToResourceDto).ToList();
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
