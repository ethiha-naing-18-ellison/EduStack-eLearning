import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { apiService } from '../services/api';
import { Course, CourseFormData, PaginatedResponse } from '../types';

export const useCourses = (page: number = 1, pageSize: number = 10, categoryId?: number) => {
  return useQuery({
    queryKey: ['courses', page, pageSize, categoryId],
    queryFn: () => apiService.getCourses(page, pageSize, categoryId),
  });
};

export const useCourse = (id: number) => {
  return useQuery({
    queryKey: ['course', id],
    queryFn: () => apiService.getCourseById(id),
    enabled: !!id,
  });
};

export const useInstructorCourses = (instructorId: number) => {
  return useQuery({
    queryKey: ['courses', 'instructor', instructorId],
    queryFn: () => apiService.getCoursesByInstructor(instructorId),
    enabled: !!instructorId,
  });
};

export const useCreateCourse = () => {
  const queryClient = useQueryClient();
  
  return useMutation({
    mutationFn: (courseData: CourseFormData) => apiService.createCourse(courseData),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['courses'] });
    },
  });
};

export const useUpdateCourse = () => {
  const queryClient = useQueryClient();
  
  return useMutation({
    mutationFn: ({ id, courseData }: { id: number; courseData: CourseFormData }) => 
      apiService.updateCourse(id, courseData),
    onSuccess: (_, { id }) => {
      queryClient.invalidateQueries({ queryKey: ['courses'] });
      queryClient.invalidateQueries({ queryKey: ['course', id] });
    },
  });
};

export const useDeleteCourse = () => {
  const queryClient = useQueryClient();
  
  return useMutation({
    mutationFn: (id: number) => apiService.deleteCourse(id),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['courses'] });
    },
  });
};
