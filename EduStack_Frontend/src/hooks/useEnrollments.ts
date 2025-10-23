import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { apiService } from '../services/api';
import { Enrollment, PaginatedResponse } from '../types';

export const useEnrollments = (page: number = 1, pageSize: number = 10) => {
  return useQuery({
    queryKey: ['enrollments', page, pageSize],
    queryFn: () => apiService.getEnrollments(page, pageSize),
  });
};

export const useStudentEnrollments = (studentId: number) => {
  return useQuery({
    queryKey: ['enrollments', 'student', studentId],
    queryFn: () => apiService.getEnrollmentsByStudent(studentId),
    enabled: !!studentId,
  });
};

export const useEnrollInCourse = () => {
  const queryClient = useQueryClient();
  
  return useMutation({
    mutationFn: (courseId: number) => apiService.enrollInCourse(courseId),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['enrollments'] });
      queryClient.invalidateQueries({ queryKey: ['courses'] });
    },
  });
};

export const useUpdateEnrollmentProgress = () => {
  const queryClient = useQueryClient();
  
  return useMutation({
    mutationFn: ({ enrollmentId, progressPercentage }: { enrollmentId: number; progressPercentage: number }) => 
      apiService.updateEnrollmentProgress(enrollmentId, progressPercentage),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['enrollments'] });
    },
  });
};
