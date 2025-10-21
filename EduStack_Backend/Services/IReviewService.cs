using EduStack.API.DTOs;

namespace EduStack.API.Services
{
    public interface IReviewService
    {
        Task<ReviewDto> CreateReviewAsync(int studentId, CreateReviewRequest request);
        Task<ReviewDto> UpdateReviewAsync(int reviewId, int studentId, UpdateReviewRequest request);
        Task<bool> DeleteReviewAsync(int reviewId, int studentId);
        Task<ReviewDto> GetReviewByIdAsync(int reviewId);
        Task<List<ReviewDto>> GetCourseReviewsAsync(int courseId, int page = 1, int pageSize = 10);
        Task<List<ReviewDto>> GetUserReviewsAsync(int userId, int page = 1, int pageSize = 10);
        Task<ReviewStatsDto> GetCourseReviewStatsAsync(int courseId);
        Task<List<ReviewDto>> GetPendingReviewsAsync(int page = 1, int pageSize = 10);
        Task<bool> ApproveReviewAsync(int reviewId);
        Task<bool> RejectReviewAsync(int reviewId);
        Task<bool> HasUserReviewedCourseAsync(int userId, int courseId);
    }
}
