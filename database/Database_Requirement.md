# 🗄️ EduStack Database Requirements & Manual Setup Guide

## 📋 Overview
This document provides comprehensive instructions for manually setting up the EduStack eLearning platform database using **PostgreSQL**. This guide covers database creation, table setup, relationships, and initial data seeding without using ORM migrations.

> **⚠️ Manual Setup Approach:** This guide focuses on manual PostgreSQL database creation and setup, providing all necessary SQL scripts and step-by-step instructions.

---

## 🎯 Project Requirements Summary

### **EduStack E-Learning Platform Features**
- **Three User Roles:** Student, Instructor, Admin
- **Core Functionality:** Course enrollment, payment processing, progress tracking
- **Content Management:** Course creation, lesson management, resource uploads
- **Payment Integration:** Stripe/PayPal support for course payments
- **Progress Tracking:** Student progress monitoring and completion tracking

### **Database Requirements**
- **Database Type:** PostgreSQL 14+
- **Connection:** Manual setup with connection pooling
- **Security:** Role-based access control (RBAC)
- **Performance:** Optimized indexes for query performance
- **Backup:** Manual backup and recovery procedures

---

## 🏗️ Manual Database Setup

### **Step 1: PostgreSQL Installation & Configuration**

#### **Install PostgreSQL**
```bash
# Windows (using Chocolatey)
choco install postgresql

# Or download from: https://www.postgresql.org/download/windows/

# macOS (using Homebrew)
brew install postgresql

# Ubuntu/Debian
sudo apt update
sudo apt install postgresql postgresql-contrib
```

#### **Initial Configuration**
```bash
# Start PostgreSQL service
# Windows: Start PostgreSQL service from Services
# macOS: brew services start postgresql
# Linux: sudo systemctl start postgresql

# Connect to PostgreSQL
psql -U postgres
```

### **Step 2: Create Database and User**

#### **Database Creation Script**
```sql
-- Connect as postgres superuser
-- Create database
CREATE DATABASE edustack;

-- Create application user
CREATE USER edustack_user WITH PASSWORD 'your_secure_password';

-- Grant privileges
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
```

---

## 📊 Database Schema Creation

### **Step 3: Create All Tables**

#### **3.1 Create Roles Table**
```sql
-- Create roles table
CREATE TABLE roles (
    id SERIAL PRIMARY KEY,
    name VARCHAR(50) UNIQUE NOT NULL,
    description TEXT,
    permissions JSONB,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Insert default roles
INSERT INTO roles (name, description, permissions) VALUES 
('Admin', 'System administrator with full access', '{"users": ["create", "read", "update", "delete"], "courses": ["create", "read", "update", "delete"], "instructors": ["approve", "reject", "manage"]}'),
('Instructor', 'Course instructor with teaching permissions', '{"courses": ["create", "read", "update", "delete"], "students": ["read"], "lessons": ["create", "read", "update", "delete"]}'),
('Student', 'Regular student with learning permissions', '{"courses": ["read"], "enrollments": ["create", "read"], "lessons": ["read"]}');
```

#### **3.2 Create Users Table**
```sql
-- Create users table
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
```

#### **3.3 Create Categories Table**
```sql
-- Create categories table
CREATE TABLE categories (
    id SERIAL PRIMARY KEY,
    name VARCHAR(100) UNIQUE NOT NULL,
    description TEXT,
    parent_id INTEGER REFERENCES categories(id),
    icon_url VARCHAR(500),
    is_active BOOLEAN DEFAULT true,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Insert sample categories
INSERT INTO categories (name, description, is_active) VALUES 
('Programming', 'Software development and coding courses', true),
('Design', 'UI/UX design and graphic design courses', true),
('Business', 'Business management and entrepreneurship courses', true),
('Marketing', 'Digital marketing and advertising courses', true),
('Data Science', 'Data analysis and machine learning courses', true);
```

#### **3.4 Create Courses Table**
```sql
-- Create courses table
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
```

#### **3.5 Create Course Sections Table**
```sql
-- Create course sections table
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
```

#### **3.6 Create Lessons Table**
```sql
-- Create lessons table
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
```

#### **3.7 Create Resources Table**
```sql
-- Create resources table
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
```

#### **3.8 Create Enrollments Table**
```sql
-- Create enrollments table
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
```

#### **3.9 Create Lesson Progress Table**
```sql
-- Create lesson progress table
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
```

#### **3.10 Create Payments Table**
```sql
-- Create payments table
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
```

#### **3.11 Create Reviews Table**
```sql
-- Create reviews table
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
```

#### **3.12 Create Instructor Applications Table**
```sql
-- Create instructor applications table
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
```

---

## 🔗 Database Relationships & Constraints

### **Step 4: Add Foreign Key Constraints**

