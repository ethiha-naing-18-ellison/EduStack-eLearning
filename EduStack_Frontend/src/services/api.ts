import axios, { AxiosInstance, AxiosResponse } from 'axios';
import { 
  User, 
  Course, 
  Enrollment, 
  Review, 
  Payment, 
  Category,
  LoginRequest, 
  RegisterRequest, 
  AuthResponse,
  ApiResponse,
  PaginatedResponse,
  CourseFormData,
  UserProfileFormData
} from '../types';

class ApiService {
  private api: AxiosInstance;

  constructor() {
    this.api = axios.create({
      baseURL: 'http://localhost:5000/api', // Backend API URL
      headers: {
        'Content-Type': 'application/json',
      },
    });

    // Add request interceptor to include auth token
    this.api.interceptors.request.use(
      (config) => {
        const token = localStorage.getItem('token');
        if (token) {
          config.headers.Authorization = `Bearer ${token}`;
        }
        return config;
      },
      (error) => {
        return Promise.reject(error);
      }
    );

    // Add response interceptor for error handling
    this.api.interceptors.response.use(
      (response) => response,
      (error) => {
        if (error.response?.status === 401) {
          localStorage.removeItem('token');
          localStorage.removeItem('user');
          window.location.href = '/login';
        }
        return Promise.reject(error);
      }
    );
  }

  // Auth endpoints
  async login(credentials: LoginRequest): Promise<AuthResponse> {
    const response: AxiosResponse<AuthResponse> = await this.api.post('/auth/login', credentials);
    return response.data;
  }

  async register(userData: RegisterRequest): Promise<AuthResponse> {
    const response: AxiosResponse<AuthResponse> = await this.api.post('/auth/register', userData);
    return response.data;
  }

  async getCurrentUser(): Promise<User> {
    const response: AxiosResponse<User> = await this.api.get('/auth/me');
    return response.data;
  }

  // User endpoints
  async getUsers(page: number = 1, pageSize: number = 10): Promise<PaginatedResponse<User>> {
    const response: AxiosResponse<PaginatedResponse<User>> = await this.api.get(`/users?page=${page}&pageSize=${pageSize}`);
    return response.data;
  }

  async getUserById(id: number): Promise<User> {
    const response: AxiosResponse<User> = await this.api.get(`/users/${id}`);
    return response.data;
  }

  async updateUser(id: number, userData: UserProfileFormData): Promise<User> {
    const response: AxiosResponse<User> = await this.api.put(`/users/${id}`, userData);
    return response.data;
  }

  async deleteUser(id: number): Promise<void> {
    await this.api.delete(`/users/${id}`);
  }

  // Course endpoints
  async getCourses(page: number = 1, pageSize: number = 10, categoryId?: number): Promise<PaginatedResponse<Course>> {
    let url = `/courses?page=${page}&pageSize=${pageSize}`;
    if (categoryId) {
      url += `&categoryId=${categoryId}`;
    }
    const response: AxiosResponse<PaginatedResponse<Course>> = await this.api.get(url);
    return response.data;
  }

  async getCourseById(id: number): Promise<Course> {
    const response: AxiosResponse<Course> = await this.api.get(`/courses/${id}`);
    return response.data;
  }

  async createCourse(courseData: CourseFormData): Promise<Course> {
    const response: AxiosResponse<Course> = await this.api.post('/courses', courseData);
    return response.data;
  }

  async updateCourse(id: number, courseData: CourseFormData): Promise<Course> {
    const response: AxiosResponse<Course> = await this.api.put(`/courses/${id}`, courseData);
    return response.data;
  }

  async deleteCourse(id: number): Promise<void> {
    await this.api.delete(`/courses/${id}`);
  }

  async getCoursesByInstructor(instructorId: number): Promise<Course[]> {
    const response: AxiosResponse<Course[]> = await this.api.get(`/courses/instructor/${instructorId}`);
    return response.data;
  }

  // Category endpoints
  async getCategories(): Promise<Category[]> {
    const response: AxiosResponse<Category[]> = await this.api.get('/categories');
    return response.data;
  }

  // Enrollment endpoints
  async getEnrollments(page: number = 1, pageSize: number = 10): Promise<PaginatedResponse<Enrollment>> {
    const response: AxiosResponse<PaginatedResponse<Enrollment>> = await this.api.get(`/enrollments?page=${page}&pageSize=${pageSize}`);
    return response.data;
  }

  async getEnrollmentsByStudent(studentId: number): Promise<Enrollment[]> {
    const response: AxiosResponse<Enrollment[]> = await this.api.get(`/enrollments/student/${studentId}`);
    return response.data;
  }

  async enrollInCourse(courseId: number): Promise<Enrollment> {
    const response: AxiosResponse<Enrollment> = await this.api.post('/enrollments', { courseId });
    return response.data;
  }

  async updateEnrollmentProgress(enrollmentId: number, progressPercentage: number): Promise<Enrollment> {
    const response: AxiosResponse<Enrollment> = await this.api.put(`/enrollments/${enrollmentId}/progress`, { progressPercentage });
    return response.data;
  }

  // Review endpoints
  async getReviewsByCourse(courseId: number): Promise<Review[]> {
    const response: AxiosResponse<Review[]> = await this.api.get(`/reviews/course/${courseId}`);
    return response.data;
  }

  async createReview(reviewData: { courseId: number; rating: number; comment?: string }): Promise<Review> {
    const response: AxiosResponse<Review> = await this.api.post('/reviews', reviewData);
    return response.data;
  }

  async updateReview(id: number, reviewData: { rating: number; comment?: string }): Promise<Review> {
    const response: AxiosResponse<Review> = await this.api.put(`/reviews/${id}`, reviewData);
    return response.data;
  }

  async deleteReview(id: number): Promise<void> {
    await this.api.delete(`/reviews/${id}`);
  }

  // Payment endpoints
  async getPayments(page: number = 1, pageSize: number = 10): Promise<PaginatedResponse<Payment>> {
    const response: AxiosResponse<PaginatedResponse<Payment>> = await this.api.get(`/payments?page=${page}&pageSize=${pageSize}`);
    return response.data;
  }

  async createPayment(paymentData: { courseId: number; amount: number; paymentMethod: string }): Promise<Payment> {
    const response: AxiosResponse<Payment> = await this.api.post('/payments', paymentData);
    return response.data;
  }

  // Admin endpoints
  async getAdminStats(): Promise<any> {
    const response: AxiosResponse<any> = await this.api.get('/admin/stats');
    return response.data;
  }

  async getAdminUsers(page: number = 1, pageSize: number = 10): Promise<PaginatedResponse<User>> {
    const response: AxiosResponse<PaginatedResponse<User>> = await this.api.get(`/admin/users?page=${page}&pageSize=${pageSize}`);
    return response.data;
  }

  async getAdminCourses(page: number = 1, pageSize: number = 10): Promise<PaginatedResponse<Course>> {
    const response: AxiosResponse<PaginatedResponse<Course>> = await this.api.get(`/admin/courses?page=${page}&pageSize=${pageSize}`);
    return response.data;
  }
}

export const apiService = new ApiService();
export default apiService;
