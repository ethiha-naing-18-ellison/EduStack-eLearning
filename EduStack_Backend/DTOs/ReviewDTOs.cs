using System.ComponentModel.DataAnnotations;

namespace EduStack.API.DTOs
{
    public class ReviewDto
    {
        public int Id { get; set; }
        public UserDto Student { get; set; } = null!;
        public CourseDto Course { get; set; } = null!;
        public int Rating { get; set; }
        public string? Comment { get; set; }
        public bool IsApproved { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class CreateReviewRequest
    {
        [Required]
        public int CourseId { get; set; }

        [Required]
        [Range(1, 5)]
        public int Rating { get; set; }

        public string? Comment { get; set; }
    }

    public class UpdateReviewRequest
    {
        [Range(1, 5)]
        public int? Rating { get; set; }

        public string? Comment { get; set; }
    }

    public class ReviewStatsDto
    {
        public double AverageRating { get; set; }
        public int TotalReviews { get; set; }
        public int Rating5 { get; set; }
        public int Rating4 { get; set; }
        public int Rating3 { get; set; }
        public int Rating2 { get; set; }
        public int Rating1 { get; set; }
    }
}
