using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EduStack.API.Models
{
    [Table("resources")]
    public class Resource
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("lesson_id")]
        public int LessonId { get; set; }

        [Required]
        [MaxLength(255)]
        [Column("file_name")]
        public string FileName { get; set; } = string.Empty;

        [Required]
        [MaxLength(500)]
        [Column("file_url")]
        public string FileUrl { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        [Column("file_type")]
        public string FileType { get; set; } = string.Empty;

        [Column("file_size")]
        public long? FileSize { get; set; }

        [Column("download_count")]
        public int DownloadCount { get; set; } = 0;

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        [ForeignKey("LessonId")]
        public virtual Lesson Lesson { get; set; } = null!;
    }
}
