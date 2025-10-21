# EduStack Backend API

A comprehensive C# .NET 8 Web API for the EduStack e-learning platform.

## 🚀 Features

- **Authentication & Authorization**: JWT-based authentication with role-based access control
- **User Management**: Student, Instructor, and Admin roles with profile management
- **Course Management**: Create, update, and manage courses with sections and lessons
- **Enrollment System**: Student enrollment with progress tracking
- **Payment Processing**: Integrated payment system with multiple gateways
- **Review System**: Course reviews with admin approval
- **Resource Management**: File uploads and resource management
- **Admin Dashboard**: Comprehensive admin panel with analytics

## 🛠️ Tech Stack

- **.NET 8**: Latest .NET framework
- **Entity Framework Core**: ORM for database operations
- **PostgreSQL**: Primary database
- **JWT Authentication**: Secure token-based authentication
- **AutoMapper**: Object mapping
- **FluentValidation**: Input validation
- **Serilog**: Structured logging
- **Swagger**: API documentation

## 📁 Project Structure

```
EduStack_Backend/
├── Controllers/           # API Controllers
│   ├── AuthController.cs
│   ├── CoursesController.cs
│   ├── EnrollmentsController.cs
│   ├── ReviewsController.cs
│   ├── UsersController.cs
│   ├── PaymentsController.cs
│   └── AdminController.cs
├── Data/                 # Database Context
│   └── EduStackDbContext.cs
├── DTOs/                 # Data Transfer Objects
│   ├── AuthDTOs.cs
│   ├── UserDTOs.cs
│   ├── CourseDTOs.cs
│   ├── EnrollmentDTOs.cs
│   ├── PaymentDTOs.cs
│   └── ReviewDTOs.cs
├── Extensions/           # Extension Methods
│   └── JwtExtensions.cs
├── Middleware/           # Custom Middleware
│   └── ExceptionHandlingMiddleware.cs
├── Models/               # Entity Models
│   ├── Role.cs
│   ├── User.cs
│   ├── Category.cs
│   ├── Course.cs
│   ├── CourseSection.cs
│   ├── Lesson.cs
│   ├── Resource.cs
│   ├── Enrollment.cs
│   ├── LessonProgress.cs
│   ├── Payment.cs
│   ├── Review.cs
│   └── InstructorApplication.cs
├── Services/             # Business Logic Services
│   ├── IAuthService.cs
│   ├── AuthService.cs
│   ├── IUserService.cs
│   ├── UserService.cs
│   ├── ICourseService.cs
│   ├── CourseService.cs
│   ├── IEnrollmentService.cs
│   ├── EnrollmentService.cs
│   ├── IPaymentService.cs
│   ├── PaymentService.cs
│   ├── IReviewService.cs
│   ├── ReviewService.cs
│   ├── ILessonService.cs
│   ├── LessonService.cs
│   ├── IResourceService.cs
│   ├── ResourceService.cs
│   ├── IAdminService.cs
│   └── AdminService.cs
├── Program.cs            # Application Entry Point
├── appsettings.json      # Configuration
├── appsettings.Development.json
└── EduStack.API.csproj  # Project File
```

## 🔧 Setup Instructions

### Prerequisites

- .NET 8 SDK
- PostgreSQL 12+
- Visual Studio 2022 or VS Code

### Installation

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd EduStack_Backend
   ```

2. **Install dependencies**
   ```bash
   dotnet restore
   ```

3. **Configure database connection**
   Update `appsettings.json` with your PostgreSQL connection string:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Host=localhost;Port=5432;Database=edustack;Username=postgres;Password=th1234"
     }
   }
   ```

4. **Run database migrations**
   ```bash
   dotnet ef database update
   ```

5. **Run the application**
   ```bash
   dotnet run
   ```

The API will be available at `https://localhost:7000` (HTTPS) or `http://localhost:5000` (HTTP).

## 📚 API Documentation

### Authentication Endpoints

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| POST | `/api/auth/register` | Register new user | No |
| POST | `/api/auth/login` | User login | No |
| POST | `/api/auth/refresh` | Refresh JWT token | No |
| POST | `/api/auth/logout` | User logout | Yes |
| POST | `/api/auth/change-password` | Change password | Yes |

### Course Endpoints

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| GET | `/api/courses` | Get all published courses | No |
| GET | `/api/courses/{id}` | Get course details | No |
| POST | `/api/courses` | Create course | Instructor/Admin |
| PUT | `/api/courses/{id}` | Update course | Instructor/Admin |
| DELETE | `/api/courses/{id}` | Delete course | Instructor/Admin |

### Enrollment Endpoints

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| POST | `/api/enrollments` | Enroll in course | Student |
| GET | `/api/enrollments/my-enrollments` | Get user enrollments | Student |
| GET | `/api/enrollments/course/{id}/progress` | Get course progress | Student |
| PUT | `/api/enrollments/lessons/progress` | Update lesson progress | Student |

### User Endpoints

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| GET | `/api/users/profile` | Get user profile | Yes |
| PUT | `/api/users/profile` | Update user profile | Yes |
| POST | `/api/users/apply-instructor` | Apply for instructor role | Student |

### Admin Endpoints

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| GET | `/api/admin/dashboard` | Get dashboard stats | Admin |
| GET | `/api/admin/users` | Get all users | Admin |
| GET | `/api/admin/instructor-applications` | Get instructor applications | Admin |
| PUT | `/api/admin/instructor-applications/{id}/review` | Review application | Admin |

## 🔐 Authentication

The API uses JWT (JSON Web Tokens) for authentication. Include the token in the Authorization header:

```
Authorization: Bearer <your-jwt-token>
```

### Roles

- **Student**: Can enroll in courses, track progress, leave reviews
- **Instructor**: Can create and manage courses, view student progress
- **Admin**: Full system access, user management, analytics

## 🗄️ Database Schema

The API uses PostgreSQL with the following main entities:

- **Users**: Student, Instructor, Admin roles
- **Courses**: Course information with sections and lessons
- **Enrollments**: Student course enrollments
- **Payments**: Payment transactions
- **Reviews**: Course reviews and ratings
- **Resources**: Course materials and files

## 🚀 Deployment

### Production Configuration

1. **Update connection strings** for production database
2. **Configure JWT secrets** for production
3. **Set up logging** for production environment
4. **Configure CORS** for your frontend domain

### Environment Variables

```bash
# Database
DB_HOST=your-db-host
DB_PORT=5432
DB_NAME=edustack
DB_USER=your-username
DB_PASSWORD=your-password

# JWT
JWT_SECRET=your-secret-key
JWT_ISSUER=your-issuer
JWT_AUDIENCE=your-audience

# Logging
LOG_LEVEL=Information
```

## 📝 API Testing

Use Swagger UI for testing the API:
- Development: `https://localhost:7000/swagger`
- Production: `https://your-domain.com/swagger`

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Add tests if applicable
5. Submit a pull request

## 📄 License

This project is licensed under the MIT License.

## 🆘 Support

For support and questions, please contact the development team or create an issue in the repository.
