using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EduStack.API.Models
{
    [Table("lesson_progress")]
    public class LessonProgress
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("student_id")]
        public int StudentId { get; set; }

        [Column("lesson_id")]
        public int LessonId { get; set; }

        [Column("is_completed")]
        public bool IsCompleted { get; set; } = false;

        [Column("completion_date")]
        public DateTime? CompletionDate { get; set; }

        [Column("time_spent_minutes")]
        public int TimeSpentMinutes { get; set; } = 0;

        [Column("last_position_seconds")]
        public int LastPositionSeconds { get; set; } = 0;

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        [ForeignKey("StudentId")]
        public virtual User Student { get; set; } = null!;

        [ForeignKey("LessonId")]
        public virtual Lesson Lesson { get; set; } = null!;
    }
}
