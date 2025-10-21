using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EduStack.API.Models
{
    [Table("instructor_applications")]
    public class InstructorApplication
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("user_id")]
        public int UserId { get; set; }

        [MaxLength(20)]
        [Column("application_status")]
        public string ApplicationStatus { get; set; } = "pending"; // 'pending', 'approved', 'rejected'

        [Column("qualifications")]
        public string? Qualifications { get; set; }

        [Column("experience_years")]
        public int? ExperienceYears { get; set; }

        [MaxLength(500)]
        [Column("portfolio_url")]
        public string? PortfolioUrl { get; set; }

        [Column("motivation")]
        public string? Motivation { get; set; }

        [Column("admin_remarks")]
        public string? AdminRemarks { get; set; }

        [Column("reviewed_by")]
        public int? ReviewedBy { get; set; }

        [Column("reviewed_at")]
        public DateTime? ReviewedAt { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        [ForeignKey("UserId")]
        public virtual User User { get; set; } = null!;

        [ForeignKey("ReviewedBy")]
        public virtual User? Reviewer { get; set; }
    }
}
