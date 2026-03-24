using System.Security.Cryptography;
using MiniInventorySales.Application.Common;
using MiniInventorySales.Application.DTOs.Auth;
using MiniInventorySales.Application.Interface;
using MiniInventorySales.Application.Interface.Services;
using MiniInventorySales.Domain.Entities;
using MiniInventorySales.Domain.Interface;
using Microsoft.Extensions.Logging;

namespace MiniInventorySales.Application.Services
{
    public class AuthService : IAuthService
    {
        private const string DefaultRoleName = "User";
        private const int ResetTokenExpiryMinutes = 60;

        private readonly IUserRepository _users;
        private readonly IRoleRepository _roles;
        private readonly IPasswordHasher _passwordHasher;
        private readonly INotificationService _notifications;
        private readonly IEmailService _email;
        private readonly IEmailTemplateService _emailTemplates;
        private readonly IAuditLogService _auditLogs;
        private readonly ILogger<AuthService> _logger;

        public AuthService(
            IUserRepository userRepository,
            IRoleRepository roleRepository,
            IPasswordHasher passwordHasher,
            INotificationService notificationService,
            IEmailService emailService,
            IEmailTemplateService emailTemplateService,
            IAuditLogService auditLogService,
            ILogger<AuthService> logger)
        {
            _users = userRepository;
            _roles = roleRepository;
            _passwordHasher = passwordHasher;
            _notifications = notificationService;
            _email = emailService;
            _emailTemplates = emailTemplateService;
            _auditLogs = auditLogService;
            _logger = logger;
        }

        public async Task<BaseResponse<string>> RegisterAsync(RegisterRequest request, CancellationToken ct)
        {
            try
            {
                _logger.LogInformation("Registering user with email {Email}", request.Email);

                if (request.Password != request.ConfirmPassword)
                {
                    _logger.LogWarning("Registration failed due to password mismatch for {Email}", request.Email);
                    return BaseResponse<string>.Failure("Password mismatch.", "Password and confirm password do not match.");
                }

                var email = request.Email.Trim();
                var username = request.Username.Trim();

                var existingEmail = await _users.GetUserByEmailAsync(email, ct);
                if (existingEmail is not null)
                {
                    _logger.LogWarning("Registration failed: email already in use {Email}", email);
                    return BaseResponse<string>.Failure("Registration failed.", "Email is already in use.");
                }

                var existingUsername = await _users.GetUserByUsernameAsync(username, ct);
                if (existingUsername is not null)
                {
                    _logger.LogWarning("Registration failed: username already in use {Username}", username);
                    return BaseResponse<string>.Failure("Registration failed.", "Username is already in use.");
                }

                var role = await _roles.GetByNameAsync(DefaultRoleName, ct);
                if (role is null)
                {
                    role = new Role
                    {
                        Name = DefaultRoleName,
                        Description = "Default role"
                    };
                    await _roles.AddAsync(role, ct);
                }

                var user = new AppUser
                {
                    FullName = request.FullName.Trim(),
                    Email = email,
                    Username = username,
                    PhoneNumber = request.PhoneNumber.Trim(),
                    PasswordHash = _passwordHasher.HashPassword(request.Password),
                    RoleId = role.Id,
                    IsActive = true
                };

                await _users.AddUserAsync(user, ct);

                await TrySendWelcomeEmailAsync(user, ct);
                await TryCreateWelcomeNotificationAsync(user, ct);
                await _auditLogs.LogAsync(
                    user.Id,
                    "UserRegistered",
                    nameof(AppUser),
                    user.Id.ToString(),
                    details: $"User {user.Email} registered.",
                    ct: ct);

                _logger.LogInformation("User registered successfully {UserId}", user.Id);

                return BaseResponse<string>.IsSuccessful(user.Id.ToString(), "Registration successful.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Registration failed for {Email}", request.Email);
                return BaseResponse<string>.Failure("Registration failed.", ex.Message);
            }
        }

