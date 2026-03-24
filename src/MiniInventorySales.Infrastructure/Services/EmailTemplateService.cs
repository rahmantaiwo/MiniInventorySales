using MiniInventorySales.Application.Interface.Services;

namespace MiniInventorySales.Infrastructure.Services
{
    public class EmailTemplateService : IEmailTemplateService
    {
        public string GetWelcomeEmail(string fullName)
        {
            return $@"
                <html>
                <body style='font-family: Arial, sans-serif;'>
                    <h2>Welcome to Mini Inventory Sales</h2>
                    <p>Hello {fullName},</p>
                    <p>Your account has been created successfully.</p>
                    <p>We are glad to have you onboard.</p>
                    <p>Regards,<br/>Mini Inventory Sales Team</p>
                </body>
                </html>";
        }

        public string GetUserOnboardingEmail(string fullName, string username, string temporaryPassword)
        {
            return $@"
                <html>
                <body style='font-family: Arial, sans-serif;'>
                    <h2>Your Account Has Been Created</h2>
                    <p>Hello {fullName},</p>
                    <p>Your user account has been created successfully.</p>
                    <p><strong>Username:</strong> {username}</p>
                    <p><strong>Temporary Password:</strong> {temporaryPassword}</p>
                    <p>Please log in and change your password immediately.</p>
                    <p>Regards,<br/>Mini Inventory Sales Team</p>
                </body>
                </html>";
        }

        public string GetForgotPasswordEmail(string fullName, string resetLink)
        {
            return $@"
                <html>
                <body style='font-family: Arial, sans-serif;'>
                    <h2>Password Reset Request</h2>
                    <p>Hello {fullName},</p>
                    <p>We received a request to reset your password.</p>
                    <p>Click the link below to reset your password:</p>
                    <p><a href='{resetLink}'>Reset Password</a></p>
                    <p>If you did not request this, please ignore this email.</p>
                    <p>Regards,<br/>Mini Inventory Sales Team</p>
                </body>
                </html>";
        }

        public string GetPasswordChangedEmail(string fullName)
        {
            return $@"
                <html>
                <body style='font-family: Arial, sans-serif;'>
                    <h2>Password Changed Successfully</h2>
                    <p>Hello {fullName},</p>
                    <p>Your password was changed successfully.</p>
                    <p>If you did not perform this action, please contact support immediately.</p>
                    <p>Regards,<br/>Mini Inventory Sales Team</p>
                </body>
                </html>";
        }

        public string GetPasswordResetSuccessEmail(string fullName)
        {
            return $@"
                <html>
                <body style='font-family: Arial, sans-serif;'>
                    <h2>Password Reset Successful</h2>
                    <p>Hello {fullName},</p>
                    <p>Your password has been reset successfully.</p>
                    <p>You can now log in with your new password.</p>
                    <p>Regards,<br/>Mini Inventory Sales Team</p>
                </body>
                </html>";
        }
    }
}
