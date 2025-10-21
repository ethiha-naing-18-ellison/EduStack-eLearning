using EduStack.API.DTOs;

namespace EduStack.API.Services
{
    public interface IUserService
    {
        Task<UserDto> GetUserByIdAsync(int id);
        Task<UserDto> GetUserByEmailAsync(string email);
        Task<UserDto> UpdateProfileAsync(int userId, UpdateProfileRequest request);
        Task<bool> DeactivateUserAsync(int userId);
        Task<bool> ActivateUserAsync(int userId);
        Task<List<UserDto>> GetAllUsersAsync(int page = 1, int pageSize = 10);
        Task<List<UserDto>> GetUsersByRoleAsync(string role, int page = 1, int pageSize = 10);
        Task<InstructorApplicationDto> ApplyForInstructorAsync(int userId, InstructorApplicationRequest request);
        Task<List<InstructorApplicationDto>> GetPendingInstructorApplicationsAsync();
        Task<InstructorApplicationDto> ReviewInstructorApplicationAsync(int applicationId, int adminId, ReviewInstructorApplicationRequest request);
        Task<List<InstructorApplicationDto>> GetInstructorApplicationsByUserAsync(int userId);
    }
}
