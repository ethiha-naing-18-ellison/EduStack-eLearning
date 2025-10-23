// Email validation
export const isValidEmail = (email: string): boolean => {
  const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
  return emailRegex.test(email);
};

// Password validation
export const isValidPassword = (password: string): boolean => {
  // At least 8 characters, 1 uppercase, 1 lowercase, 1 number
  const passwordRegex = /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)[a-zA-Z\d@$!%*?&]{8,}$/;
  return passwordRegex.test(password);
};

// Phone validation
export const isValidPhone = (phone: string): boolean => {
  const phoneRegex = /^[\+]?[1-9][\d]{0,15}$/;
  return phoneRegex.test(phone.replace(/\s/g, ''));
};

// Name validation
export const isValidName = (name: string): boolean => {
  return name.trim().length >= 2 && name.trim().length <= 50;
};

// Course title validation
export const isValidCourseTitle = (title: string): boolean => {
  return title.trim().length >= 5 && title.trim().length <= 100;
};

// Course description validation
export const isValidCourseDescription = (description: string): boolean => {
  return description.trim().length >= 10 && description.trim().length <= 1000;
};

// Price validation
export const isValidPrice = (price: number): boolean => {
  return price >= 0 && price <= 9999.99;
};

// Duration validation
export const isValidDuration = (hours: number): boolean => {
  return hours > 0 && hours <= 1000;
};

// Rating validation
export const isValidRating = (rating: number): boolean => {
  return rating >= 1 && rating <= 5;
};

// Form validation helpers
export const getValidationError = (field: string, value: any): string | null => {
  switch (field) {
    case 'email':
      if (!value) return 'Email is required';
      if (!isValidEmail(value)) return 'Please enter a valid email address';
      return null;
    
    case 'password':
      if (!value) return 'Password is required';
      if (value.length < 8) return 'Password must be at least 8 characters';
      if (!isValidPassword(value)) return 'Password must contain at least one uppercase letter, one lowercase letter, and one number';
      return null;
    
    case 'confirmPassword':
      if (!value) return 'Please confirm your password';
      return null;
    
    case 'name':
      if (!value) return 'Name is required';
      if (!isValidName(value)) return 'Name must be between 2 and 50 characters';
      return null;
    
    case 'phone':
      if (value && !isValidPhone(value)) return 'Please enter a valid phone number';
      return null;
    
    case 'title':
      if (!value) return 'Title is required';
      if (!isValidCourseTitle(value)) return 'Title must be between 5 and 100 characters';
      return null;
    
    case 'description':
      if (!value) return 'Description is required';
      if (!isValidCourseDescription(value)) return 'Description must be between 10 and 1000 characters';
      return null;
    
    case 'price':
      if (value < 0) return 'Price cannot be negative';
      if (!isValidPrice(value)) return 'Price must be between $0 and $9999.99';
      return null;
    
    case 'durationHours':
      if (!value) return 'Duration is required';
      if (!isValidDuration(value)) return 'Duration must be between 1 and 1000 hours';
      return null;
    
    case 'rating':
      if (!isValidRating(value)) return 'Rating must be between 1 and 5';
      return null;
    
    default:
      return null;
  }
};

// Check if passwords match
export const doPasswordsMatch = (password: string, confirmPassword: string): boolean => {
  return password === confirmPassword;
};
