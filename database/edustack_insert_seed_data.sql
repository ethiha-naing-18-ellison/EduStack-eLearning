-- EduStack: Sample Data Insertion Script
-- EduStack E-Learning Platform - Initial Sample Data
-- Compatible with pgAdmin Query Tool

-- =====================================================
-- 1. INSERT DEFAULT ROLES
-- =====================================================
INSERT INTO roles (name, description, permissions) VALUES 
('Admin', 'System administrator with full access', '{"users": ["create", "read", "update", "delete"], "courses": ["create", "read", "update", "delete"], "instructors": ["approve", "reject", "manage"]}'),
('Instructor', 'Course instructor with teaching permissions', '{"courses": ["create", "read", "update", "delete"], "students": ["read"], "lessons": ["create", "read", "update", "delete"]}'),
('Student', 'Regular student with learning permissions', '{"courses": ["read"], "enrollments": ["create", "read"], "lessons": ["read"]}');

-- =====================================================
-- 2. INSERT SAMPLE USERS
-- =====================================================
-- Note: Password hashes are for 'password123' - change in production
INSERT INTO users (name, email, password_hash, role_id, is_active, email_verified) VALUES 
('System Administrator', 'admin@edustack.com', '$2b$10$92IXUNpkjO0rOQ5byMi.Ye4oKoEa3Ro9llC/.og/at2.uheWG/igi', 1, true, true),
('John Instructor', 'instructor@edustack.com', '$2b$10$92IXUNpkjO0rOQ5byMi.Ye4oKoEa3Ro9llC/.og/at2.uheWG/igi', 2, true, true),
('Jane Student', 'student@edustack.com', '$2b$10$92IXUNpkjO0rOQ5byMi.Ye4oKoEa3Ro9llC/.og/at2.uheWG/igi', 3, true, true),
('Sarah Wilson', 'sarah@edustack.com', '$2b$10$92IXUNpkjO0rOQ5byMi.Ye4oKoEa3Ro9llC/.og/at2.uheWG/igi', 3, true, true),
('Mike Chen', 'mike@edustack.com', '$2b$10$92IXUNpkjO0rOQ5byMi.Ye4oKoEa3Ro9llC/.og/at2.uheWG/igi', 2, true, true);

-- =====================================================
-- 3. INSERT CATEGORIES
-- =====================================================
INSERT INTO categories (name, description, is_active) VALUES 
('Programming', 'Software development and coding courses', true),
('Design', 'UI/UX design and graphic design courses', true),
('Business', 'Business management and entrepreneurship courses', true),
('Marketing', 'Digital marketing and advertising courses', true),
('Data Science', 'Data analysis and machine learning courses', true),
('Web Development', 'Frontend and backend web development', true),
('Mobile Development', 'iOS and Android app development', true),
('DevOps', 'Development operations and deployment', true);

-- =====================================================
-- 4. INSERT SAMPLE COURSES
-- =====================================================
INSERT INTO courses (title, description, price, instructor_id, category_id, is_published, difficulty_level, duration_hours, language) VALUES 
('Complete Web Development Course', 'Learn full-stack web development from scratch using modern technologies including HTML, CSS, JavaScript, React, Node.js, and databases.', 99.99, 2, 6, true, 'beginner', 40, 'en'),
('Advanced React Development', 'Master React.js with hooks, context, state management, and advanced patterns for building scalable applications.', 149.99, 2, 6, true, 'intermediate', 25, 'en'),
('Python for Data Science', 'Learn data analysis, visualization, and machine learning with Python using pandas, numpy, matplotlib, and scikit-learn.', 199.99, 5, 5, true, 'beginner', 35, 'en'),
('UI/UX Design Fundamentals', 'Master the principles of user interface and user experience design with hands-on projects.', 89.99, 5, 2, true, 'beginner', 20, 'en'),
('Digital Marketing Mastery', 'Complete guide to digital marketing including SEO, social media, email marketing, and analytics.', 79.99, 5, 4, true, 'beginner', 30, 'en');

-- =====================================================
-- 5. INSERT COURSE SECTIONS
-- =====================================================
-- Course 1: Complete Web Development Course
INSERT INTO course_sections (course_id, title, description, order_index, is_published) VALUES 
(1, 'Introduction to Web Development', 'Get started with web development fundamentals', 1, true),
(1, 'HTML Fundamentals', 'Learn HTML structure and semantic markup', 2, true),
(1, 'CSS Styling', 'Master CSS for beautiful and responsive designs', 3, true),
(1, 'JavaScript Basics', 'Learn JavaScript programming fundamentals', 4, true),
(1, 'React Development', 'Build dynamic user interfaces with React', 5, true),
(1, 'Backend Development', 'Server-side development with Node.js', 6, true),
(1, 'Database Integration', 'Connect your app to databases', 7, true),
(1, 'Final Project', 'Build a complete web application', 8, true);

