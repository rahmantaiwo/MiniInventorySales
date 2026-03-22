using MiniInventorySales.Application.Common;
using MiniInventorySales.Application.DTOs.Auth;

namespace MiniInventorySales.Application.Interface
{
    public interface IAuthService
    {
        Task<BaseResponse<string>> RegisterAsync(RegisterDto dto, CancellationToken ct);
        Task<BaseResponse<AuthSessionDto>> LoginAsync(LoginDto dto, CancellationToken ct);
    }
}
