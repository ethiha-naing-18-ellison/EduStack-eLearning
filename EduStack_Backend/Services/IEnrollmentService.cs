using EduStack.API.DTOs;

namespace EduStack.API.Services
{
    public interface IEnrollmentService
    {
        Task<EnrollmentDto> EnrollInCourseAsync(int studentId, EnrollRequest request);
        Task<bool> UnenrollFromCourseAsync(int studentId, int courseId);
        Task<List<EnrollmentDto>> GetStudentEnrollmentsAsync(int studentId, int page = 1, int pageSize = 10);
        Task<List<EnrollmentDto>> GetCourseEnrollmentsAsync(int courseId, int page = 1, int pageSize = 10);
        Task<EnrollmentProgressDto> GetEnrollmentProgressAsync(int studentId, int courseId);
        Task<bool> UpdateLessonProgressAsync(int studentId, UpdateLessonProgressRequest request);
        Task<List<LessonProgressDto>> GetStudentLessonProgressAsync(int studentId, int courseId);
        Task<bool> CompleteCourseAsync(int studentId, int courseId);
        Task<bool> IsEnrolledAsync(int studentId, int courseId);
        Task<decimal> CalculateCourseProgressAsync(int studentId, int courseId);
    }
}
