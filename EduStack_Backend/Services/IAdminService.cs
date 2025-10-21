using EduStack.API.DTOs;

namespace EduStack.API.Services
{
    public interface IAdminService
    {
        // User Management
        Task<List<UserDto>> GetAllUsersAsync(int page = 1, int pageSize = 10, string? search = null, string? role = null);
        Task<UserDto> GetUserByIdAsync(int userId);
        Task<bool> ActivateUserAsync(int userId);
        Task<bool> DeactivateUserAsync(int userId);
        Task<bool> DeleteUserAsync(int userId);
        
        // Course Management
        Task<List<CourseDto>> GetAllCoursesAsync(int page = 1, int pageSize = 10, string? search = null, int? categoryId = null);
        Task<bool> PublishCourseAsync(int courseId);
        Task<bool> UnpublishCourseAsync(int courseId);
        Task<bool> DeleteCourseAsync(int courseId);
        
        // Instructor Applications
        Task<List<InstructorApplicationDto>> GetPendingInstructorApplicationsAsync();
        Task<List<InstructorApplicationDto>> GetAllInstructorApplicationsAsync(int page = 1, int pageSize = 10);
        Task<InstructorApplicationDto> ReviewInstructorApplicationAsync(int applicationId, int adminId, ReviewInstructorApplicationRequest request);
        
        // Reviews Management
        Task<List<ReviewDto>> GetPendingReviewsAsync(int page = 1, int pageSize = 10);
        Task<bool> ApproveReviewAsync(int reviewId);
        Task<bool> RejectReviewAsync(int reviewId);
        
        // Categories Management
        Task<List<CategoryDto>> GetAllCategoriesAsync();
        Task<CategoryDto> CreateCategoryAsync(CreateCategoryRequest request);
        Task<CategoryDto> UpdateCategoryAsync(int categoryId, UpdateCategoryRequest request);
        Task<bool> DeleteCategoryAsync(int categoryId);
        
        // Analytics and Reports
        Task<object> GetDashboardStatsAsync();
        Task<List<PaymentDto>> GetRevenueReportAsync(DateTime? fromDate = null, DateTime? toDate = null);
        Task<List<CourseDto>> GetTopCoursesAsync(int count = 10);
        Task<List<UserDto>> GetTopInstructorsAsync(int count = 10);
    }
}
