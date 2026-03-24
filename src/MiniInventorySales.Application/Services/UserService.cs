using Microsoft.Extensions.Logging;
using MiniInventorySales.Application.Common;
using MiniInventorySales.Application.DTOs.Auth;
using MiniInventorySales.Application.Interface;
using MiniInventorySales.Application.Interface.Services;
using MiniInventorySales.Domain.Entities;
using MiniInventorySales.Domain.Interface;

namespace MiniInventorySales.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _users;
        private readonly IRoleRepository _roles;
        private readonly IPasswordHasher _passwordHasher;
        private readonly INotificationService _notifications;
        private readonly IEmailService _email;
        private readonly IEmailTemplateService _emailTemplates;
        private readonly IAuditLogService _auditLogs;
        private readonly ILogger<UserService> _logger;

        public UserService(
            IUserRepository userRepository,
            IRoleRepository roleRepository,
            IPasswordHasher passwordHasher,
            INotificationService notificationService,
            IEmailService emailService,
            IEmailTemplateService emailTemplateService,
            IAuditLogService auditLogService,
            ILogger<UserService> logger)
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

        public async Task<BaseResponse<Guid>> CreateAsync(CreateUserRequest request, CancellationToken ct = default)
        {
            try
            {
                _logger.LogInformation("Creating user {Email}", request.Email);

                var email = request.Email.Trim();
                var username = request.Username.Trim();

                var existingEmail = await _users.GetUserByEmailAsync(email, ct);
                if (existingEmail is not null)
                    return BaseResponse<Guid>.Failure("User creation failed.", "Email is already in use.");

                var existingUsername = await _users.GetUserByUsernameAsync(username, ct);
                if (existingUsername is not null)
                    return BaseResponse<Guid>.Failure("User creation failed.", "Username is already in use.");

                var role = await _roles.GetByIdAsync(request.RoleId, ct);
                if (role is null)
                    return BaseResponse<Guid>.Failure("User creation failed.", "Role not found.");

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

                await TrySendOnboardingEmailAsync(user, request.Password, ct);
                await TryCreateOnboardingNotificationAsync(user, ct);

                await _auditLogs.LogAsync(
                    user.Id,
                    "UserCreated",
                    nameof(AppUser),
                    user.Id.ToString(),
                    details: $"User {user.Email} created by admin.",
                    ct: ct);

                _logger.LogInformation("User created successfully {UserId}", user.Id);
                return BaseResponse<Guid>.IsSuccessful(user.Id, "User created successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create user {Email}", request.Email);
                return BaseResponse<Guid>.Failure("User creation failed.", ex.Message);
            }
        }

        public async Task<BaseResponse<UserDto>> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            try
            {
                var user = await _users.GetUserByIdAsync(id, ct);
                if (user is null)
                    return BaseResponse<UserDto>.Failure("User not found.");

                var role = await _roles.GetByIdAsync(user.RoleId, ct);

                var dto = new UserDto
                {
                    Id = user.Id,
                    FullName = user.FullName,
                    Email = user.Email,
                    Username = user.Username,
                    PhoneNumber = user.PhoneNumber ?? string.Empty,
                    RoleId = user.RoleId,
                    RoleName = role?.Name ?? string.Empty,
                    IsActive = user.IsActive,
                    CreatedAt = user.CreatedAt
                };

                return BaseResponse<UserDto>.IsSuccessful(dto, "User retrieved successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve user {UserId}", id);
                return BaseResponse<UserDto>.Failure("Failed to retrieve user.", ex.Message);
            }
        }

        public async Task<BaseResponse<List<UserDto>>> GetAllAsync(CancellationToken ct = default)
        {
            try
            {
                var users = await _users.GetAllAsync(ct);

                var rolesById = new Dictionary<Guid, string>();
                foreach (var user in users)
                {
                    if (rolesById.ContainsKey(user.RoleId))
                        continue;

                    var role = await _roles.GetByIdAsync(user.RoleId, ct);
                    if (role is not null)
                        rolesById[user.RoleId] = role.Name;
                }

                var items = users.Select(u => new UserDto
                {
                    Id = u.Id,
                    FullName = u.FullName,
                    Email = u.Email,
                    Username = u.Username,
                    PhoneNumber = u.PhoneNumber ?? string.Empty,
                    RoleId = u.RoleId,
                    RoleName = rolesById.TryGetValue(u.RoleId, out var name) ? name : string.Empty,
                    IsActive = u.IsActive,
                    CreatedAt = u.CreatedAt
                }).ToList();

                return BaseResponse<List<UserDto>>.IsSuccessful(items, "Users retrieved successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve users");
                return BaseResponse<List<UserDto>>.Failure("Failed to retrieve users.", ex.Message);
            }
        }

        public async Task<BaseResponse<UserRoleDto>> UpdateRoleAsync(UpdateUserRoleRequest request, CancellationToken ct = default)
        {
            try
            {
                var user = await _users.GetUserByIdAsync(request.UserId, ct);
                if (user is null)
                    return BaseResponse<UserRoleDto>.Failure("User not found.");

                var role = await _roles.GetByIdAsync(request.RoleId, ct);
                if (role is null)
                    return BaseResponse<UserRoleDto>.Failure("Role not found.");

                user.RoleId = role.Id;
                user.UpdatedAt = DateTime.UtcNow;
                await _users.UpdateUserAsync(user, ct);

                await _notifications.CreateAsync(user.Id, "Role Updated", $"Your role has been changed to {role.Name}.");

                await _auditLogs.LogAsync(
                    user.Id,
                    "UserRoleUpdated",
                    nameof(AppUser),
                    user.Id.ToString(),
                    details: $"Role updated to {role.Name}.",
                    ct: ct);

                _logger.LogInformation("User role updated {UserId} -> {Role}", user.Id, role.Name);

                return BaseResponse<UserRoleDto>.IsSuccessful(new UserRoleDto
                {
                    RoleId = role.Id,
                    RoleName = role.Name
                }, "User role updated successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update role for user {UserId}", request.UserId);
                return BaseResponse<UserRoleDto>.Failure("Failed to update user role.", ex.Message);
            }
        }

        public async Task<BaseResponse<string>> DeactivateAsync(Guid id, CancellationToken ct = default)
        {
            try
            {
                var success = await _users.DeactivateUser(id, ct);
                if (!success)
                    return BaseResponse<string>.Failure("Deactivate failed.", "User not found or already inactive.");

                await _auditLogs.LogAsync(
                    id,
                    "UserDeactivated",
                    nameof(AppUser),
                    id.ToString(),
                    details: "User deactivated.",
                    ct: ct);

                _logger.LogInformation("User deactivated {UserId}", id);
                return BaseResponse<string>.IsSuccessful("OK", "User deactivated successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to deactivate user {UserId}", id);
                return BaseResponse<string>.Failure("Failed to deactivate user.", ex.Message);
            }
        }

        private async Task TrySendOnboardingEmailAsync(AppUser user, string temporaryPassword, CancellationToken ct)
        {
            try
            {
                var body = _emailTemplates.GetUserOnboardingEmail(user.FullName, user.Username, temporaryPassword);
                await _email.SendEmailAsync(user.Email, "Your account has been created", body);
                _logger.LogInformation("Onboarding email sent to {Email}", user.Email);
            }
            catch
            {
                _logger.LogWarning("Failed to send onboarding email to {Email}", user.Email);
            }
        }

        private async Task TryCreateOnboardingNotificationAsync(AppUser user, CancellationToken ct)
        {
            try
            {
                await _notifications.CreateAsync(user.Id, "Account Created", "Your account has been created successfully.");
                _logger.LogInformation("Onboarding notification created for {UserId}", user.Id);
            }
            catch
            {
                _logger.LogWarning("Failed to create onboarding notification for {UserId}", user.Id);
            }
        }
    }
}