```sql
-- Add foreign key constraints (if not already added during table creation)
ALTER TABLE users ADD CONSTRAINT fk_users_role_id FOREIGN KEY (role_id) REFERENCES roles(id);
ALTER TABLE courses ADD CONSTRAINT fk_courses_instructor_id FOREIGN KEY (instructor_id) REFERENCES users(id);
ALTER TABLE courses ADD CONSTRAINT fk_courses_category_id FOREIGN KEY (category_id) REFERENCES categories(id);
ALTER TABLE course_sections ADD CONSTRAINT fk_course_sections_course_id FOREIGN KEY (course_id) REFERENCES courses(id) ON DELETE CASCADE;
ALTER TABLE lessons ADD CONSTRAINT fk_lessons_section_id FOREIGN KEY (section_id) REFERENCES course_sections(id) ON DELETE CASCADE;
ALTER TABLE resources ADD CONSTRAINT fk_resources_lesson_id FOREIGN KEY (lesson_id) REFERENCES lessons(id) ON DELETE CASCADE;
ALTER TABLE enrollments ADD CONSTRAINT fk_enrollments_student_id FOREIGN KEY (student_id) REFERENCES users(id);
ALTER TABLE enrollments ADD CONSTRAINT fk_enrollments_course_id FOREIGN KEY (course_id) REFERENCES courses(id);
ALTER TABLE lesson_progress ADD CONSTRAINT fk_lesson_progress_student_id FOREIGN KEY (student_id) REFERENCES users(id);
ALTER TABLE lesson_progress ADD CONSTRAINT fk_lesson_progress_lesson_id FOREIGN KEY (lesson_id) REFERENCES lessons(id);
ALTER TABLE payments ADD CONSTRAINT fk_payments_user_id FOREIGN KEY (user_id) REFERENCES users(id);
ALTER TABLE payments ADD CONSTRAINT fk_payments_course_id FOREIGN KEY (course_id) REFERENCES courses(id);
ALTER TABLE reviews ADD CONSTRAINT fk_reviews_student_id FOREIGN KEY (student_id) REFERENCES users(id);
ALTER TABLE reviews ADD CONSTRAINT fk_reviews_course_id FOREIGN KEY (course_id) REFERENCES courses(id);
ALTER TABLE instructor_applications ADD CONSTRAINT fk_instructor_applications_user_id FOREIGN KEY (user_id) REFERENCES users(id);
ALTER TABLE instructor_applications ADD CONSTRAINT fk_instructor_applications_reviewed_by FOREIGN KEY (reviewed_by) REFERENCES users(id);
ALTER TABLE categories ADD CONSTRAINT fk_categories_parent_id FOREIGN KEY (parent_id) REFERENCES categories(id);
```

---

## 📈 Performance Optimization

### **Step 5: Create Database Indexes**

```sql
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
```

---

## 🌱 Initial Data Seeding

### **Step 6: Insert Sample Data**

#### **6.1 Create Admin User**
```sql
-- Insert admin user (password: admin123 - hash this in your application)
INSERT INTO users (name, email, password_hash, role_id, is_active, email_verified) VALUES 
('System Administrator', 'admin@edustack.com', '$2b$10$example_hash_here', 1, true, true);
```

#### **6.2 Create Sample Instructor**
```sql
-- Insert sample instructor
INSERT INTO users (name, email, password_hash, role_id, is_active, email_verified) VALUES 
('John Instructor', 'instructor@edustack.com', '$2b$10$example_hash_here', 2, true, true);
```

#### **6.3 Create Sample Student**
```sql
-- Insert sample student
INSERT INTO users (name, email, password_hash, role_id, is_active, email_verified) VALUES 
('Jane Student', 'student@edustack.com', '$2b$10$example_hash_here', 3, true, true);
```

#### **6.4 Create Sample Course**
```sql
-- Insert sample course
INSERT INTO courses (title, description, price, instructor_id, category_id, is_published, difficulty_level, duration_hours) VALUES 
('Complete Web Development Course', 'Learn full-stack web development from scratch', 99.99, 2, 1, true, 'beginner', 40);

-- Insert course section
INSERT INTO course_sections (course_id, title, order_index, is_published) VALUES 
(1, 'Introduction to Web Development', 1, true);

-- Insert sample lesson
INSERT INTO lessons (section_id, title, lesson_type, content, order_index, is_published, duration_minutes) VALUES 
(1, 'What is Web Development?', 'video', 'Introduction to web development concepts', 1, true, 15);
```

---

## 🔒 Security Configuration

### **Step 7: Database Security Setup**

#### **7.1 Create Application User with Limited Privileges**
```sql
-- Create application user with limited privileges
CREATE USER edustack_app WITH PASSWORD 'app_secure_password';

-- Grant only necessary privileges
GRANT CONNECT ON DATABASE edustack TO edustack_app;
GRANT USAGE ON SCHEMA public TO edustack_app;
GRANT SELECT, INSERT, UPDATE, DELETE ON ALL TABLES IN SCHEMA public TO edustack_app;
GRANT USAGE, SELECT ON ALL SEQUENCES IN SCHEMA public TO edustack_app;

-- Set default privileges for future tables
ALTER DEFAULT PRIVILEGES IN SCHEMA public GRANT SELECT, INSERT, UPDATE, DELETE ON TABLES TO edustack_app;
ALTER DEFAULT PRIVILEGES IN SCHEMA public GRANT USAGE, SELECT ON SEQUENCES TO edustack_app;
```

