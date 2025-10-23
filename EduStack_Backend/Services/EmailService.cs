using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace EduStack.API.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<bool> SendVerificationEmailAsync(string email, string verificationToken)
        {
            try
            {
                var smtpSettings = _configuration.GetSection("SmtpSettings");
                var smtpHost = smtpSettings["Host"];
                var smtpPort = int.Parse(smtpSettings["Port"] ?? "587");
                var smtpUsername = smtpSettings["Username"];
                var smtpPassword = smtpSettings["Password"];
                var fromEmail = smtpSettings["FromEmail"];
                var fromName = smtpSettings["FromName"] ?? "EduStack";

                using var client = new SmtpClient(smtpHost, smtpPort);
                client.Credentials = new NetworkCredential(smtpUsername, smtpPassword);
                client.EnableSsl = true;

                var message = new MailMessage();
                message.From = new MailAddress(fromEmail!, fromName);
                message.To.Add(email);
                message.Subject = "Verify Your EduStack Account";
                message.IsBodyHtml = true;
                
                var frontendUrl = _configuration["FrontendUrl"] ?? "http://localhost:5173";
                var verificationLink = $"{frontendUrl}/verify-email?token={verificationToken}";
                
                message.Body = $@"
                    <html>
                    <body style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto; padding: 20px;'>
                        <div style='text-align: center; margin-bottom: 30px;'>
                            <h1 style='color: #2563eb; margin: 0;'>🎓 EduStack</h1>
                            <p style='color: #666; margin: 5px 0 0 0;'>Learn Without Limits</p>
                        </div>
                        
                        <div style='background-color: #f8fafc; padding: 30px; border-radius: 8px; text-align: center;'>
                            <h2 style='color: #1f2937; margin: 0 0 20px 0;'>Verify Your Email Address</h2>
                            <p style='color: #4b5563; margin: 0 0 30px 0; line-height: 1.6;'>
                                Thank you for signing up with EduStack! To complete your registration, 
                                please click the button below to verify your email address:
                            </p>
                            
                            <a href='{verificationLink}' 
                               style='background-color: #2563eb; color: white; padding: 16px 32px; text-decoration: none; border-radius: 8px; display: inline-block; font-weight: 600; font-size: 16px; margin: 20px 0;'>
                                Verify Email Address
                            </a>
                            
                            <p style='color: #6b7280; margin: 30px 0 0 0; font-size: 14px;'>
                                This link will expire in 24 hours.
                            </p>
                            
                            <p style='color: #9ca3af; margin: 20px 0 0 0; font-size: 12px;'>
                                If the button doesn't work, copy and paste this link into your browser:<br>
                                <a href='{verificationLink}' style='color: #2563eb; word-break: break-all;'>{verificationLink}</a>
                            </p>
                        </div>
                        
                        <div style='text-align: center; margin-top: 30px; padding-top: 20px; border-top: 1px solid #e5e7eb;'>
                            <p style='color: #9ca3af; font-size: 12px; margin: 0;'>
                                If you didn't create an account with EduStack, please ignore this email.
                            </p>
                        </div>
                    </body>
                    </html>";

                await client.SendMailAsync(message);
                _logger.LogInformation($"Verification email sent to {email}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to send verification email to {email}");
                return false;
            }
        }

        public async Task<bool> SendPasswordResetEmailAsync(string email, string resetToken)
        {
            try
            {
                var smtpSettings = _configuration.GetSection("SmtpSettings");
                var smtpHost = smtpSettings["Host"];
                var smtpPort = int.Parse(smtpSettings["Port"] ?? "587");
                var smtpUsername = smtpSettings["Username"];
                var smtpPassword = smtpSettings["Password"];
                var fromEmail = smtpSettings["FromEmail"];
                var fromName = smtpSettings["FromName"] ?? "EduStack";

                using var client = new SmtpClient(smtpHost, smtpPort);
                client.Credentials = new NetworkCredential(smtpUsername, smtpPassword);
                client.EnableSsl = true;

                var message = new MailMessage();
                message.From = new MailAddress(fromEmail!, fromName);
                message.To.Add(email);
                message.Subject = "Reset Your EduStack Password";
                message.IsBodyHtml = true;
                message.Body = $@"
                    <html>
                    <body style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto; padding: 20px;'>
                        <div style='text-align: center; margin-bottom: 30px;'>
                            <h1 style='color: #2563eb; margin: 0;'>🎓 EduStack</h1>
                            <p style='color: #666; margin: 5px 0 0 0;'>Learn Without Limits</p>
                        </div>
                        
                        <div style='background-color: #f8fafc; padding: 30px; border-radius: 8px; text-align: center;'>
                            <h2 style='color: #1f2937; margin: 0 0 20px 0;'>Reset Your Password</h2>
                            <p style='color: #4b5563; margin: 0 0 30px 0; line-height: 1.6;'>
                                You requested to reset your password. Click the button below to reset it:
                            </p>
                            
                            <a href='{_configuration["FrontendUrl"]}/reset-password?token={resetToken}' 
                               style='background-color: #2563eb; color: white; padding: 12px 24px; text-decoration: none; border-radius: 6px; display: inline-block; font-weight: 500;'>
                                Reset Password
                            </a>
                            
                            <p style='color: #6b7280; margin: 30px 0 0 0; font-size: 14px;'>
                                This link will expire in 1 hour.
                            </p>
                        </div>
                        
                        <div style='text-align: center; margin-top: 30px; padding-top: 20px; border-top: 1px solid #e5e7eb;'>
                            <p style='color: #9ca3af; font-size: 12px; margin: 0;'>
                                If you didn't request a password reset, please ignore this email.
                            </p>
                        </div>
                    </body>
                    </html>";

                await client.SendMailAsync(message);
                _logger.LogInformation($"Password reset email sent to {email}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to send password reset email to {email}");
                return false;
            }
        }
    }
}
