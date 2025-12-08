using System.ComponentModel.DataAnnotations;
using UserService.Domain.Entities;

namespace UserService.Contracts.Role
{
    public record ChangeRoleRequest(
        [Required(ErrorMessage = "ID do usuário é obrigatório")]
        string UserId,
        
        [Required(ErrorMessage = "Role é obrigatória")]
        Domain.Enums.Role NewRole
    );
}
