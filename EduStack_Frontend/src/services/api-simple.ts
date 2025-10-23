// Simple API service without complex imports for testing
export const apiService = {
  // Mock methods for testing
  getCourses: () => Promise.resolve({ data: [], totalCount: 0, pageNumber: 1, pageSize: 10, totalPages: 0 }),
  getCategories: () => Promise.resolve([]),
  getCurrentUser: () => Promise.resolve({ id: 1, name: 'Test User', email: 'test@example.com', roleId: 1, isActive: true, emailVerified: true, createdAt: new Date().toISOString(), updatedAt: new Date().toISOString() }),
  login: () => Promise.resolve({ token: 'mock-token', user: { id: 1, name: 'Test User', email: 'test@example.com', roleId: 1, isActive: true, emailVerified: true, createdAt: new Date().toISOString(), updatedAt: new Date().toISOString() } }),
  register: () => Promise.resolve({ token: 'mock-token', user: { id: 1, name: 'Test User', email: 'test@example.com', roleId: 1, isActive: true, emailVerified: true, createdAt: new Date().toISOString(), updatedAt: new Date().toISOString() } }),
};
