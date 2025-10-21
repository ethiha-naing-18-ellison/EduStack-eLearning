using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EduStack.API.Models
{
    [Table("enrollments")]
    public class Enrollment
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("student_id")]
        public int StudentId { get; set; }

        [Column("course_id")]
        public int CourseId { get; set; }

        [Column("enrollment_date")]
        public DateTime EnrollmentDate { get; set; } = DateTime.UtcNow;

        [Column("progress_percentage", TypeName = "decimal(5,2)")]
        public decimal ProgressPercentage { get; set; } = 0.00m;

        [Column("completion_date")]
        public DateTime? CompletionDate { get; set; }

        [Column("is_active")]
        public bool IsActive { get; set; } = true;

        [MaxLength(20)]
        [Column("payment_status")]
        public string PaymentStatus { get; set; } = "pending";

        // Navigation properties
        [ForeignKey("StudentId")]
        public virtual User Student { get; set; } = null!;

        [ForeignKey("CourseId")]
        public virtual Course Course { get; set; } = null!;
    }
}
