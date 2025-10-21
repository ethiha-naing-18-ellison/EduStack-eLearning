# EduStack-eLearning-Website

# 🎓 EduStack – Full-Stack E-Learning Platform

## 📘 Overview
EduStack is a **full-stack web application** for online learning that supports **three main roles**:
- **Student** – Enroll in courses, make payments, access and download resources, track progress.
- **Instructor** – Create and manage courses, upload lessons and resources, view student enrollments.
- **Admin** – Approve instructor applications, manage all users, courses, and transactions.

The system is built with **Node.js (Express)** for the backend, **React.js** for the frontend, and **PostgreSQL** for the database.

---

## 🧩 Features by Role

### 🧑‍🎓 Student
- Register, log in, and view available courses  
- Enroll in free or paid courses  
- Make payments via integrated gateway  
- Track lesson completion progress  
- Download or view course materials  
- Leave course reviews  

### 👩‍🏫 Instructor
- Apply for instructor role or be added by admin  
- Create, edit, or delete courses and lessons  
- Upload videos, PDFs, and quizzes  
- Manage enrolled students  
- View earnings and feedback  

### 🧑‍💼 Admin
- Approve or reject instructor registration  
- Manage all users (students, instructors, admins)  
- Manage all courses and course contents  
- Add or remove courses directly  
- Access system logs and reports  

---

## 🧱 Tech Stack

### **Frontend**
- React.js (Vite)
- React Router DOM
- Redux Toolkit or React Query
- Material-UI (MUI)
- Axios
- Stripe / PayPal SDK

### **Backend**
- Node.js + Express.js
- Sequelize ORM (for PostgreSQL)
- JWT Authentication (Access + Refresh tokens)
- Multer (file uploads)
- Cloudinary or AWS S3 (media storage)
- Stripe / PayPal integration
- Swagger for API documentation

### **Database**
- PostgreSQL (Recommended)
- Sequelize Models for table relations
- Custom migration system
- Raw SQL queries for complex operations

---

## 🗂️ Folder Structure

edustack/
│
├── backend/
│ ├── src/
│ │ ├── config/
│ │ ├── controllers/
│ │ ├── middleware/
│ │ ├── models/
│ │ ├── routes/
│ │ ├── services/
│ │ └── utils/
│ ├── migrations/
│ ├── seeds/
│ ├── .env
│ └── server.js
│
├── frontend/
│ ├── src/
│ │ ├── components/
│ │ ├── pages/
│ │ ├── hooks/
│ │ ├── redux/
│ │ └── utils/
│ ├── public/
│ ├── vite.config.js
│ └── package.json
│
└── README.md

markdown
Copy code

---

## 🗄️ Database Schema (Simplified ER Diagram)

**Main Entities:**
- **Users** → (id, name, email, passwordHash, roleId)
- **Roles** → (id, name) → Admin, Instructor, Student
- **Courses** → (id, title, description, price, instructorId, categoryId)
- **CourseSections** → (id, courseId, title, orderIndex)
- **Lessons** → (id, sectionId, title, type, content, videoUrl)
- **Resources** → (id, lessonId, fileName, fileUrl, fileType)
- **Enrollments** → (id, studentId, courseId, progress, paymentStatus)
- **LessonProgress** → (id, studentId, lessonId, isCompleted, timeSpent)
- **Payments** → (id, userId, courseId, amount, status, transactionId)
- **Reviews** → (id, studentId, courseId, rating, comment)
- **InstructorApplications** → (id, userId, status, qualifications)
- **Categories** → (id, name, description, parentId)

> **📋 Detailed Database Documentation:** See [DATABASE.md](./DATABASE.md) for comprehensive database schema, relationships, and operations.

---

## ⚙️ API Endpoints

### Auth
| Method | Endpoint | Description |
|---------|-----------|-------------|
| POST | `/api/auth/register` | Register a new user |
| POST | `/api/auth/login` | Log in and get tokens |
| POST | `/api/auth/refresh` | Refresh JWT token |

### Courses
| Method | Endpoint | Description |
|---------|-----------|-------------|
| GET | `/api/courses` | Get all published courses |
| GET | `/api/courses/:id` | Get course details |
| POST | `/api/courses` | Create a course (Instructor/Admin only) |
| PUT | `/api/courses/:id` | Update a course |
| DELETE | `/api/courses/:id` | Delete a course |

### Enrollment
| Method | Endpoint | Description |
|---------|-----------|-------------|
| POST | `/api/enroll` | Enroll in a course |
| GET | `/api/enrollments/:userId` | Get user enrollments |

### Admin
| Method | Endpoint | Description |
|---------|-----------|-------------|
| GET | `/api/admin/instructors/pending` | View pending instructor requests |
| PUT | `/api/admin/instructors/:id/approve` | Approve instructor |
| DELETE | `/api/admin/courses/:id` | Remove a course |

---

## 🧠 Role-Based Access Control (RBAC)

Authorization handled via middleware:
```js
function authorizeRoles(...roles) {
  return (req, res, next) => {
    if (!roles.includes(req.user.role)) {
      return res.status(403).json({ message: "Access denied" });
    }
    next();
  };
}
Usage example:

js
Copy code
router.post('/api/courses', authorizeRoles('Instructor', 'Admin'), createCourse);
🚀 Getting Started
1️⃣ Clone the Repository
bash
Copy code
git clone https://github.com/yourusername/edustack.git
cd edustack
2️⃣ Install Dependencies
bash
Copy code
cd backend && npm install
cd ../frontend && npm install
3️⃣ Setup Environment Variables (.env)
bash
Copy code
# Database Configuration
DB_HOST=localhost
DB_PORT=5432
DB_NAME=edustack
DB_USER=postgres
DB_PASSWORD=yourpassword

# JWT Configuration
JWT_SECRET="yoursecretkey"
JWT_REFRESH_SECRET="yourrefreshsecret"
JWT_EXPIRE=24h
JWT_REFRESH_EXPIRE=7d

# File Upload Configuration
CLOUDINARY_URL="yourcloudinaryurl"
AWS_ACCESS_KEY_ID="yourawskey"
AWS_SECRET_ACCESS_KEY="yourawssecret"
AWS_BUCKET_NAME="yourbucketname"

# Payment Configuration
STRIPE_SECRET_KEY="yourstripekey"
STRIPE_WEBHOOK_SECRET="yourwebhooksecret"
PAYPAL_CLIENT_ID="yourpaypalclientid"
PAYPAL_CLIENT_SECRET="yourpaypalsecret"
4️⃣ Run the Development Servers
bash
Copy code
# Run backend
cd backend
npm run dev

# Run frontend
cd ../frontend
npm run dev
☁️ Deployment
Frontend: Vercel / Netlify

Backend: Render / Railway / AWS EC2

Database: Supabase / PostgreSQL

Media Storage: AWS S3 / Cloudinary

📜 License
MIT License © 2025 Prisma Technology Solution / EduStack Project

🧑‍💻 Author
Thiha Naing (Leon)
Full-Stack Developer @ Prisma Technology Solution
📧 thihanaing1842003@gmail.com

yaml
Copy code

---

## 📚 Additional Documentation

- **[DATABASE.md](./DATABASE.md)** - Comprehensive database documentation with Sequelize models, migrations, and relationships
- **Database Setup** - Detailed instructions for PostgreSQL setup and Sequelize configuration
- **API Documentation** - Complete API endpoint documentation with examples
- **Deployment Guide** - Production deployment instructions for various platforms