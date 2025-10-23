// User Types
export interface User {
  id: number;
  name: string;
  email: string;
  roleId: number;
  profileImage?: string;
  phone?: string;
  bio?: string;
  isActive: boolean;
  emailVerified: boolean;
  createdAt: string;
  updatedAt: string;
  role?: Role;
}

export interface Role {
  id: number;
  name: string;
  description?: string;
}

// Course Types
export interface Course {
  id: number;
  title: string;
  description?: string;
  price: number;
  instructorId: number;
  categoryId: number;
  thumbnailUrl?: string;
  isPublished: boolean;
  difficultyLevel: 'beginner' | 'intermediate' | 'advanced';
  durationHours: number;
  language: string;
  createdAt: string;
  updatedAt: string;
  instructor?: User;
  category?: Category;
  courseSections?: CourseSection[];
  enrollments?: Enrollment[];
  reviews?: Review[];
}

export interface Category {
  id: number;
  name: string;
  description?: string;
  iconUrl?: string;
  createdAt: string;
  updatedAt: string;
}

export interface CourseSection {
  id: number;
  courseId: number;
  title: string;
  description?: string;
  orderIndex: number;
  createdAt: string;
  updatedAt: string;
  lessons?: Lesson[];
}

export interface Lesson {
  id: number;
  sectionId: number;
  title: string;
  description?: string;
  content?: string;
  videoUrl?: string;
  durationMinutes: number;
  orderIndex: number;
  isFree: boolean;
  createdAt: string;
  updatedAt: string;
  resources?: Resource[];
}

export interface Resource {
  id: number;
  lessonId: number;
  title: string;
  fileUrl: string;
  fileType: string;
  fileSize: number;
  createdAt: string;
}

// Enrollment Types
export interface Enrollment {
  id: number;
  studentId: number;
  courseId: number;
  enrollmentDate: string;
  progressPercentage: number;
  completionDate?: string;
  isActive: boolean;
  paymentStatus: 'pending' | 'paid' | 'failed' | 'refunded';
  student?: User;
  course?: Course;
}

export interface LessonProgress {
  id: number;
  studentId: number;
  lessonId: number;
  isCompleted: boolean;
  completedAt?: string;
  timeSpent: number;
  createdAt: string;
  updatedAt: string;
}

// Review Types
export interface Review {
  id: number;
  studentId: number;
  courseId: number;
  rating: number;
  comment?: string;
  createdAt: string;
  updatedAt: string;
  student?: User;
  course?: Course;
}

// Payment Types
export interface Payment {
  id: number;
  studentId: number;
  courseId: number;
  amount: number;
  paymentMethod: string;
  transactionId: string;
  status: 'pending' | 'completed' | 'failed' | 'refunded';
  paymentDate: string;
  createdAt: string;
  student?: User;
  course?: Course;
}

// Auth Types
export interface LoginRequest {
  email: string;
  password: string;
}

export interface RegisterRequest {
  name: string;
  email: string;
  password: string;
  confirmPassword: string;
}

export interface AuthResponse {
  token: string;
  user: User;
}

// API Response Types
export interface ApiResponse<T> {
  success: boolean;
  data?: T;
  message?: string;
  errors?: string[];
}

export interface PaginatedResponse<T> {
  data: T[];
  totalCount: number;
  pageNumber: number;
  pageSize: number;
  totalPages: number;
}

// Form Types
export interface CourseFormData {
  title: string;
  description: string;
  price: number;
  categoryId: number;
  difficultyLevel: 'beginner' | 'intermediate' | 'advanced';
  durationHours: number;
  language: string;
  thumbnailUrl?: string;
}

export interface UserProfileFormData {
  name: string;
  email: string;
  phone?: string;
  bio?: string;
  profileImage?: string;
}
