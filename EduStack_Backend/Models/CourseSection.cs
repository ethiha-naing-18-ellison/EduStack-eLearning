using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EduStack.API.Models
{
    [Table("course_sections")]
    public class CourseSection
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("course_id")]
        public int CourseId { get; set; }

        [Required]
        [MaxLength(255)]
        [Column("title")]
        public string Title { get; set; } = string.Empty;

        [Column("description")]
        public string? Description { get; set; }

        [Column("order_index")]
        public int OrderIndex { get; set; }

        [Column("is_published")]
        public bool IsPublished { get; set; } = false;

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        [ForeignKey("CourseId")]
        public virtual Course Course { get; set; } = null!;

        public virtual ICollection<Lesson> Lessons { get; set; } = new List<Lesson>();
    }
}