-- Course 2: Advanced React Development
INSERT INTO course_sections (course_id, title, description, order_index, is_published) VALUES 
(2, 'React Hooks Deep Dive', 'Master React hooks and custom hooks', 1, true),
(2, 'State Management', 'Redux, Context API, and state management patterns', 2, true),
(2, 'Performance Optimization', 'Optimize React applications for better performance', 3, true),
(2, 'Testing React Apps', 'Unit testing and integration testing', 4, true);

-- Course 3: Python for Data Science
INSERT INTO course_sections (course_id, title, description, order_index, is_published) VALUES 
(3, 'Python Basics for Data Science', 'Python fundamentals for data analysis', 1, true),
(3, 'Data Manipulation with Pandas', 'Working with data using pandas library', 2, true),
(3, 'Data Visualization', 'Creating charts and graphs with matplotlib and seaborn', 3, true),
(3, 'Machine Learning Introduction', 'Basic machine learning concepts and implementation', 4, true);

-- =====================================================
-- 6. INSERT LESSONS
-- =====================================================
-- Course 1, Section 1: Introduction to Web Development
INSERT INTO lessons (section_id, title, description, lesson_type, content, order_index, is_published, duration_minutes, is_preview) VALUES 
(1, 'What is Web Development?', 'Introduction to web development concepts and career opportunities', 'video', 'Learn about the web development industry and career paths', 1, true, 15, true),
(1, 'Setting up Development Environment', 'Install and configure VS Code, Git, and browser developer tools', 'video', 'Step-by-step guide to setting up your development environment', 2, true, 20, false),
(1, 'Understanding the Web', 'How the internet works, HTTP, and web servers', 'text', 'Deep dive into how the web functions behind the scenes', 3, true, 10, true);

-- Course 1, Section 2: HTML Fundamentals
INSERT INTO lessons (section_id, title, description, lesson_type, content, order_index, is_published, duration_minutes, is_preview) VALUES 
(2, 'HTML Document Structure', 'Learn about DOCTYPE, html, head, and body elements', 'video', 'Understanding the basic structure of HTML documents', 1, true, 25, true),
(2, 'HTML Elements and Tags', 'Common HTML elements: headings, paragraphs, lists, links', 'video', 'Master the essential HTML elements for content structure', 2, true, 30, false),
(2, 'HTML Forms', 'Create forms with input fields, buttons, and validation', 'video', 'Build interactive forms for user input', 3, true, 35, false),
(2, 'HTML Semantic Elements', 'Use semantic HTML for better accessibility and SEO', 'video', 'Learn about header, nav, main, section, article, and footer', 4, true, 20, false);

-- Course 1, Section 3: CSS Styling
INSERT INTO lessons (section_id, title, description, lesson_type, content, order_index, is_published, duration_minutes, is_preview) VALUES 
(3, 'CSS Basics and Selectors', 'Introduction to CSS syntax and selectors', 'video', 'Learn how to style HTML elements with CSS', 1, true, 25, true),
(3, 'CSS Layout with Flexbox', 'Create flexible layouts using CSS Flexbox', 'video', 'Master Flexbox for modern web layouts', 2, true, 40, false),
(3, 'CSS Grid Layout', 'Advanced layout techniques with CSS Grid', 'video', 'Create complex layouts with CSS Grid', 3, true, 35, false),
(3, 'Responsive Design', 'Make your websites work on all devices', 'video', 'Learn responsive design principles and media queries', 4, true, 30, false);

-- Course 2, Section 1: React Hooks Deep Dive
INSERT INTO lessons (section_id, title, description, lesson_type, content, order_index, is_published, duration_minutes, is_preview) VALUES 
(5, 'useState Hook', 'Managing component state with useState', 'video', 'Learn the fundamentals of state management in React', 1, true, 20, true),
(5, 'useEffect Hook', 'Side effects and lifecycle management', 'video', 'Handle side effects and component lifecycle with useEffect', 2, true, 25, false),
(5, 'Custom Hooks', 'Creating reusable logic with custom hooks', 'video', 'Build your own custom hooks for code reusability', 3, true, 30, false);

-- Course 3, Section 1: Python Basics for Data Science
INSERT INTO lessons (section_id, title, description, lesson_type, content, order_index, is_published, duration_minutes, is_preview) VALUES 
(9, 'Python Environment Setup', 'Setting up Python and Jupyter Notebooks', 'video', 'Install Python and essential data science libraries', 1, true, 15, true),
(9, 'Python Data Types', 'Understanding Python data types for data science', 'video', 'Learn about lists, dictionaries, and data structures', 2, true, 20, false),
(9, 'NumPy Fundamentals', 'Working with arrays using NumPy', 'video', 'Master NumPy for numerical computing', 3, true, 25, false);

