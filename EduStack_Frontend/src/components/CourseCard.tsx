import React from 'react';
import { Link } from 'react-router-dom';
import { Clock, Star, Users, DollarSign } from 'lucide-react';
import { Course } from '../types';
import { formatCurrency, formatDuration, formatRating, getDifficultyColor } from '../utils/format';

interface CourseCardProps {
  course: Course;
  showInstructor?: boolean;
}

const CourseCard: React.FC<CourseCardProps> = ({ course, showInstructor = true }) => {
  const averageRating = course.reviews?.length 
    ? course.reviews.reduce((sum, review) => sum + review.rating, 0) / course.reviews.length 
    : 0;

  const totalStudents = course.enrollments?.length || 0;

  return (
    <div className="bg-white rounded-lg shadow-sm border border-gray-200 overflow-hidden hover:shadow-md transition-shadow duration-200">
      {/* Course Thumbnail */}
      <div className="aspect-video bg-gray-200 relative">
        {course.thumbnailUrl ? (
          <img
            src={course.thumbnailUrl}
            alt={course.title}
            className="w-full h-full object-cover"
          />
        ) : (
          <div className="w-full h-full flex items-center justify-center bg-gradient-to-br from-primary-100 to-primary-200">
            <span className="text-primary-600 font-semibold text-lg">
              {course.title.charAt(0).toUpperCase()}
            </span>
          </div>
        )}
        
        {/* Difficulty Badge */}
        <div className="absolute top-2 right-2">
          <span className={`px-2 py-1 rounded-full text-xs font-medium ${getDifficultyColor(course.difficultyLevel)}`}>
            {course.difficultyLevel}
          </span>
        </div>
      </div>

      {/* Course Content */}
      <div className="p-4">
        {/* Category */}
        {course.category && (
          <span className="text-xs text-primary-600 font-medium">
            {course.category.name}
          </span>
        )}

        {/* Course Title */}
        <h3 className="text-lg font-semibold text-gray-900 mt-1 line-clamp-2">
          {course.title}
        </h3>

        {/* Course Description */}
        {course.description && (
          <p className="text-sm text-gray-600 mt-2 line-clamp-2">
            {course.description}
          </p>
        )}

        {/* Instructor */}
        {showInstructor && course.instructor && (
          <div className="flex items-center mt-3">
            <div className="h-6 w-6 rounded-full bg-gray-200 flex items-center justify-center">
              <span className="text-xs font-medium text-gray-600">
                {course.instructor.name.charAt(0).toUpperCase()}
              </span>
            </div>
            <span className="text-sm text-gray-600 ml-2">
              {course.instructor.name}
            </span>
          </div>
        )}

        {/* Course Stats */}
        <div className="flex items-center justify-between mt-4">
          <div className="flex items-center space-x-4 text-sm text-gray-500">
            <div className="flex items-center space-x-1">
              <Clock className="h-4 w-4" />
              <span>{formatDuration(course.durationHours)}</span>
            </div>
            <div className="flex items-center space-x-1">
              <Users className="h-4 w-4" />
              <span>{totalStudents}</span>
            </div>
          </div>

          {/* Rating */}
          {averageRating > 0 && (
            <div className="flex items-center space-x-1">
              <Star className="h-4 w-4 text-yellow-400 fill-current" />
              <span className="text-sm font-medium text-gray-700">
                {formatRating(averageRating)}
              </span>
            </div>
          )}
        </div>

        {/* Price and Action */}
        <div className="flex items-center justify-between mt-4 pt-4 border-t border-gray-100">
          <div className="flex items-center space-x-2">
            <DollarSign className="h-4 w-4 text-green-600" />
            <span className="text-lg font-bold text-gray-900">
              {formatCurrency(course.price)}
            </span>
          </div>

          <Link
            to={`/courses/${course.id}`}
            className="bg-primary-600 text-white px-4 py-2 rounded-md text-sm font-medium hover:bg-primary-700 transition-colors"
          >
            View Course
          </Link>
        </div>
      </div>
    </div>
  );
};

export default CourseCard;
