using System.ComponentModel.DataAnnotations;

namespace UserService.Contracts.Profile
{
    public record UpdateProfileRequest(
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Nome deve ter entre 2 e 100 caracteres")]
        string? Name = null,
        
        [Phone(ErrorMessage = "Formato de telefone inválido")]
        [StringLength(20, ErrorMessage = "Telefone deve ter no máximo 20 caracteres")]
        string? PhoneNumber = null
        
        // [Url(ErrorMessage = "URL da foto de perfil inválida")]
        // [StringLength(500, ErrorMessage = "URL da foto deve ter no máximo 500 caracteres")]
        // string? ProfilePic = null
    );
}
