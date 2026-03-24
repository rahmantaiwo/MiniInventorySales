using MiniInventorySales.Application.Common;
using MiniInventorySales.Application.DTOs.Auth;

namespace MiniInventorySales.Application.Interface
{
    public interface IUserService
    {
        Task<BaseResponse<Guid>> CreateAsync(CreateUserRequest request, CancellationToken ct = default);
        Task<BaseResponse<UserDto>> GetByIdAsync(Guid id, CancellationToken ct = default);
        Task<BaseResponse<List<UserDto>>> GetAllAsync(CancellationToken ct = default);
        Task<BaseResponse<UserRoleDto>> UpdateRoleAsync(UpdateUserRoleRequest request, CancellationToken ct = default);
        Task<BaseResponse<string>> DeactivateAsync(Guid id, CancellationToken ct = default);
    }
}
