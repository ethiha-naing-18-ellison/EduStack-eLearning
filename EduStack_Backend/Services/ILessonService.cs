using EduStack.API.DTOs;

namespace EduStack.API.Services
{
    public interface ILessonService
    {
        Task<LessonDto> GetLessonByIdAsync(int id);
        Task<List<LessonDto>> GetLessonsBySectionAsync(int sectionId);
        Task<List<LessonDto>> GetLessonsByCourseAsync(int courseId);
        Task<LessonDto> CreateLessonAsync(int sectionId, int instructorId, CreateLessonRequest request);
        Task<LessonDto> UpdateLessonAsync(int lessonId, int instructorId, UpdateLessonRequest request);
        Task<bool> DeleteLessonAsync(int lessonId, int instructorId);
        Task<bool> PublishLessonAsync(int lessonId, int instructorId);
        Task<bool> UnpublishLessonAsync(int lessonId, int instructorId);
        Task<bool> SetLessonPreviewAsync(int lessonId, int instructorId, bool isPreview);
        Task<List<LessonDto>> GetStudentLessonsAsync(int studentId, int courseId);
        Task<bool> CanStudentAccessLessonAsync(int studentId, int lessonId);
        Task<int> GetTotalCourseDurationAsync(int courseId);
    }
}
