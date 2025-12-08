using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using UserService.App.Interfaces;
using UserService.Domain.Enums;

namespace UserService.Api.Controllers
{
    [ApiController]
    [Route("api/v1/Users")]
    [Authorize(Roles = "Moderator")]
    public class RoleManagementController : ControllerBase
    {
        private readonly IRoleService _roleService;

        public RoleManagementController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        [HttpPut("{userId}/role")]
        public async Task<IActionResult> ChangeUserRole([FromRoute] string userId, [FromBody] ChangeRoleRequest request)
        {
            var currentAdminId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(currentAdminId))
            {
                return Unauthorized();
            }

            var success = await _roleService.TryChangeUserRoleAsync(currentAdminId, userId, request.NewRole);

            if (!success)
            {
                return Forbid("Operação não permitida ou usuário não encontrado.");
            }

            return Ok(new { message = $"Role do usuário {userId} alterada para {request.NewRole} com sucesso." });
        }

        [HttpGet("{userId}/role")]
        public async Task<IActionResult> GetUserRole([FromRoute] string userId)
        {
            var role = await _roleService.GetUserRoleAsync(userId);

            if (role is null)
            {
                return NotFound(new { message = "Usuário não encontrado." });
            }

            return Ok(new { userId, role = role.ToString() });
        }
    }

    public record ChangeRoleRequest(Role NewRole);
}