using UserService.Domain.Entities;
using UserService.Domain.Enums;

namespace UserService.App.Interfaces
{
    public interface IRoleService
    {
        Task<bool> CanChangeRoleAsync(string currentUserId, string targetUserId, Role newRole);
        Task<bool> TryChangeUserRoleAsync(string currentAdminId, string targetUserId, Role newRole);
        Task<Role?> GetUserRoleAsync(string userId);
        Task<bool> IsAuthorizedForActionAsync(string userId, string action, string? resourceId = null);
        Task<bool> IsAdminAsync(string userId);
        Task<bool> IsModeratorAsync(string userId);
        Task<bool> IsUserAsync(string userId);
    }
}
