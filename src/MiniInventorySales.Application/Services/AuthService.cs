using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using MiniInventorySales.Application.Common;
using MiniInventorySales.Application.DTOs.Auth;
using MiniInventorySales.Application.Interface;
using MiniInventorySales.Domain.Entities;
using MiniInventorySales.Domain.Enums;
using MiniInventorySales.Domain.Interface;
using System.Net.Http;
using System.Security.Claims;

namespace MiniInventorySales.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _users;
        private readonly IUnitOfWork _uow;
        private readonly PasswordHasher<AppUser> _hasher = new();

        public AuthService(IUserRepository users, IUnitOfWork uow)
        {
            _users = users;
            _uow = uow;
        }
            
        public async Task<BaseResponse<AuthSessionDto>> LoginAsync(LoginDto dto, CancellationToken ct)
        {
            var email = dto.Email.Trim().ToLowerInvariant();
            var user = await _users.GetUserByEmailAsync(email, ct);
            if(user == null)
                return BaseResponse<AuthSessionDto>.Failure("Login failed", "Invalid email or password");

            var result = _hasher.VerifyHashedPassword(user, user.PasswordHash, dto.Password);
            if(result == PasswordVerificationResult.Failed)
                return BaseResponse<AuthSessionDto>.Failure("Login failed", "Invalid email or password");

            var session = new AuthSessionDto
            {
                UserId = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Role = user.Role.ToString()
            };

            return BaseResponse<AuthSessionDto>.IsSuccessful(session, "Login successful");
        }

        public async Task<BaseResponse<string>> RegisterAsync(RegisterDto dto, CancellationToken ct)
        {
            var email = dto.Email.Trim().ToLowerInvariant();

            var exist = await _users.GetUserByEmailAsync(email, ct);
            if(exist != null)
                return BaseResponse<string>.Failure("Registration failed", "Email already exists");

            var user = new AppUser
            {
                FirstName = dto.FirstName.Trim(),
                LastName = dto.LastName.Trim(),
                Email = email,
                Role = UserRole.Staff
            };

            user.PasswordHash = _hasher.HashPassword(user, dto.Password);

            await _users.AddUserAsync(user, ct);
            await _uow.SaveChangesAsync(ct);

            return BaseResponse<string>.IsSuccessful("OK", "Account created successfully.");
        }
    }
}

