using System.ComponentModel.DataAnnotations;

namespace EduStack.API.DTOs
{
    public class EnrollmentDto
    {
        public int Id { get; set; }
        public UserDto Student { get; set; } = null!;
        public CourseDto Course { get; set; } = null!;
        public DateTime EnrollmentDate { get; set; }
        public decimal ProgressPercentage { get; set; }
        public DateTime? CompletionDate { get; set; }
        public bool IsActive { get; set; }
        public string PaymentStatus { get; set; } = string.Empty;
    }

    public class EnrollRequest
    {
        [Required]
        public int CourseId { get; set; }
    }

    public class EnrollmentProgressDto
    {
        public int CourseId { get; set; }
        public string CourseTitle { get; set; } = string.Empty;
        public string? ThumbnailUrl { get; set; }
        public decimal ProgressPercentage { get; set; }
        public DateTime EnrollmentDate { get; set; }
        public DateTime? CompletionDate { get; set; }
        public bool IsActive { get; set; }
        public string PaymentStatus { get; set; } = string.Empty;
        public List<SectionProgressDto> Sections { get; set; } = new List<SectionProgressDto>();
    }

    public class SectionProgressDto
    {
        public int SectionId { get; set; }
        public string SectionTitle { get; set; } = string.Empty;
        public int OrderIndex { get; set; }
        public List<LessonProgressDto> Lessons { get; set; } = new List<LessonProgressDto>();
        public decimal SectionProgress { get; set; }
    }

    public class LessonProgressDto
    {
        public int LessonId { get; set; }
        public string LessonTitle { get; set; } = string.Empty;
        public string LessonType { get; set; } = string.Empty;
        public int DurationMinutes { get; set; }
        public int OrderIndex { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime? CompletionDate { get; set; }
        public int TimeSpentMinutes { get; set; }
        public int LastPositionSeconds { get; set; }
        public bool IsPreview { get; set; }
    }

    public class UpdateLessonProgressRequest
    {
        [Required]
        public int LessonId { get; set; }

        public bool? IsCompleted { get; set; }

        [Range(0, int.MaxValue)]
        public int? TimeSpentMinutes { get; set; }

        [Range(0, int.MaxValue)]
        public int? LastPositionSeconds { get; set; }
    }
}
