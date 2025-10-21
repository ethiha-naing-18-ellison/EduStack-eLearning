using EduStack.API.DTOs;

namespace EduStack.API.Services
{
    public interface ICourseService
    {
        Task<CourseDto> GetCourseByIdAsync(int id);
        Task<List<CourseDto>> GetAllCoursesAsync(int page = 1, int pageSize = 10, string? search = null, int? categoryId = null);
        Task<List<CourseDto>> GetCoursesByInstructorAsync(int instructorId, int page = 1, int pageSize = 10);
        Task<List<CourseDto>> GetPublishedCoursesAsync(int page = 1, int pageSize = 10, string? search = null, int? categoryId = null);
        Task<CourseDto> CreateCourseAsync(int instructorId, CreateCourseRequest request);
        Task<CourseDto> UpdateCourseAsync(int courseId, int instructorId, UpdateCourseRequest request);
        Task<bool> DeleteCourseAsync(int courseId, int instructorId);
        Task<bool> PublishCourseAsync(int courseId, int instructorId);
        Task<bool> UnpublishCourseAsync(int courseId, int instructorId);
        
        // Course Sections
        Task<CourseSectionDto> CreateCourseSectionAsync(int courseId, int instructorId, CreateCourseSectionRequest request);
        Task<CourseSectionDto> UpdateCourseSectionAsync(int sectionId, int instructorId, UpdateCourseSectionRequest request);
        Task<bool> DeleteCourseSectionAsync(int sectionId, int instructorId);
        
        // Lessons
        Task<LessonDto> CreateLessonAsync(int sectionId, int instructorId, CreateLessonRequest request);
        Task<LessonDto> UpdateLessonAsync(int lessonId, int instructorId, UpdateLessonRequest request);
        Task<bool> DeleteLessonAsync(int lessonId, int instructorId);
        
        // Resources
        Task<ResourceDto> CreateResourceAsync(int lessonId, int instructorId, CreateResourceRequest request);
        Task<bool> DeleteResourceAsync(int resourceId, int instructorId);
        
        // Categories
        Task<List<CategoryDto>> GetAllCategoriesAsync();
        Task<CategoryDto> CreateCategoryAsync(CreateCategoryRequest request);
        Task<CategoryDto> UpdateCategoryAsync(int categoryId, UpdateCategoryRequest request);
        Task<bool> DeleteCategoryAsync(int categoryId);
    }
}