        public async Task<BaseResponse<AuthSessionDto>> LoginAsync(LoginRequest request, CancellationToken ct)
        {
            try
            {
                _logger.LogInformation("Login attempt for {Identifier}", request.EmailOrUsername);

                var identifier = request.EmailOrUsername.Trim();

                var user = await _users.GetUserByEmailAsync(identifier, ct)
                           ?? await _users.GetUserByUsernameAsync(identifier, ct);

                if (user is null)
                {
                    _logger.LogWarning("Login failed: user not found for {Identifier}", identifier);
                    await _auditLogs.LogAsync(
                        null,
                        "LoginFailed",
                        nameof(AppUser),
                        details: $"Invalid login attempt for {identifier}.",
                        ct: ct);
                    return BaseResponse<AuthSessionDto>.Failure("Login failed.", "Invalid credentials.");
                }

                if (!user.IsActive)
                {
                    _logger.LogWarning("Login failed: account deactivated for {UserId}", user.Id);
                    await _auditLogs.LogAsync(
                        user.Id,
                        "LoginFailed",
                        nameof(AppUser),
                        user.Id.ToString(),
                        details: "Login attempt on deactivated account.",
                        ct: ct);
                    return BaseResponse<AuthSessionDto>.Failure("Login failed.", "Account is deactivated.");
                }

                if (!_passwordHasher.VerifyPassword(request.Password, user.PasswordHash))
                {
                    _logger.LogWarning("Login failed: invalid password for {UserId}", user.Id);
                    await _auditLogs.LogAsync(
                        user.Id,
                        "LoginFailed",
                        nameof(AppUser),
                        user.Id.ToString(),
                        details: "Invalid password.",
                        ct: ct);
                    return BaseResponse<AuthSessionDto>.Failure("Login failed.", "Invalid credentials.");
                }

                user.LastLoginAt = DateTime.UtcNow;
                user.UpdatedAt = DateTime.UtcNow;
                await _users.UpdateUserAsync(user, ct);

                var role = await _roles.GetByIdAsync(user.RoleId, ct);

                var session = new AuthSessionDto
                {
                    UserId = user.Id,
                    FullName = user.FullName,
                    Email = user.Email,
                    Username = user.Username,
                    RoleId = user.RoleId,
                    RoleName = role?.Name ?? string.Empty,
                    LastLoginAt = user.LastLoginAt,
                    IsActive = user.IsActive
                };

                await _auditLogs.LogAsync(
                    user.Id,
                    "UserLoggedIn",
                    nameof(AppUser),
                    user.Id.ToString(),
                    details: "User logged in successfully.",
                    ct: ct);

                _logger.LogInformation("Login successful for {UserId}", user.Id);
                return BaseResponse<AuthSessionDto>.IsSuccessful(session, "Login successful.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Login failed for {Identifier}", request.EmailOrUsername);
                return BaseResponse<AuthSessionDto>.Failure("Login failed.", ex.Message);
            }
        }

        public async Task<BaseResponse<string>> ForgotPasswordAsync(ForgotPasswordRequest request, CancellationToken ct)
        {
            try
            {
                _logger.LogInformation("Password reset requested for {Email}", request.Email);

                var email = request.Email.Trim();
                var user = await _users.GetUserByEmailAsync(email, ct);
                if (user is null)
                {
                    _logger.LogWarning("Password reset requested for non-existent email {Email}", email);
                    return BaseResponse<string>.IsSuccessful("OK", "If the email exists, a reset link has been sent.");
                }

                var resetToken = GenerateResetToken();
                user.PasswordResetToken = _passwordHasher.HashPassword(resetToken);
                user.PasswordResetTokenExpiresAt = DateTime.UtcNow.AddMinutes(ResetTokenExpiryMinutes);
                user.UpdatedAt = DateTime.UtcNow;

                await _users.UpdateUserAsync(user, ct);

                var resetLink = BuildResetLink(email, resetToken);
                var body = _emailTemplates.GetForgotPasswordEmail(user.FullName, resetLink);
                await _email.SendEmailAsync(user.Email, "Password reset request", body);

                await _auditLogs.LogAsync(
                    user.Id,
                    "PasswordResetRequested",
                    nameof(AppUser),
                    user.Id.ToString(),
                    details: "Password reset link sent.",
                    ct: ct);

                _logger.LogInformation("Password reset email sent to {Email}", user.Email);
                return BaseResponse<string>.IsSuccessful("OK", "If the email exists, a reset link has been sent.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to process forgot password for {Email}", request.Email);
                return BaseResponse<string>.Failure("Failed to process password reset request.", ex.Message);
            }
        }

