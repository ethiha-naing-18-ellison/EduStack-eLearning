# 🗄️ EduStack Database Documentation

## 📋 Overview
This document provides a comprehensive guide to the EduStack eLearning platform database structure. The database is built using **PostgreSQL** with **raw SQL queries** and **Sequelize ORM** for database operations.

> **⚠️ Important Note:** This project does **NOT** use Prisma ORM. All database operations are handled through Sequelize ORM and raw SQL queries.

---

## 🏗️ Database Architecture

### **Database Management**
- **Database Type:** PostgreSQL
- **ORM:** Sequelize (Node.js)
- **Connection Pool:** pg-pool for connection management
- **Migrations:** Custom migration system with Sequelize
- **Seeding:** Custom seed files for initial data

### **Database Configuration**
```javascript
// Database connection configuration
const dbConfig = {
  host: process.env.DB_HOST || 'localhost',
  port: process.env.DB_PORT || 5432,
  database: process.env.DB_NAME || 'edustack',
  username: process.env.DB_USER || 'postgres',
  password: process.env.DB_PASSWORD || 'password',
  dialect: 'postgresql',
  pool: {
    max: 5,
    min: 0,
    acquire: 30000,
    idle: 10000
  }
};
```

---

## 📊 Database Schema

### **Core Tables Structure**

#### 1. **Users Table**
```sql
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

#### 2. **Roles Table**
```sql
CREATE TABLE roles (
    id SERIAL PRIMARY KEY,
    name VARCHAR(50) UNIQUE NOT NULL,
    description TEXT,
    permissions JSONB,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);
```

#### 3. **Courses Table**
```sql
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

#### 4. **Course Sections Table**
```sql
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

#### 5. **Lessons Table**
```sql
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

#### 6. **Resources Table**
```sql
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

#### 7. **Enrollments Table**
```sql
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

#### 8. **Lesson Progress Table**
```sql
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

#### 9. **Payments Table**
```sql
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

#### 10. **Reviews Table**
```sql
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

#### 11. **Instructor Applications Table**
```sql
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

#### 12. **Categories Table**
```sql
CREATE TABLE categories (
    id SERIAL PRIMARY KEY,
    name VARCHAR(100) UNIQUE NOT NULL,
    description TEXT,
    parent_id INTEGER REFERENCES categories(id),
    icon_url VARCHAR(500),
    is_active BOOLEAN DEFAULT true,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);
```

---

## 🔗 Database Relationships

### **Entity Relationship Diagram (ERD)**

```
Users (1) ←→ (M) Roles
Users (1) ←→ (M) Courses [as instructor]
Users (1) ←→ (M) Enrollments
Users (1) ←→ (M) Reviews
Users (1) ←→ (M) Payments
Users (1) ←→ (M) Lesson Progress

Courses (1) ←→ (M) Course Sections
Courses (1) ←→ (M) Enrollments
Courses (1) ←→ (M) Reviews
Courses (1) ←→ (M) Payments
Courses (M) ←→ (1) Categories

Course Sections (1) ←→ (M) Lessons
Lessons (1) ←→ (M) Resources
Lessons (1) ←→ (M) Lesson Progress

Categories (1) ←→ (M) Categories [self-referencing]
Categories (1) ←→ (M) Courses
```

---

## 🛠️ Database Operations

### **Sequelize Models**

#### User Model
```javascript
const { DataTypes } = require('sequelize');

const User = sequelize.define('User', {
  id: {
    type: DataTypes.INTEGER,
    primaryKey: true,
    autoIncrement: true
  },
  name: {
    type: DataTypes.STRING(255),
    allowNull: false
  },
  email: {
    type: DataTypes.STRING(255),
    allowNull: false,
    unique: true
  },
  password_hash: {
    type: DataTypes.STRING(255),
    allowNull: false
  },
  role_id: {
    type: DataTypes.INTEGER,
    references: {
      model: 'roles',
      key: 'id'
    }
  },
  profile_image: DataTypes.STRING(500),
  phone: DataTypes.STRING(20),
  bio: DataTypes.TEXT,
  is_active: {
    type: DataTypes.BOOLEAN,
    defaultValue: true
  },
  email_verified: {
    type: DataTypes.BOOLEAN,
    defaultValue: false
  }
}, {
  tableName: 'users',
  timestamps: true
});
```

