using EduStack.API.Models;
using Microsoft.EntityFrameworkCore;

namespace EduStack.API.Data
{
    public class EduStackDbContext : DbContext
    {
        public EduStackDbContext(DbContextOptions<EduStackDbContext> options) : base(options)
        {
        }

        public DbSet<Role> Roles { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<CourseSection> CourseSections { get; set; }
        public DbSet<Lesson> Lessons { get; set; }
        public DbSet<Resource> Resources { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }
        public DbSet<LessonProgress> LessonProgresses { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<InstructorApplication> InstructorApplications { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure table names to match PostgreSQL naming convention
            modelBuilder.Entity<Role>().ToTable("roles");
            modelBuilder.Entity<User>().ToTable("users");
            modelBuilder.Entity<Category>().ToTable("categories");
            modelBuilder.Entity<Course>().ToTable("courses");
            modelBuilder.Entity<CourseSection>().ToTable("course_sections");
            modelBuilder.Entity<Lesson>().ToTable("lessons");
            modelBuilder.Entity<Resource>().ToTable("resources");
            modelBuilder.Entity<Enrollment>().ToTable("enrollments");
            modelBuilder.Entity<LessonProgress>().ToTable("lesson_progress");
            modelBuilder.Entity<Payment>().ToTable("payments");
            modelBuilder.Entity<Review>().ToTable("reviews");
            modelBuilder.Entity<InstructorApplication>().ToTable("instructor_applications");

            // Configure relationships
            ConfigureUserRelationships(modelBuilder);
            ConfigureCourseRelationships(modelBuilder);
            ConfigureEnrollmentRelationships(modelBuilder);
            ConfigureProgressRelationships(modelBuilder);
            ConfigureReviewRelationships(modelBuilder);
            ConfigureApplicationRelationships(modelBuilder);

            // Configure unique constraints
            modelBuilder.Entity<Enrollment>()
                .HasIndex(e => new { e.StudentId, e.CourseId })
                .IsUnique();

            modelBuilder.Entity<LessonProgress>()
                .HasIndex(lp => new { lp.StudentId, lp.LessonId })
                .IsUnique();

            modelBuilder.Entity<Review>()
                .HasIndex(r => new { r.StudentId, r.CourseId })
                .IsUnique();

            // Configure check constraints
            modelBuilder.Entity<Review>()
                .HasCheckConstraint("CK_Review_Rating", "rating >= 1 AND rating <= 5");

            // Seed data
            SeedData(modelBuilder);
        }

        private void ConfigureUserRelationships(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasOne(u => u.Role)
                .WithMany(r => r.Users)
                .HasForeignKey(u => u.RoleId)
                .OnDelete(DeleteBehavior.Restrict);
        }

        private void ConfigureCourseRelationships(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Course>()
                .HasOne(c => c.Instructor)
                .WithMany(u => u.Courses)
                .HasForeignKey(c => c.InstructorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Course>()
                .HasOne(c => c.Category)
                .WithMany(cat => cat.Courses)
                .HasForeignKey(c => c.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CourseSection>()
                .HasOne(cs => cs.Course)
                .WithMany(c => c.CourseSections)
                .HasForeignKey(cs => cs.CourseId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Lesson>()
                .HasOne(l => l.Section)
                .WithMany(cs => cs.Lessons)
                .HasForeignKey(l => l.SectionId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Resource>()
                .HasOne(r => r.Lesson)
                .WithMany(l => l.Resources)
                .HasForeignKey(r => r.LessonId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Category>()
                .HasOne(c => c.Parent)
                .WithMany(c => c.Children)
                .HasForeignKey(c => c.ParentId)
                .OnDelete(DeleteBehavior.Restrict);
        }

        private void ConfigureEnrollmentRelationships(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Enrollment>()
                .HasOne(e => e.Student)
                .WithMany(u => u.Enrollments)
                .HasForeignKey(e => e.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Enrollment>()
                .HasOne(e => e.Course)
                .WithMany(c => c.Enrollments)
                .HasForeignKey(e => e.CourseId)
                .OnDelete(DeleteBehavior.Restrict);
        }

        private void ConfigureProgressRelationships(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<LessonProgress>()
                .HasOne(lp => lp.Student)
                .WithMany(u => u.LessonProgresses)
                .HasForeignKey(lp => lp.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<LessonProgress>()
                .HasOne(lp => lp.Lesson)
                .WithMany(l => l.LessonProgresses)
                .HasForeignKey(lp => lp.LessonId)
                .OnDelete(DeleteBehavior.Restrict);
        }

        private void ConfigureReviewRelationships(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Review>()
                .HasOne(r => r.Student)
                .WithMany(u => u.Reviews)
                .HasForeignKey(r => r.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Review>()
                .HasOne(r => r.Course)
                .WithMany(c => c.Reviews)
                .HasForeignKey(r => r.CourseId)
                .OnDelete(DeleteBehavior.Restrict);
        }

        private void ConfigureApplicationRelationships(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<InstructorApplication>()
                .HasOne(ia => ia.User)
                .WithMany(u => u.InstructorApplications)
                .HasForeignKey(ia => ia.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<InstructorApplication>()
                .HasOne(ia => ia.Reviewer)
                .WithMany()
                .HasForeignKey(ia => ia.ReviewedBy)
                .OnDelete(DeleteBehavior.Restrict);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            // Seed roles
            modelBuilder.Entity<Role>().HasData(
                new Role { Id = 1, Name = "Admin", Description = "System Administrator", CreatedAt = DateTime.UtcNow },
                new Role { Id = 2, Name = "Instructor", Description = "Course Instructor", CreatedAt = DateTime.UtcNow },
                new Role { Id = 3, Name = "Student", Description = "Course Student", CreatedAt = DateTime.UtcNow }
            );

            // Seed categories
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Programming", Description = "Programming and Software Development", IsActive = true, CreatedAt = DateTime.UtcNow },
                new Category { Id = 2, Name = "Web Development", Description = "Frontend and Backend Web Development", ParentId = 1, IsActive = true, CreatedAt = DateTime.UtcNow },
                new Category { Id = 3, Name = "Data Science", Description = "Data Analysis and Machine Learning", IsActive = true, CreatedAt = DateTime.UtcNow },
                new Category { Id = 4, Name = "Design", Description = "UI/UX and Graphic Design", IsActive = true, CreatedAt = DateTime.UtcNow },
                new Category { Id = 5, Name = "Business", Description = "Business and Entrepreneurship", IsActive = true, CreatedAt = DateTime.UtcNow }
            );
        }
    }
}
