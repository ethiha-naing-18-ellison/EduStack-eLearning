using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EduStack.API.Models
{
    [Table("lessons")]
    public class Lesson
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("section_id")]
        public int SectionId { get; set; }

        [Required]
        [MaxLength(255)]
        [Column("title")]
        public string Title { get; set; } = string.Empty;

        [Column("description")]
        public string? Description { get; set; }

        [Required]
        [MaxLength(50)]
        [Column("lesson_type")]
        public string LessonType { get; set; } = string.Empty; // 'video', 'text', 'quiz', 'assignment'

        [Column("content")]
        public string? Content { get; set; }

        [MaxLength(500)]
        [Column("video_url")]
        public string? VideoUrl { get; set; }

        [MaxLength(500)]
        [Column("file_url")]
        public string? FileUrl { get; set; }

        [Column("duration_minutes")]
        public int DurationMinutes { get; set; } = 0;

        [Column("order_index")]
        public int OrderIndex { get; set; }

        [Column("is_published")]
        public bool IsPublished { get; set; } = false;

        [Column("is_preview")]
        public bool IsPreview { get; set; } = false;

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        [ForeignKey("SectionId")]
        public virtual CourseSection Section { get; set; } = null!;

        public virtual ICollection<Resource> Resources { get; set; } = new List<Resource>();
        public virtual ICollection<LessonProgress> LessonProgresses { get; set; } = new List<LessonProgress>();
    }
}