#### Course Model
```javascript
const Course = sequelize.define('Course', {
  id: {
    type: DataTypes.INTEGER,
    primaryKey: true,
    autoIncrement: true
  },
  title: {
    type: DataTypes.STRING(255),
    allowNull: false
  },
  description: DataTypes.TEXT,
  price: {
    type: DataTypes.DECIMAL(10, 2),
    defaultValue: 0.00
  },
  instructor_id: {
    type: DataTypes.INTEGER,
    references: {
      model: 'users',
      key: 'id'
    }
  },
  category_id: {
    type: DataTypes.INTEGER,
    references: {
      model: 'categories',
      key: 'id'
    }
  },
  thumbnail_url: DataTypes.STRING(500),
  is_published: {
    type: DataTypes.BOOLEAN,
    defaultValue: false
  },
  difficulty_level: {
    type: DataTypes.STRING(20),
    defaultValue: 'beginner'
  },
  duration_hours: {
    type: DataTypes.INTEGER,
    defaultValue: 0
  },
  language: {
    type: DataTypes.STRING(10),
    defaultValue: 'en'
  }
}, {
  tableName: 'courses',
  timestamps: true
});
```

### **Model Associations**
```javascript
// User associations
User.belongsTo(Role, { foreignKey: 'role_id' });
User.hasMany(Course, { foreignKey: 'instructor_id', as: 'InstructorCourses' });
User.hasMany(Enrollment, { foreignKey: 'student_id' });
User.hasMany(Review, { foreignKey: 'student_id' });
User.hasMany(Payment, { foreignKey: 'user_id' });
User.hasMany(LessonProgress, { foreignKey: 'student_id' });

// Course associations
Course.belongsTo(User, { foreignKey: 'instructor_id', as: 'Instructor' });
Course.belongsTo(Category, { foreignKey: 'category_id' });
Course.hasMany(CourseSection, { foreignKey: 'course_id' });
Course.hasMany(Enrollment, { foreignKey: 'course_id' });
Course.hasMany(Review, { foreignKey: 'course_id' });
Course.hasMany(Payment, { foreignKey: 'course_id' });

// Course Section associations
CourseSection.belongsTo(Course, { foreignKey: 'course_id' });
CourseSection.hasMany(Lesson, { foreignKey: 'section_id' });

// Lesson associations
Lesson.belongsTo(CourseSection, { foreignKey: 'section_id' });
Lesson.hasMany(Resource, { foreignKey: 'lesson_id' });
Lesson.hasMany(LessonProgress, { foreignKey: 'lesson_id' });

// Enrollment associations
Enrollment.belongsTo(User, { foreignKey: 'student_id' });
Enrollment.belongsTo(Course, { foreignKey: 'course_id' });
```

---

## 📈 Database Indexes

