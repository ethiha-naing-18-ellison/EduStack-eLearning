-- =====================================================
-- EduStack E-Learning Platform - Complete Database Setup
-- =====================================================
-- This script creates the entire EduStack database structure
-- Run this script as a PostgreSQL superuser (postgres)
-- =====================================================

-- =====================================================
-- STEP 1: CREATE DATABASE AND USER
-- =====================================================

-- Create database (run this as postgres superuser)
CREATE DATABASE edustack;

-- Create application user
CREATE USER edustack_user WITH PASSWORD 'edustack_secure_password_2025';

-- Grant privileges to the user
GRANT ALL PRIVILEGES ON DATABASE edustack TO edustack_user;

-- Connect to the database
\c edustack;

-- Grant schema privileges
GRANT ALL ON SCHEMA public TO edustack_user;
GRANT ALL PRIVILEGES ON ALL TABLES IN SCHEMA public TO edustack_user;
GRANT ALL PRIVILEGES ON ALL SEQUENCES IN SCHEMA public TO edustack_user;

-- Set default privileges for future tables
ALTER DEFAULT PRIVILEGES IN SCHEMA public GRANT ALL ON TABLES TO edustack_user;
ALTER DEFAULT PRIVILEGES IN SCHEMA public GRANT ALL ON SEQUENCES TO edustack_user;

-- =====================================================
-- STEP 2: CREATE ALL TABLES
-- =====================================================

