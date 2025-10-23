namespace EduStack.API.Services
{
    public interface IEmailService
    {
        Task<bool> SendVerificationEmailAsync(string email, string verificationCode);
        Task<bool> SendPasswordResetEmailAsync(string email, string resetToken);
    }
}
