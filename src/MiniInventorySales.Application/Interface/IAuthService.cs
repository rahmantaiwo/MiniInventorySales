using MiniInventorySales.Application.Common;
using MiniInventorySales.Application.DTOs.Auth;

namespace MiniInventorySales.Application.Interface
{
    public interface IAuthService
    {
        Task<BaseResponse<string>> RegisterAsync(RegisterRequest request, CancellationToken ct);
        Task<BaseResponse<AuthSessionDto>> LoginAsync(LoginRequest request, CancellationToken ct);
        Task<BaseResponse<string>> ForgotPasswordAsync(ForgotPasswordRequest request, CancellationToken ct);
        Task<BaseResponse<string>> ResetPasswordAsync(ResetPasswordRequest request, CancellationToken ct);
        Task<BaseResponse<string>> ChangePasswordAsync(ChangePasswordRequest request, CancellationToken ct);
    }
}