-- =====================================================
-- 7. INSERT RESOURCES
-- =====================================================
INSERT INTO resources (lesson_id, file_name, file_url, file_type, file_size) VALUES 
(1, 'web-development-overview.pdf', 'https://example.com/resources/web-dev-overview.pdf', 'pdf', 1024000),
(2, 'development-setup-guide.pdf', 'https://example.com/resources/setup-guide.pdf', 'pdf', 2048000),
(4, 'html-cheatsheet.pdf', 'https://example.com/resources/html-cheatsheet.pdf', 'pdf', 512000),
(5, 'html-elements-reference.pdf', 'https://example.com/resources/html-elements.pdf', 'pdf', 768000),
(7, 'css-selectors-guide.pdf', 'https://example.com/resources/css-selectors.pdf', 'pdf', 640000),
(8, 'flexbox-complete-guide.pdf', 'https://example.com/resources/flexbox-guide.pdf', 'pdf', 1536000),
(10, 'react-hooks-reference.pdf', 'https://example.com/resources/react-hooks.pdf', 'pdf', 896000),
(12, 'python-data-science-setup.pdf', 'https://example.com/resources/python-setup.pdf', 'pdf', 1280000);

-- =====================================================
-- 8. INSERT ENROLLMENTS
-- =====================================================
INSERT INTO enrollments (student_id, course_id, progress_percentage, payment_status, is_active) VALUES 
(3, 1, 25.50, 'completed', true),
(3, 2, 0.00, 'pending', true),
(4, 1, 15.25, 'completed', true),
(4, 3, 0.00, 'pending', true),
(3, 3, 10.00, 'completed', true);

-- =====================================================
-- 9. INSERT LESSON PROGRESS
-- =====================================================
INSERT INTO lesson_progress (student_id, lesson_id, is_completed, time_spent_minutes) VALUES 
(3, 1, true, 15),
(3, 2, true, 20),
(3, 4, true, 25),
(3, 5, false, 10),
(4, 1, true, 15),
(4, 2, false, 5),
(4, 4, true, 25);

-- =====================================================
-- 10. INSERT PAYMENTS
-- =====================================================
INSERT INTO payments (user_id, course_id, amount, payment_method, payment_status, transaction_id, gateway_response) VALUES 
(3, 1, 99.99, 'stripe', 'completed', 'stripe_txn_123456789', '{"stripe_payment_intent": "pi_123456789", "status": "succeeded"}'),
(4, 1, 99.99, 'paypal', 'completed', 'paypal_txn_987654321', '{"paypal_order_id": "order_987654321", "status": "approved"}'),
(3, 3, 199.99, 'stripe', 'completed', 'stripe_txn_456789123', '{"stripe_payment_intent": "pi_456789123", "status": "succeeded"}');

-- =====================================================
-- 11. INSERT REVIEWS
-- =====================================================
INSERT INTO reviews (student_id, course_id, rating, comment, is_approved) VALUES 
(3, 1, 5, 'Excellent course! Very well structured and easy to follow. The instructor explains concepts clearly.', true),
(4, 1, 4, 'Great course for beginners. The hands-on projects really help solidify the concepts.', true),
(3, 3, 5, 'Amazing data science course! The Python examples are practical and relevant.', true);

-- =====================================================
-- 12. INSERT INSTRUCTOR APPLICATIONS
-- =====================================================
INSERT INTO instructor_applications (user_id, application_status, qualifications, experience_years, portfolio_url, motivation, admin_remarks, reviewed_by, reviewed_at) VALUES 
(5, 'approved', 'Masters in Computer Science, 5 years industry experience', 5, 'https://mikechen.dev/portfolio', 'Passionate about teaching web development and helping students succeed', 'Approved - Excellent qualifications and portfolio', 1, CURRENT_TIMESTAMP);

-- =====================================================
-- VERIFICATION QUERIES
-- =====================================================
-- Display counts of inserted data
SELECT 'Data Insertion Summary:' as info;
SELECT 'Roles' as table_name, COUNT(*) as count FROM roles
UNION ALL
SELECT 'Users', COUNT(*) FROM users
UNION ALL
SELECT 'Categories', COUNT(*) FROM categories
UNION ALL
SELECT 'Courses', COUNT(*) FROM courses
UNION ALL
SELECT 'Course Sections', COUNT(*) FROM course_sections
UNION ALL
SELECT 'Lessons', COUNT(*) FROM lessons
UNION ALL
SELECT 'Resources', COUNT(*) FROM resources
UNION ALL
SELECT 'Enrollments', COUNT(*) FROM enrollments
UNION ALL
SELECT 'Lesson Progress', COUNT(*) FROM lesson_progress
UNION ALL
SELECT 'Payments', COUNT(*) FROM payments
UNION ALL
SELECT 'Reviews', COUNT(*) FROM reviews
UNION ALL
SELECT 'Instructor Applications', COUNT(*) FROM instructor_applications;

-- =====================================================
-- SAMPLE DATA INSERTION COMPLETE
-- =====================================================
-- All tables have been populated with sample data
-- Ready for testing EduStack E-Learning Platform
-- =====================================================
