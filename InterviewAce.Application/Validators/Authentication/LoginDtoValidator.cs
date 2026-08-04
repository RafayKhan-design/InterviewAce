using FluentValidation;
using InterviewAce.Application.DTOs.Authentication;

namespace InterviewAce.Application.Validators.Authentication;

public class LoginDtoValidator
    : AbstractValidator<LoginDto>
{
    public LoginDtoValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.Password)
            .NotEmpty();
    }
}