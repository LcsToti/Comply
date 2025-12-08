using FluentValidation;

namespace Payments.App.UseCases.UserCases.CreatePaymentAccount;

public class CreatePaymentAccountEventValidator : AbstractValidator<CreatePaymentAccountEvent>
{
    public CreatePaymentAccountEventValidator()
    {
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required")
                .NotNull().WithMessage("Name is required")
                .MaximumLength(50).WithMessage("Name must not exceed 50 characters")
                .MinimumLength(2).WithMessage("Name must not exceed 2 characters");
            RuleFor(x => x.Email)
                .EmailAddress().WithMessage("Email is required")
                .NotEmpty().WithMessage("Email is required");
        }
    }   
}