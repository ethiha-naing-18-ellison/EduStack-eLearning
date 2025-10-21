using EduStack.API.DTOs;

namespace EduStack.API.Services
{
    public interface IResourceService
    {
        Task<ResourceDto> GetResourceByIdAsync(int id);
        Task<List<ResourceDto>> GetResourcesByLessonAsync(int lessonId);
        Task<ResourceDto> CreateResourceAsync(int lessonId, int instructorId, CreateResourceRequest request);
        Task<bool> DeleteResourceAsync(int resourceId, int instructorId);
        Task<bool> UpdateResourceAsync(int resourceId, int instructorId, CreateResourceRequest request);
        Task<bool> IncrementDownloadCountAsync(int resourceId);
        Task<List<ResourceDto>> GetResourcesByCourseAsync(int courseId);
        Task<bool> CanStudentAccessResourceAsync(int studentId, int resourceId);
        Task<List<ResourceDto>> GetStudentResourcesAsync(int studentId, int courseId);
    }
}
