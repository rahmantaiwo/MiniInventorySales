namespace MiniInventorySales.Application.Interface.Services
{
    public interface IEmailTemplateService
    {
        string GetWelcomeEmail(string fullName);
        string GetUserOnboardingEmail(string fullName, string username, string temporaryPassword);
        string GetForgotPasswordEmail(string fullName, string resetLink);
        string GetPasswordChangedEmail(string fullName);
        string GetPasswordResetSuccessEmail(string fullName);
    }
}