#### **7.2 Enable SSL (Production)**
```sql
-- In postgresql.conf, set:
-- ssl = on
-- ssl_cert_file = 'server.crt'
-- ssl_key_file = 'server.key'
```

---

## 📊 Database Monitoring & Maintenance

### **Step 8: Setup Monitoring**

#### **8.1 Create Health Check Function**
```sql
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
```

#### **8.2 Create Backup Script**
```bash
#!/bin/bash
# backup_edustack.sh
# Database backup script

DB_NAME="edustack"
DB_USER="edustack_user"
BACKUP_DIR="/path/to/backups"
DATE=$(date +%Y%m%d_%H%M%S)

# Create backup
pg_dump -U $DB_USER -h localhost $DB_NAME > $BACKUP_DIR/edustack_backup_$DATE.sql

# Compress backup
gzip $BACKUP_DIR/edustack_backup_$DATE.sql

# Keep only last 7 days of backups
find $BACKUP_DIR -name "edustack_backup_*.sql.gz" -mtime +7 -delete

echo "Backup completed: edustack_backup_$DATE.sql.gz"
```

---

## 🚀 Application Connection Configuration

### **Step 9: Environment Variables for Application**

Create `.env` file in your backend directory:

```bash
# Database Configuration
DB_HOST=localhost
DB_PORT=5432
DB_NAME=edustack
DB_USER=edustack_app
DB_PASSWORD=app_secure_password
DB_SSL=false

# JWT Configuration
JWT_SECRET=your_jwt_secret_key_here
JWT_REFRESH_SECRET=your_refresh_secret_key_here
JWT_EXPIRE=24h
JWT_REFRESH_EXPIRE=7d

# File Upload Configuration
CLOUDINARY_URL=your_cloudinary_url
AWS_ACCESS_KEY_ID=your_aws_key
AWS_SECRET_ACCESS_KEY=your_aws_secret
AWS_BUCKET_NAME=your_bucket_name

# Payment Configuration
STRIPE_SECRET_KEY=your_stripe_secret_key
STRIPE_WEBHOOK_SECRET=your_stripe_webhook_secret
PAYPAL_CLIENT_ID=your_paypal_client_id
PAYPAL_CLIENT_SECRET=your_paypal_secret
```

---

## 📋 Verification Checklist

### **Step 10: Database Setup Verification**

```sql
-- Verify all tables exist
SELECT table_name 
FROM information_schema.tables 
WHERE table_schema = 'public' 
ORDER BY table_name;

-- Verify all indexes exist
SELECT indexname, tablename 
FROM pg_indexes 
WHERE schemaname = 'public' 
ORDER BY tablename, indexname;

-- Verify foreign key constraints
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
SELECT * FROM check_database_health();
```

---

## 🔧 Troubleshooting

### **Common Issues & Solutions**

#### **Issue 1: Permission Denied**
```sql
-- Grant necessary permissions
GRANT ALL PRIVILEGES ON DATABASE edustack TO edustack_user;
GRANT ALL ON SCHEMA public TO edustack_user;
```

#### **Issue 2: Foreign Key Constraint Errors**
```sql
-- Check for orphaned records
SELECT * FROM users WHERE role_id NOT IN (SELECT id FROM roles);
```

#### **Issue 3: Connection Issues**
```bash
# Check PostgreSQL service status
# Windows: services.msc
# Linux: sudo systemctl status postgresql
# macOS: brew services list | grep postgresql
```

---

## 📚 Additional Resources

### **Useful Commands**
```sql
-- View table sizes
SELECT 
    schemaname,
    tablename,
    pg_size_pretty(pg_total_relation_size(schemaname||'.'||tablename)) as size
FROM pg_tables 
WHERE schemaname = 'public'
ORDER BY pg_total_relation_size(schemaname||'.'||tablename) DESC;

-- View active connections
SELECT pid, usename, application_name, client_addr, state 
FROM pg_stat_activity 
WHERE datname = 'edustack';

-- View slow queries
SELECT query, mean_time, calls 
FROM pg_stat_statements 
ORDER BY mean_time DESC 
LIMIT 10;
```

---

**Last Updated:** January 2025  
**Database Version:** PostgreSQL 14+  
**Setup Type:** Manual PostgreSQL Configuration  
**Maintainer:** EduStack Development Team

---

## 📝 Next Steps

After completing this manual database setup:

1. **Test Database Connection** - Verify your application can connect to the database
2. **Implement Authentication** - Set up JWT authentication with the created user roles
3. **Create API Endpoints** - Build REST API endpoints for all database operations
4. **Add Data Validation** - Implement proper input validation and sanitization
5. **Setup Monitoring** - Configure database monitoring and alerting
6. **Create Backup Strategy** - Implement automated backup and recovery procedures
