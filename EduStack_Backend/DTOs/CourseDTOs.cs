using System.ComponentModel.DataAnnotations;

namespace EduStack.API.DTOs
{
    public class CourseDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public UserDto Instructor { get; set; } = null!;
        public CategoryDto Category { get; set; } = null!;
        public string? ThumbnailUrl { get; set; }
        public bool IsPublished { get; set; }
        public string DifficultyLevel { get; set; } = string.Empty;
        public int DurationHours { get; set; }
        public string Language { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public List<CourseSectionDto> Sections { get; set; } = new List<CourseSectionDto>();
        public List<ReviewDto> Reviews { get; set; } = new List<ReviewDto>();
        public double AverageRating { get; set; }
        public int TotalReviews { get; set; }
        public int TotalStudents { get; set; }
    }

    public class CreateCourseRequest
    {
        [Required]
        [MaxLength(255)]
        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        [Range(0, double.MaxValue)]
        public decimal Price { get; set; } = 0;

        [Required]
        public int CategoryId { get; set; }

        [MaxLength(500)]
        public string? ThumbnailUrl { get; set; }

        [MaxLength(20)]
        public string DifficultyLevel { get; set; } = "beginner";

        [Range(0, int.MaxValue)]
        public int DurationHours { get; set; } = 0;

        [MaxLength(10)]
        public string Language { get; set; } = "en";
    }

    public class UpdateCourseRequest
    {
        [MaxLength(255)]
        public string? Title { get; set; }

        public string? Description { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? Price { get; set; }

        public int? CategoryId { get; set; }

        [MaxLength(500)]
        public string? ThumbnailUrl { get; set; }

        [MaxLength(20)]
        public string? DifficultyLevel { get; set; }

        [Range(0, int.MaxValue)]
        public int? DurationHours { get; set; }

        [MaxLength(10)]
        public string? Language { get; set; }

        public bool? IsPublished { get; set; }
    }

    public class CourseSectionDto
    {
        public int Id { get; set; }
        public int CourseId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int OrderIndex { get; set; }
        public bool IsPublished { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<LessonDto> Lessons { get; set; } = new List<LessonDto>();
    }

    public class CreateCourseSectionRequest
    {
        [Required]
        [MaxLength(255)]
        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        [Required]
        public int OrderIndex { get; set; }
    }

    public class UpdateCourseSectionRequest
    {
        [MaxLength(255)]
        public string? Title { get; set; }

        public string? Description { get; set; }

        public int? OrderIndex { get; set; }

        public bool? IsPublished { get; set; }
    }

    public class LessonDto
    {
        public int Id { get; set; }
        public int SectionId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string LessonType { get; set; } = string.Empty;
        public string? Content { get; set; }
        public string? VideoUrl { get; set; }
        public string? FileUrl { get; set; }
        public int DurationMinutes { get; set; }
        public int OrderIndex { get; set; }
        public bool IsPublished { get; set; }
        public bool IsPreview { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<ResourceDto> Resources { get; set; } = new List<ResourceDto>();
    }

    public class CreateLessonRequest
    {
        [Required]
        [MaxLength(255)]
        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        [Required]
        [MaxLength(50)]
        public string LessonType { get; set; } = string.Empty;

        public string? Content { get; set; }

        [MaxLength(500)]
        public string? VideoUrl { get; set; }

        [MaxLength(500)]
        public string? FileUrl { get; set; }

        [Range(0, int.MaxValue)]
        public int DurationMinutes { get; set; } = 0;

        [Required]
        public int OrderIndex { get; set; }

        public bool IsPreview { get; set; } = false;
    }

    public class UpdateLessonRequest
    {
        [MaxLength(255)]
        public string? Title { get; set; }

        public string? Description { get; set; }

        [MaxLength(50)]
        public string? LessonType { get; set; }

        public string? Content { get; set; }

        [MaxLength(500)]
        public string? VideoUrl { get; set; }

        [MaxLength(500)]
        public string? FileUrl { get; set; }

        [Range(0, int.MaxValue)]
        public int? DurationMinutes { get; set; }

        public int? OrderIndex { get; set; }

        public bool? IsPublished { get; set; }

        public bool? IsPreview { get; set; }
    }

    public class ResourceDto
    {
        public int Id { get; set; }
        public int LessonId { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string FileUrl { get; set; } = string.Empty;
        public string FileType { get; set; } = string.Empty;
        public long? FileSize { get; set; }
        public int DownloadCount { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CreateResourceRequest
    {
        [Required]
        [MaxLength(255)]
        public string FileName { get; set; } = string.Empty;

        [Required]
        [MaxLength(500)]
        public string FileUrl { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string FileType { get; set; } = string.Empty;

        public long? FileSize { get; set; }
    }

    public class CategoryDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int? ParentId { get; set; }
        public string? IconUrl { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<CategoryDto> Children { get; set; } = new List<CategoryDto>();
    }

    public class CreateCategoryRequest
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        public int? ParentId { get; set; }

        [MaxLength(500)]
        public string? IconUrl { get; set; }
    }

    public class UpdateCategoryRequest
    {
        [MaxLength(100)]
        public string? Name { get; set; }

        public string? Description { get; set; }

        public int? ParentId { get; set; }

        [MaxLength(500)]
        public string? IconUrl { get; set; }

        public bool? IsActive { get; set; }
    }
}
