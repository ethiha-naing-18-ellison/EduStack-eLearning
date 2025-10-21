using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EduStack.API.Models
{
    [Table("courses")]
    public class Course
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [MaxLength(255)]
        [Column("title")]
        public string Title { get; set; } = string.Empty;

        [Column("description")]
        public string? Description { get; set; }

        [Column("price", TypeName = "decimal(10,2)")]
        public decimal Price { get; set; } = 0.00m;

        [Column("instructor_id")]
        public int InstructorId { get; set; }

        [Column("category_id")]
        public int CategoryId { get; set; }

        [MaxLength(500)]
        [Column("thumbnail_url")]
        public string? ThumbnailUrl { get; set; }

        [Column("is_published")]
        public bool IsPublished { get; set; } = false;

        [MaxLength(20)]
        [Column("difficulty_level")]
        public string DifficultyLevel { get; set; } = "beginner";

        [Column("duration_hours")]
        public int DurationHours { get; set; } = 0;

        [MaxLength(10)]
        [Column("language")]
        public string Language { get; set; } = "en";

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        [ForeignKey("InstructorId")]
        public virtual User Instructor { get; set; } = null!;

        [ForeignKey("CategoryId")]
        public virtual Category Category { get; set; } = null!;

        public virtual ICollection<CourseSection> CourseSections { get; set; } = new List<CourseSection>();
        public virtual ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
        public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
        public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
    }
}
