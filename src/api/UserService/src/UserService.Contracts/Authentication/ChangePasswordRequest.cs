using System.ComponentModel.DataAnnotations;

namespace UserService.Contracts.Authentication
{
    public record ChangePasswordRequest(
        [Required(ErrorMessage = "Senha atual é obrigatória")]
        string CurrentPassword,
        
        [Required(ErrorMessage = "Nova senha é obrigatória")]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Nova senha deve ter entre 8 e 100 caracteres")]
        string NewPassword,
        
        [Required(ErrorMessage = "Confirmação de senha é obrigatória")]
        string ConfirmPassword
    );
}