        public async Task<BaseResponse<string>> ResetPasswordAsync(ResetPasswordRequest request, CancellationToken ct)
        {
            try
            {
                var email = request.Email.Trim();
                var user = await _users.GetUserByEmailAsync(email, ct);
                if (user is null)
                    return BaseResponse<string>.Failure("Reset failed.", "Invalid reset token or email.");

                if (user.PasswordResetToken is null || user.PasswordResetTokenExpiresAt is null)
                    return BaseResponse<string>.Failure("Reset failed.", "Invalid reset token or email.");

                if (user.PasswordResetTokenExpiresAt < DateTime.UtcNow)
                    return BaseResponse<string>.Failure("Reset failed.", "Reset token has expired.");

                if (!_passwordHasher.VerifyPassword(request.Token, user.PasswordResetToken))
                    return BaseResponse<string>.Failure("Reset failed.", "Invalid reset token or email.");

                if (request.NewPassword != request.ConfirmPassword)
                    return BaseResponse<string>.Failure("Reset failed.", "New password and confirm password do not match.");

                user.PasswordHash = _passwordHasher.HashPassword(request.NewPassword);
                user.PasswordResetToken = null;
                user.PasswordResetTokenExpiresAt = null;
                user.UpdatedAt = DateTime.UtcNow;

                await _users.UpdateUserAsync(user, ct);

                var body = _emailTemplates.GetPasswordResetSuccessEmail(user.FullName);
                await _email.SendEmailAsync(user.Email, "Password reset successful", body);
                await _notifications.CreateAsync(user.Id, "Password Reset", "Your password has been reset successfully.");

                await _auditLogs.LogAsync(
                    user.Id,
                    "PasswordResetCompleted",
                    nameof(AppUser),
                    user.Id.ToString(),
                    details: "Password reset completed.",
                    ct: ct);

                _logger.LogInformation("Password reset completed for {UserId}", user.Id);
                return BaseResponse<string>.IsSuccessful("OK", "Password reset successful.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to reset password for {Email}", request.Email);
                return BaseResponse<string>.Failure("Failed to reset password.", ex.Message);
            }
        }

        public async Task<BaseResponse<string>> ChangePasswordAsync(ChangePasswordRequest request, CancellationToken ct)
        {
            try
            {
                var user = await _users.GetUserByIdAsync(request.UserId, ct);
                if (user is null)
                    return BaseResponse<string>.Failure("Change password failed.", "User not found.");

                if (!_passwordHasher.VerifyPassword(request.CurrentPassword, user.PasswordHash))
                    return BaseResponse<string>.Failure("Change password failed.", "Current password is incorrect.");

                if (request.NewPassword != request.ConfirmNewPassword)
                    return BaseResponse<string>.Failure("Change password failed.", "New password and confirm password do not match.");

                user.PasswordHash = _passwordHasher.HashPassword(request.NewPassword);
                user.UpdatedAt = DateTime.UtcNow;

                await _users.UpdateUserAsync(user, ct);

                var body = _emailTemplates.GetPasswordChangedEmail(user.FullName);
                await _email.SendEmailAsync(user.Email, "Password changed", body);
                await _notifications.CreateAsync(user.Id, "Password Changed", "Your password was changed successfully.");

                await _auditLogs.LogAsync(
                    user.Id,
                    "PasswordChanged",
                    nameof(AppUser),
                    user.Id.ToString(),
                    details: "Password changed by user.",
                    ct: ct);

                _logger.LogInformation("Password changed for {UserId}", user.Id);
                return BaseResponse<string>.IsSuccessful("OK", "Password changed successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to change password for {UserId}", request.UserId);
                return BaseResponse<string>.Failure("Failed to change password.", ex.Message);
            }
        }

        private async Task TrySendWelcomeEmailAsync(AppUser user, CancellationToken ct)
        {
            try
            {
                var body = _emailTemplates.GetWelcomeEmail(user.FullName);
                await _email.SendEmailAsync(user.Email, "Welcome to Mini Inventory Sales", body);
                _logger.LogInformation("Welcome email sent to {Email}", user.Email);
            }
            catch
            {
                _logger.LogWarning("Failed to send welcome email to {Email}", user.Email);
                // Best-effort: registration should not fail on email issues.
            }
        }

        private async Task TryCreateWelcomeNotificationAsync(AppUser user, CancellationToken ct)
        {
            try
            {
                await _notifications.CreateAsync(user.Id, "Welcome", "Your account has been created successfully.");
                _logger.LogInformation("Welcome notification created for {UserId}", user.Id);
            }
            catch
            {
                _logger.LogWarning("Failed to create welcome notification for {UserId}", user.Id);
                // Best-effort: registration should not fail on notification issues.
            }
        }

        private static string GenerateResetToken()
        {
            var bytes = RandomNumberGenerator.GetBytes(32);
            var token = Convert.ToBase64String(bytes);
            return token.Replace("+", "-").Replace("/", "_").TrimEnd('=');
        }

        private static string BuildResetLink(string email, string token)
        {
            var encodedEmail = Uri.EscapeDataString(email);
            var encodedToken = Uri.EscapeDataString(token);
            return $"/Auth/ResetPassword?email={encodedEmail}&token={encodedToken}";
        }
    }
}
