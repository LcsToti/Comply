using System.ComponentModel.DataAnnotations;

namespace UserService.Contracts.DeliveryAddress;

public record UpdateDeliveryAddressRequest(
        [Required(ErrorMessage = "Rua é obrigatória")]
        [StringLength(200, MinimumLength = 2, ErrorMessage = "Rua deve ter entre 2 e 200 caracteres")]
        string Street,

        [Required(ErrorMessage = "Número é obrigatório")]
        [StringLength(20, MinimumLength = 1, ErrorMessage = "Número deve ter entre 1 e 20 caracteres")]
        string Number,

        [Required(ErrorMessage = "Cidade é obrigatória")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Cidade deve ter entre 2 e 100 caracteres")]
        string City,

        [Required(ErrorMessage = "Estado é obrigatório")]
        [StringLength(2, MinimumLength = 2, ErrorMessage = "Estado deve ter exatamente 2 caracteres")]
        string State,

        [Required(ErrorMessage = "CEP é obrigatório")]
        [RegularExpression(@"^\d{5}-?\d{3}$", ErrorMessage = "CEP deve estar no formato 00000-000 ou 00000000")]
        string ZipCode
    );