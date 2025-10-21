using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EduStack.API.Models
{
    [Table("categories")]
    public class Category
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("name")]
        public string Name { get; set; } = string.Empty;

        [Column("description")]
        public string? Description { get; set; }

        [Column("parent_id")]
        public int? ParentId { get; set; }

        [MaxLength(500)]
        [Column("icon_url")]
        public string? IconUrl { get; set; }

        [Column("is_active")]
        public bool IsActive { get; set; } = true;

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        [ForeignKey("ParentId")]
        public virtual Category? Parent { get; set; }

        public virtual ICollection<Category> Children { get; set; } = new List<Category>();
        public virtual ICollection<Course> Courses { get; set; } = new List<Course>();
    }
}
