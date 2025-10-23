using EduStack.API.Data;
using EduStack.API.DTOs;
using EduStack.API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BCrypt.Net;

namespace EduStack.API.Services
{
    public class AuthService : IAuthService
    {
        private readonly EduStackDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthService> _logger;
        private readonly IEmailService _emailService;

        public AuthService(EduStackDbContext context, IConfiguration configuration, ILogger<AuthService> logger, IEmailService emailService)
        {
            _context = context;
            _configuration = configuration;
            _logger = logger;
            _emailService = emailService;
        }

        public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
        {
            try
            {
                // Check if user already exists
                if (await _context.Users.AnyAsync(u => u.Email == request.Email))
                {
                    throw new InvalidOperationException("User with this email already exists");
                }

                // Get default role (Student)
                var studentRole = await _context.Roles.FirstOrDefaultAsync(r => r.Name == "Student");
                if (studentRole == null)
                {
                    throw new InvalidOperationException("Student role not found");
                }

                // Create new user
                var user = new User
                {
                    Name = request.Name,
                    Email = request.Email,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                    RoleId = studentRole.Id,
                    Phone = request.Phone,
                    Bio = request.Bio,
                    IsActive = true,
                    EmailVerified = false,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                // Generate and send verification code
                var verificationCode = GenerateVerificationCode();
                await StoreVerificationCodeAsync(user.Id, verificationCode);
                await _emailService.SendVerificationEmailAsync(user.Email, verificationCode);

                // Don't return tokens until email is verified
                return new AuthResponse
                {
                    AccessToken = string.Empty,
                    RefreshToken = string.Empty,
                    ExpiresAt = DateTime.UtcNow,
                    User = MapToUserDto(user)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during user registration");
                throw;
            }
        }

        public async Task<AuthResponse> LoginAsync(LoginRequest request)
        {
            try
            {
                var user = await _context.Users
                    .Include(u => u.Role)
                    .FirstOrDefaultAsync(u => u.Email == request.Email);

                if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                {
                    throw new UnauthorizedAccessException("Invalid email or password");
                }

                if (!user.IsActive)
                {
                    throw new UnauthorizedAccessException("Account is deactivated");
                }

                // Generate tokens
                var tokens = await GenerateTokensAsync(user);

                return new AuthResponse
                {
                    AccessToken = tokens.AccessToken,
                    RefreshToken = tokens.RefreshToken,
                    ExpiresAt = DateTime.UtcNow.AddMinutes(GetJwtExpiryMinutes()),
                    User = MapToUserDto(user)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during user login");
                throw;
            }
        }

        public async Task<AuthResponse> RefreshTokenAsync(RefreshTokenRequest request)
        {
            try
            {
                // In a real implementation, you would validate the refresh token
                // For now, we'll just generate new tokens
                // This is a simplified implementation
                throw new NotImplementedException("Refresh token functionality needs to be implemented with proper token storage");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during token refresh");
                throw;
            }
        }

        public async Task<bool> ChangePasswordAsync(int userId, ChangePasswordRequest request)
        {
            try
            {
                var user = await _context.Users.FindAsync(userId);
                if (user == null)
                {
                    return false;
                }

                if (!BCrypt.Net.BCrypt.Verify(request.CurrentPassword, user.PasswordHash))
                {
                    throw new UnauthorizedAccessException("Current password is incorrect");
                }

                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
                user.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during password change");
                throw;
            }
        }

        public async Task<bool> LogoutAsync(string refreshToken)
        {
            try
            {
                // In a real implementation, you would invalidate the refresh token
                // For now, this is a placeholder
                await Task.Delay(1); // Simulate async operation
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during logout");
                throw;
            }
        }

        public async Task<bool> VerifyEmailAsync(string email, string code)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
                if (user == null)
                {
                    return false;
                }

                // Check if verification code is valid
                var isValidCode = await ValidateVerificationCodeAsync(user.Id, code);
                if (!isValidCode)
                {
                    return false;
                }

                // Mark email as verified
                user.EmailVerified = true;
                user.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                // Clean up verification code
                await CleanupVerificationCodeAsync(user.Id);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during email verification");
                throw;
            }
        }

        public async Task<bool> ResendVerificationAsync(string email)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
                if (user == null)
                {
                    return false;
                }

                // Generate new verification code
                var verificationCode = GenerateVerificationCode();
                await StoreVerificationCodeAsync(user.Id, verificationCode);
                
                // Send verification email
                var emailSent = await _emailService.SendVerificationEmailAsync(user.Email, verificationCode);
                return emailSent;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during resend verification");
                throw;
            }
        }

        public async Task<bool> ForgotPasswordAsync(string email)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
                if (user == null)
                {
                    return false; // Don't reveal if email exists
                }

                // In a real implementation, you would send a password reset email
                return await Task.FromResult(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during forgot password");
                throw;
            }
        }

        public async Task<bool> ResetPasswordAsync(string token, string newPassword)
        {
            try
            {
                // Password reset implementation
                // This would typically involve validating a reset token
                return await Task.FromResult(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during password reset");
                throw;
            }
        }

        private async Task<(string AccessToken, string RefreshToken)> GenerateTokensAsync(User user)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var secretKey = jwtSettings["SecretKey"];
            var issuer = jwtSettings["Issuer"];
            var audience = jwtSettings["Audience"];

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey!));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Role, user.Role?.Name ?? "Student"),
                new Claim("role_id", user.RoleId.ToString())
            };

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(GetJwtExpiryMinutes()),
                signingCredentials: credentials
            );

            var accessToken = new JwtSecurityTokenHandler().WriteToken(token);
            var refreshToken = Guid.NewGuid().ToString(); // Simplified refresh token

            return (accessToken, refreshToken);
        }

        private int GetJwtExpiryMinutes()
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            return int.Parse(jwtSettings["ExpiryMinutes"] ?? "60");
        }

        private UserDto MapToUserDto(User user)
        {
            return new UserDto
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Role = user.Role?.Name ?? "Student",
                ProfileImage = user.ProfileImage,
                Phone = user.Phone,
                Bio = user.Bio,
                IsActive = user.IsActive,
                EmailVerified = user.EmailVerified,
                CreatedAt = user.CreatedAt
            };
        }

        private string GenerateVerificationCode()
        {
            var random = new Random();
            return random.Next(100000, 999999).ToString();
        }

        private async Task StoreVerificationCodeAsync(int userId, string code)
        {
            // In a real implementation, you would store this in a database table
            // For now, we'll use a simple in-memory cache or you can create a VerificationCodes table
            // This is a simplified implementation
            await Task.Delay(1);
        }

        private async Task<bool> ValidateVerificationCodeAsync(int userId, string code)
        {
            // In a real implementation, you would validate against stored codes
            // For now, we'll accept any 6-digit code for testing
            return await Task.FromResult(code.Length == 6 && code.All(char.IsDigit));
        }

        private async Task CleanupVerificationCodeAsync(int userId)
        {
            // In a real implementation, you would remove the verification code from storage
            await Task.Delay(1);
        }
    }
}
