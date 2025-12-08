using UserService.App.Interfaces;
using UserService.Domain.Entities;
using UserService.Domain.Enums;
using UserService.Domain.Interfaces;

namespace UserService.App.Services
{
    public class RoleService : IRoleService
    {
        private readonly IUserRepository _userRepository;

        public RoleService(IUserRepository userRepository)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        }

        public async Task<bool> CanChangeRoleAsync(string currentUserId, string targetUserId, Role newRole)
        {
            var currentUser = await _userRepository.GetByIdAsync(currentUserId);
            var targetUser = await _userRepository.GetByIdAsync(targetUserId);

            if (currentUser is null || targetUser is null) return false;
            
            if (!currentUser.IsAdmin()) return false;
            
            if (targetUser.IsAdmin() && currentUser.Id != targetUser.Id) return false;
            
            return true;
        }
        public async Task<bool> TryChangeUserRoleAsync(string currentAdminId, string targetUserId, Role newRole)
        {
            if (!await CanChangeRoleAsync(currentAdminId, targetUserId, newRole))
            {
                return false;
            }

            var targetUser = await _userRepository.GetByIdAsync(targetUserId);
            if (targetUser is null)
            {
                return false; 
            }

            targetUser.ChangeRole(newRole);
            await _userRepository.UpdateAsync(targetUser);
            
            return true;
        }
        public async Task<Role?> GetUserRoleAsync(string userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            return user?.Role;
        }
        public async Task<bool> IsAuthorizedForActionAsync(string userId, string action, string? resourceId = null)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user is null)
            {
                return false;
            }

            return action switch
            {
                "view_profile" => true,
                "edit_own_profile" => resourceId == null || user.Id == resourceId,
                "create_support_ticket" => true,
                "admin_actions" => user.IsAdmin(),
                "moderator_actions" => user.IsAdmin() || user.IsModerator(),
                _ => false
            };
        }
        public async Task<bool> IsAdminAsync(string userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            return user?.IsAdmin() == true;
        }
        public async Task<bool> IsModeratorAsync(string userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            return user?.IsModerator() == true;
        }
        public async Task<bool> IsUserAsync(string userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            return user?.IsUser() == true;
        }
    }
}