### **Performance Optimization Indexes**
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
```

---

## 🔄 Database Migrations

### **Migration Structure**
```
backend/
├── migrations/
│   ├── 001_create_roles_table.js
│   ├── 002_create_users_table.js
│   ├── 003_create_categories_table.js
│   ├── 004_create_courses_table.js
│   ├── 005_create_course_sections_table.js
│   ├── 006_create_lessons_table.js
│   ├── 007_create_resources_table.js
│   ├── 008_create_enrollments_table.js
│   ├── 009_create_lesson_progress_table.js
│   ├── 010_create_payments_table.js
│   ├── 011_create_reviews_table.js
│   └── 012_create_instructor_applications_table.js
```

### **Sample Migration File**
```javascript
// migrations/001_create_roles_table.js
module.exports = {
  up: async (queryInterface, Sequelize) => {
    await queryInterface.createTable('roles', {
      id: {
        type: Sequelize.INTEGER,
        primaryKey: true,
        autoIncrement: true
      },
      name: {
        type: Sequelize.STRING(50),
        allowNull: false,
        unique: true
      },
      description: Sequelize.TEXT,
      permissions: Sequelize.JSONB,
      created_at: {
        type: Sequelize.DATE,
        allowNull: false,
        defaultValue: Sequelize.NOW
      }
    });
  },

  down: async (queryInterface, Sequelize) => {
    await queryInterface.dropTable('roles');
  }
};
```

---

## 🌱 Database Seeding

### **Seed Data Structure**
```
backend/
├── seeds/
│   ├── 001_roles_seed.js
│   ├── 002_categories_seed.js
│   ├── 003_admin_user_seed.js
│   └── 004_sample_courses_seed.js
```

### **Sample Seed File**
```javascript
// seeds/001_roles_seed.js
module.exports = {
  up: async (queryInterface, Sequelize) => {
    await queryInterface.bulkInsert('roles', [
      {
        name: 'Admin',
        description: 'System administrator with full access',
        permissions: JSON.stringify({
          users: ['create', 'read', 'update', 'delete'],
          courses: ['create', 'read', 'update', 'delete'],
          instructors: ['approve', 'reject', 'manage']
        }),
        created_at: new Date(),
        updated_at: new Date()
      },
      {
        name: 'Instructor',
        description: 'Course instructor with teaching permissions',
        permissions: JSON.stringify({
          courses: ['create', 'read', 'update', 'delete'],
          students: ['read'],
          lessons: ['create', 'read', 'update', 'delete']
        }),
        created_at: new Date(),
        updated_at: new Date()
      },
      {
        name: 'Student',
        description: 'Regular student with learning permissions',
        permissions: JSON.stringify({
          courses: ['read'],
          enrollments: ['create', 'read'],
          lessons: ['read']
        }),
        created_at: new Date(),
        updated_at: new Date()
      }
    ]);
  },

  down: async (queryInterface, Sequelize) => {
    await queryInterface.bulkDelete('roles', null, {});
  }
};
```

---

## 🔒 Database Security

### **Security Measures**
1. **Connection Security**
   - SSL/TLS encryption for database connections
   - Environment-based configuration
   - Connection pooling with limits

2. **Data Protection**
   - Password hashing with bcrypt
   - JWT token-based authentication
   - Role-based access control (RBAC)

3. **Query Security**
   - Parameterized queries to prevent SQL injection
   - Input validation and sanitization
   - Rate limiting on API endpoints

### **Environment Variables**
```bash
# Database Configuration
DB_HOST=localhost
DB_PORT=5432
DB_NAME=edustack
DB_USER=postgres
DB_PASSWORD=your_secure_password
DB_SSL=true

# JWT Configuration
JWT_SECRET=your_jwt_secret_key
JWT_REFRESH_SECRET=your_refresh_secret_key
JWT_EXPIRE=24h
JWT_REFRESH_EXPIRE=7d

# File Upload Configuration
CLOUDINARY_URL=your_cloudinary_url
AWS_ACCESS_KEY_ID=your_aws_key
AWS_SECRET_ACCESS_KEY=your_aws_secret
AWS_BUCKET_NAME=your_bucket_name
```

---

## 📊 Database Monitoring

### **Performance Monitoring**
- Query execution time tracking
- Database connection monitoring
- Index usage analysis
- Slow query logging

### **Health Checks**
```javascript
// Database health check endpoint
app.get('/api/health/database', async (req, res) => {
  try {
    await sequelize.authenticate();
    res.json({
      status: 'healthy',
      database: 'connected',
      timestamp: new Date().toISOString()
    });
  } catch (error) {
    res.status(500).json({
      status: 'unhealthy',
      database: 'disconnected',
      error: error.message
    });
  }
});
```

---

## 🚀 Deployment Considerations

### **Production Database Setup**
1. **PostgreSQL Configuration**
   - Connection pooling
   - Memory optimization
   - Backup strategies

2. **Database Hosting Options**
   - AWS RDS PostgreSQL
   - Google Cloud SQL
   - Azure Database for PostgreSQL
   - Supabase PostgreSQL

3. **Backup Strategy**
   - Automated daily backups
   - Point-in-time recovery
   - Cross-region replication

---

## 📝 Database Documentation Maintenance

This database documentation should be updated whenever:
- New tables are added
- Existing table structures are modified
- New relationships are established
- Performance optimizations are implemented
- Security measures are updated

---

**Last Updated:** January 2025  
**Database Version:** PostgreSQL 14+  
**ORM Version:** Sequelize 6.x  
**Maintainer:** EduStack Development Team
