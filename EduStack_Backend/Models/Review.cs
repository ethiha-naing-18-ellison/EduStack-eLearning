using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EduStack.API.Models
{
    [Table("reviews")]
    public class Review
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("student_id")]
        public int StudentId { get; set; }

        [Column("course_id")]
        public int CourseId { get; set; }

        [Column("rating")]
        public int Rating { get; set; } // 1-5

        [Column("comment")]
        public string? Comment { get; set; }

        [Column("is_approved")]
        public bool IsApproved { get; set; } = false;

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        [ForeignKey("StudentId")]
        public virtual User Student { get; set; } = null!;

        [ForeignKey("CourseId")]
        public virtual Course Course { get; set; } = null!;
    }
}