-- 1. Create Roles Table
CREATE TABLE roles (
    id SERIAL PRIMARY KEY,
    name VARCHAR(50) UNIQUE NOT NULL,
    description TEXT,
    permissions JSONB,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- 2. Create Users Table
CREATE TABLE users (
    id SERIAL PRIMARY KEY,
    name VARCHAR(255) NOT NULL,
    email VARCHAR(255) UNIQUE NOT NULL,
    password_hash VARCHAR(255) NOT NULL,
    role_id INTEGER REFERENCES roles(id),
    profile_image VARCHAR(500),
    phone VARCHAR(20),
    bio TEXT,
    is_active BOOLEAN DEFAULT true,
    email_verified BOOLEAN DEFAULT false,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- 3. Create Categories Table
CREATE TABLE categories (
    id SERIAL PRIMARY KEY,
    name VARCHAR(100) UNIQUE NOT NULL,
    description TEXT,
    parent_id INTEGER REFERENCES categories(id),
    icon_url VARCHAR(500),
    is_active BOOLEAN DEFAULT true,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- 4. Create Courses Table
CREATE TABLE courses (
    id SERIAL PRIMARY KEY,
    title VARCHAR(255) NOT NULL,
    description TEXT,
    price DECIMAL(10,2) DEFAULT 0.00,
    instructor_id INTEGER REFERENCES users(id),
    category_id INTEGER REFERENCES categories(id),
    thumbnail_url VARCHAR(500),
    is_published BOOLEAN DEFAULT false,
    difficulty_level VARCHAR(20) DEFAULT 'beginner',
    duration_hours INTEGER DEFAULT 0,
    language VARCHAR(10) DEFAULT 'en',
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- 5. Create Course Sections Table
CREATE TABLE course_sections (
    id SERIAL PRIMARY KEY,
    course_id INTEGER REFERENCES courses(id) ON DELETE CASCADE,
    title VARCHAR(255) NOT NULL,
    description TEXT,
    order_index INTEGER NOT NULL,
    is_published BOOLEAN DEFAULT false,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- 6. Create Lessons Table
CREATE TABLE lessons (
    id SERIAL PRIMARY KEY,
    section_id INTEGER REFERENCES course_sections(id) ON DELETE CASCADE,
    title VARCHAR(255) NOT NULL,
    description TEXT,
    lesson_type VARCHAR(50) NOT NULL, -- 'video', 'text', 'quiz', 'assignment'
    content TEXT,
    video_url VARCHAR(500),
    file_url VARCHAR(500),
    duration_minutes INTEGER DEFAULT 0,
    order_index INTEGER NOT NULL,
    is_published BOOLEAN DEFAULT false,
    is_preview BOOLEAN DEFAULT false,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- 7. Create Resources Table
CREATE TABLE resources (
    id SERIAL PRIMARY KEY,
    lesson_id INTEGER REFERENCES lessons(id) ON DELETE CASCADE,
    file_name VARCHAR(255) NOT NULL,
    file_url VARCHAR(500) NOT NULL,
    file_type VARCHAR(50) NOT NULL,
    file_size BIGINT,
    download_count INTEGER DEFAULT 0,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- 8. Create Enrollments Table
CREATE TABLE enrollments (
    id SERIAL PRIMARY KEY,
    student_id INTEGER REFERENCES users(id),
    course_id INTEGER REFERENCES courses(id),
    enrollment_date TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    progress_percentage DECIMAL(5,2) DEFAULT 0.00,
    completion_date TIMESTAMP,
    is_active BOOLEAN DEFAULT true,
    payment_status VARCHAR(20) DEFAULT 'pending',
    UNIQUE(student_id, course_id)
);

-- 9. Create Lesson Progress Table
CREATE TABLE lesson_progress (
    id SERIAL PRIMARY KEY,
    student_id INTEGER REFERENCES users(id),
    lesson_id INTEGER REFERENCES lessons(id),
    is_completed BOOLEAN DEFAULT false,
    completion_date TIMESTAMP,
    time_spent_minutes INTEGER DEFAULT 0,
    last_position_seconds INTEGER DEFAULT 0,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    UNIQUE(student_id, lesson_id)
);

-- 10. Create Payments Table
CREATE TABLE payments (
    id SERIAL PRIMARY KEY,
    user_id INTEGER REFERENCES users(id),
    course_id INTEGER REFERENCES courses(id),
    amount DECIMAL(10,2) NOT NULL,
    currency VARCHAR(3) DEFAULT 'USD',
    payment_method VARCHAR(50) NOT NULL,
    payment_status VARCHAR(20) DEFAULT 'pending',
    transaction_id VARCHAR(255),
    gateway_response JSONB,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- 11. Create Reviews Table
CREATE TABLE reviews (
    id SERIAL PRIMARY KEY,
    student_id INTEGER REFERENCES users(id),
    course_id INTEGER REFERENCES courses(id),
    rating INTEGER CHECK (rating >= 1 AND rating <= 5),
    comment TEXT,
    is_approved BOOLEAN DEFAULT false,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    UNIQUE(student_id, course_id)
);

-- 12. Create Instructor Applications Table
CREATE TABLE instructor_applications (
    id SERIAL PRIMARY KEY,
    user_id INTEGER REFERENCES users(id),
    application_status VARCHAR(20) DEFAULT 'pending', -- 'pending', 'approved', 'rejected'
    qualifications TEXT,
    experience_years INTEGER,
    portfolio_url VARCHAR(500),
    motivation TEXT,
    admin_remarks TEXT,
    reviewed_by INTEGER REFERENCES users(id),
    reviewed_at TIMESTAMP,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- =====================================================
-- STEP 3: CREATE ALL INDEXES FOR PERFORMANCE
-- =====================================================

-- User table indexes
CREATE INDEX idx_users_email ON users(email);
CREATE INDEX idx_users_role_id ON users(role_id);
CREATE INDEX idx_users_is_active ON users(is_active);

-- Course table indexes
CREATE INDEX idx_courses_instructor_id ON courses(instructor_id);
CREATE INDEX idx_courses_category_id ON courses(category_id);
CREATE INDEX idx_courses_is_published ON courses(is_published);
CREATE INDEX idx_courses_price ON courses(price);

-- Enrollment table indexes
CREATE INDEX idx_enrollments_student_id ON enrollments(student_id);
CREATE INDEX idx_enrollments_course_id ON enrollments(course_id);
CREATE INDEX idx_enrollments_payment_status ON enrollments(payment_status);

-- Lesson Progress indexes
CREATE INDEX idx_lesson_progress_student_id ON lesson_progress(student_id);
CREATE INDEX idx_lesson_progress_lesson_id ON lesson_progress(lesson_id);
CREATE INDEX idx_lesson_progress_is_completed ON lesson_progress(is_completed);

-- Payment table indexes
CREATE INDEX idx_payments_user_id ON payments(user_id);
CREATE INDEX idx_payments_course_id ON payments(course_id);
CREATE INDEX idx_payments_status ON payments(payment_status);
CREATE INDEX idx_payments_created_at ON payments(created_at);

-- Review table indexes
CREATE INDEX idx_reviews_course_id ON reviews(course_id);
CREATE INDEX idx_reviews_student_id ON reviews(student_id);
CREATE INDEX idx_reviews_rating ON reviews(rating);

-- Course sections and lessons indexes
CREATE INDEX idx_course_sections_course_id ON course_sections(course_id);
CREATE INDEX idx_course_sections_order_index ON course_sections(order_index);
CREATE INDEX idx_lessons_section_id ON lessons(section_id);
CREATE INDEX idx_lessons_order_index ON lessons(order_index);
CREATE INDEX idx_lessons_is_published ON lessons(is_published);

-- Resources indexes
CREATE INDEX idx_resources_lesson_id ON resources(lesson_id);
CREATE INDEX idx_resources_file_type ON resources(file_type);

-- Categories indexes
CREATE INDEX idx_categories_parent_id ON categories(parent_id);
CREATE INDEX idx_categories_is_active ON categories(is_active);

-- Instructor applications indexes
CREATE INDEX idx_instructor_applications_user_id ON instructor_applications(user_id);
CREATE INDEX idx_instructor_applications_status ON instructor_applications(application_status);

-- =====================================================
-- STEP 4: INSERT DEFAULT DATA
-- =====================================================

-- Insert default roles
INSERT INTO roles (name, description, permissions) VALUES 
('Admin', 'System administrator with full access', '{"users": ["create", "read", "update", "delete"], "courses": ["create", "read", "update", "delete"], "instructors": ["approve", "reject", "manage"]}'),
('Instructor', 'Course instructor with teaching permissions', '{"courses": ["create", "read", "update", "delete"], "students": ["read"], "lessons": ["create", "read", "update", "delete"]}'),
('Student', 'Regular student with learning permissions', '{"courses": ["read"], "enrollments": ["create", "read"], "lessons": ["read"]}');

-- Insert sample categories
INSERT INTO categories (name, description, is_active) VALUES 
('Programming', 'Software development and coding courses', true),
('Design', 'UI/UX design and graphic design courses', true),
('Business', 'Business management and entrepreneurship courses', true),
('Marketing', 'Digital marketing and advertising courses', true),
('Data Science', 'Data analysis and machine learning courses', true),
('Web Development', 'Frontend and backend web development', true),
('Mobile Development', 'iOS and Android app development', true),
('DevOps', 'Development operations and deployment', true);

-- Insert sample admin user (password: admin123 - hash this in your application)
INSERT INTO users (name, email, password_hash, role_id, is_active, email_verified) VALUES 
('System Administrator', 'admin@edustack.com', '$2b$10$92IXUNpkjO0rOQ5byMi.Ye4oKoEa3Ro9llC/.og/at2.uheWG/igi', 1, true, true);

-- Insert sample instructor
INSERT INTO users (name, email, password_hash, role_id, is_active, email_verified) VALUES 
('John Instructor', 'instructor@edustack.com', '$2b$10$92IXUNpkjO0rOQ5byMi.Ye4oKoEa3Ro9llC/.og/at2.uheWG/igi', 2, true, true);

-- Insert sample student
INSERT INTO users (name, email, password_hash, role_id, is_active, email_verified) VALUES 
('Jane Student', 'student@edustack.com', '$2b$10$92IXUNpkjO0rOQ5byMi.Ye4oKoEa3Ro9llC/.og/at2.uheWG/igi', 3, true, true);

-- Insert sample course
INSERT INTO courses (title, description, price, instructor_id, category_id, is_published, difficulty_level, duration_hours) VALUES 
('Complete Web Development Course', 'Learn full-stack web development from scratch using modern technologies', 99.99, 2, 6, true, 'beginner', 40),
('Advanced React Development', 'Master React.js with hooks, context, and advanced patterns', 149.99, 2, 6, true, 'intermediate', 25),
('Python for Data Science', 'Learn data analysis and machine learning with Python', 199.99, 2, 5, true, 'beginner', 35);

-- Insert course sections for the first course
INSERT INTO course_sections (course_id, title, order_index, is_published) VALUES 
(1, 'Introduction to Web Development', 1, true),
(1, 'HTML Fundamentals', 2, true),
(1, 'CSS Styling', 3, true),
(1, 'JavaScript Basics', 4, true),
(1, 'Project: Build a Portfolio Website', 5, true);

-- Insert sample lessons for the first section
INSERT INTO lessons (section_id, title, lesson_type, content, order_index, is_published, duration_minutes, is_preview) VALUES 
(1, 'What is Web Development?', 'video', 'Introduction to web development concepts and career opportunities', 1, true, 15, true),
(1, 'Setting up Development Environment', 'video', 'Install and configure VS Code, Git, and browser developer tools', 2, true, 20, false),
(1, 'Understanding the Web', 'text', 'How the internet works, HTTP, and web servers', 3, true, 10, true);

-- Insert sample lessons for HTML section
INSERT INTO lessons (section_id, title, lesson_type, content, order_index, is_published, duration_minutes, is_preview) VALUES 
(2, 'HTML Document Structure', 'video', 'Learn about DOCTYPE, html, head, and body elements', 1, true, 25, true),
(2, 'HTML Elements and Tags', 'video', 'Common HTML elements: headings, paragraphs, lists, links', 2, true, 30, false),
(2, 'HTML Forms', 'video', 'Create forms with input fields, buttons, and validation', 3, true, 35, false);

-- Insert sample resources
INSERT INTO resources (lesson_id, file_name, file_url, file_type, file_size) VALUES 
(1, 'web-development-overview.pdf', 'https://example.com/resources/web-dev-overview.pdf', 'pdf', 1024000),
(2, 'development-setup-guide.pdf', 'https://example.com/resources/setup-guide.pdf', 'pdf', 2048000),
(4, 'html-cheatsheet.pdf', 'https://example.com/resources/html-cheatsheet.pdf', 'pdf', 512000);

-- Insert sample enrollment
INSERT INTO enrollments (student_id, course_id, progress_percentage, payment_status) VALUES 
(3, 1, 25.50, 'completed'),
(3, 2, 0.00, 'pending');

-- Insert sample lesson progress
INSERT INTO lesson_progress (student_id, lesson_id, is_completed, time_spent_minutes) VALUES 
(3, 1, true, 15),
(3, 2, true, 20),
(3, 4, true, 25);

-- Insert sample payment
INSERT INTO payments (user_id, course_id, amount, payment_method, payment_status, transaction_id) VALUES 
(3, 1, 99.99, 'stripe', 'completed', 'stripe_txn_123456789');

-- Insert sample review
INSERT INTO reviews (student_id, course_id, rating, comment, is_approved) VALUES 
(3, 1, 5, 'Excellent course! Very well structured and easy to follow.', true);

-- =====================================================
-- STEP 5: CREATE UTILITY FUNCTIONS
-- =====================================================

-- Create health check function
CREATE OR REPLACE FUNCTION check_database_health()
RETURNS TABLE(
    status TEXT,
    database_name TEXT,
    connection_count INTEGER,
    timestamp TIMESTAMP
) AS $$
BEGIN
    RETURN QUERY
    SELECT 
        'healthy'::TEXT as status,
        current_database()::TEXT as database_name,
        (SELECT count(*) FROM pg_stat_activity WHERE datname = current_database())::INTEGER as connection_count,
        now()::TIMESTAMP as timestamp;
END;
$$ LANGUAGE plpgsql;

-- Create function to get course statistics
CREATE OR REPLACE FUNCTION get_course_stats(course_id_param INTEGER)
RETURNS TABLE(
    total_students INTEGER,
    completion_rate DECIMAL,
    average_rating DECIMAL,
    total_revenue DECIMAL
) AS $$
BEGIN
    RETURN QUERY
    SELECT 
        COUNT(DISTINCT e.student_id)::INTEGER as total_students,
        ROUND(AVG(e.progress_percentage), 2) as completion_rate,
        ROUND(AVG(r.rating), 2) as average_rating,
        COALESCE(SUM(p.amount), 0) as total_revenue
    FROM enrollments e
    LEFT JOIN reviews r ON e.course_id = r.course_id
    LEFT JOIN payments p ON e.course_id = p.course_id AND p.payment_status = 'completed'
    WHERE e.course_id = course_id_param;
END;
$$ LANGUAGE plpgsql;

-- =====================================================
-- STEP 6: GRANT FINAL PERMISSIONS
-- =====================================================

-- Grant all necessary permissions to the application user
GRANT ALL PRIVILEGES ON ALL TABLES IN SCHEMA public TO edustack_user;
GRANT ALL PRIVILEGES ON ALL SEQUENCES IN SCHEMA public TO edustack_user;
GRANT EXECUTE ON ALL FUNCTIONS IN SCHEMA public TO edustack_user;

-- =====================================================
-- STEP 7: VERIFICATION QUERIES
-- =====================================================

-- Display all created tables
SELECT 'Tables Created:' as info;
SELECT table_name 
FROM information_schema.tables 
WHERE table_schema = 'public' 
ORDER BY table_name;

-- Display all created indexes
SELECT 'Indexes Created:' as info;
SELECT indexname, tablename 
FROM pg_indexes 
WHERE schemaname = 'public' 
ORDER BY tablename, indexname;

-- Display foreign key constraints
SELECT 'Foreign Key Constraints:' as info;
SELECT 
    tc.table_name, 
    kcu.column_name, 
    ccu.table_name AS foreign_table_name,
    ccu.column_name AS foreign_column_name 
FROM information_schema.table_constraints AS tc 
JOIN information_schema.key_column_usage AS kcu
    ON tc.constraint_name = kcu.constraint_name
    AND tc.table_schema = kcu.table_schema
JOIN information_schema.constraint_column_usage AS ccu
    ON ccu.constraint_name = tc.constraint_name
    AND ccu.table_schema = tc.table_schema
WHERE tc.constraint_type = 'FOREIGN KEY' 
    AND tc.table_schema = 'public'
ORDER BY tc.table_name;

-- Test database health
SELECT 'Database Health Check:' as info;
SELECT * FROM check_database_health();

-- Display sample data counts
SELECT 'Sample Data Counts:' as info;
SELECT 'Users' as table_name, COUNT(*) as count FROM users
UNION ALL
SELECT 'Roles', COUNT(*) FROM roles
UNION ALL
SELECT 'Categories', COUNT(*) FROM categories
UNION ALL
SELECT 'Courses', COUNT(*) FROM courses
UNION ALL
SELECT 'Course Sections', COUNT(*) FROM course_sections
UNION ALL
SELECT 'Lessons', COUNT(*) FROM lessons
UNION ALL
SELECT 'Resources', COUNT(*) FROM resources
UNION ALL
SELECT 'Enrollments', COUNT(*) FROM enrollments
UNION ALL
SELECT 'Lesson Progress', COUNT(*) FROM lesson_progress
UNION ALL
SELECT 'Payments', COUNT(*) FROM payments
UNION ALL
SELECT 'Reviews', COUNT(*) FROM reviews;

-- =====================================================
-- SETUP COMPLETE!
-- =====================================================

SELECT '=====================================================' as info;
SELECT 'EduStack Database Setup Complete!' as info;
SELECT '=====================================================' as info;
SELECT 'Database: edustack' as info;
SELECT 'User: edustack_user' as info;
SELECT 'Password: edustack_secure_password_2025' as info;
SELECT '=====================================================' as info;
SELECT 'Next Steps:' as info;
SELECT '1. Update your .env file with the database credentials' as info;
SELECT '2. Test the database connection from your application' as info;
SELECT '3. Start building your EduStack eLearning platform!' as info;
SELECT '=====================================================' as info;
