using System.ComponentModel.DataAnnotations;

namespace EduStack.API.DTOs
{
    public class UserDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string? ProfileImage { get; set; }
        public string? Phone { get; set; }
        public string? Bio { get; set; }
        public bool IsActive { get; set; }
        public bool EmailVerified { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class UpdateProfileRequest
    {
        [MaxLength(255)]
        public string? Name { get; set; }

        [MaxLength(20)]
        public string? Phone { get; set; }

        public string? Bio { get; set; }

        [MaxLength(500)]
        public string? ProfileImage { get; set; }
    }

    public class InstructorApplicationRequest
    {
        [Required]
        public string Qualifications { get; set; } = string.Empty;

        public int? ExperienceYears { get; set; }

        [MaxLength(500)]
        public string? PortfolioUrl { get; set; }

        [Required]
        public string Motivation { get; set; } = string.Empty;
    }

    public class InstructorApplicationDto
    {
        public int Id { get; set; }
        public UserDto User { get; set; } = null!;
        public string ApplicationStatus { get; set; } = string.Empty;
        public string Qualifications { get; set; } = string.Empty;
        public int? ExperienceYears { get; set; }
        public string? PortfolioUrl { get; set; }
        public string Motivation { get; set; } = string.Empty;
        public string? AdminRemarks { get; set; }
        public UserDto? Reviewer { get; set; }
        public DateTime? ReviewedAt { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class ReviewInstructorApplicationRequest
    {
        [Required]
        public string Status { get; set; } = string.Empty; // 'approved' or 'rejected'

        public string? Remarks { get; set; }
    }
}
