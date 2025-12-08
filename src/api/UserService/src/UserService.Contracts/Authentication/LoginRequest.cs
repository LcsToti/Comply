using System.ComponentModel.DataAnnotations;

namespace UserService.Contracts.Authentication
{
    public record LoginRequest(
        [Required(ErrorMessage = "Email é obrigatório")]
        [EmailAddress(ErrorMessage = "Formato de email inválido")]
        string Email,
        
        [Required(ErrorMessage = "Senha é obrigatória")]
        string Password
    );
}
